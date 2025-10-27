using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using iTextSharp.text.pdf;
using ImageMagick;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.Log;
using iTextSharp.text.exceptions;
using System.Collections.Concurrent;
/*
using org.apache.pdfbox.pdmodel;
using org.apache.pdfbox.pdfviewer;
*/

namespace RaaiVan.Modules.Documents
{
    public static class PDF2ImageProcessing
    {
        private static ConcurrentDictionary<Guid, bool> Files = new ConcurrentDictionary<Guid, bool>();

        private static string redis_key(Guid fileId)
        {
            return fileId.ToString() + "_pdf_processing";
        }

        public static bool? is_processing(Guid fileId)
        {
            if (RedisAPI.Enabled)
                return RedisAPI.get_value<bool?>(redis_key(fileId));
            else
            {
                if (!Files.ContainsKey(fileId)) return null;
                else return Files[fileId];
            }
        }

        public static void is_processing(Guid fileId, bool value)
        {
            if (RedisAPI.Enabled)
                RedisAPI.set_value<bool>(redis_key(fileId), value);
            else Files[fileId] = value;
        }
    }

    public static class PDFUtil
    {
        public static PdfReader open_pdf_file(byte[] fileBytes, string password, ref bool invalidPassword) {
            invalidPassword = false;

            try
            {
                return string.IsNullOrEmpty(password) ? new PdfReader(fileBytes) :
                    new PdfReader(fileBytes, ownerPassword: Encoding.UTF8.GetBytes(password));
            }
            catch (BadPasswordException)
            {
                invalidPassword = true;
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static PdfReader open_pdf_file(DocFileInfo pdf, string password, ref bool invalidPassword)
        {
            return open_pdf_file(pdf.toByteArray(), password, ref invalidPassword);
        }

        public static byte[] to_byte_array(PdfReader reader) {
            try
            {
                if (reader == null) return new byte[0];

                using (MemoryStream memoryStream = new MemoryStream())
                using (PdfStamper stamper = new PdfStamper(reader, memoryStream))
                {
                    stamper.Close();
                    reader.Close();
                    return memoryStream.ToArray();
                }
            }
            catch { return new byte[0]; }
        }

        public static byte[] get_pdf_content(byte[] fileBytes, string password, ref bool invalidPassword) {
            PdfReader reader = PDFUtil.open_pdf_file(fileBytes, password, ref invalidPassword);

            if (reader != null) return PDFUtil.to_byte_array(reader);
            else return new byte[0];
        }

        public static int get_pages_count(DocFileInfo pdf, string password, ref bool invalidPassword)
        {
            try
            {
                PdfReader reader = PDFUtil.open_pdf_file(pdf, password, ref invalidPassword);

                return reader == null ? 0 : reader.NumberOfPages;
            }
            catch { return 0; }
        }

        public static int get_converted_pages_count(DocFileInfo destFile) {
            return destFile.files_count_in_folder();
        }

        public static Bitmap get_image_magick(Guid applicationId, string pdfPath, int pageNum, ImageFormat imageFormat)
        {
            //MagickNET.SetGhostscriptDirectory("[GhostScript DLL Dir]");
            //MagickNET.SetTempDirectory("[a temp dir]");

            //pageNum starts from 1, but ImageMagick starts paging from 0; so, ...
            pageNum -= 1;

            MagickReadSettings settings = new MagickReadSettings();
            settings.Density = new Density(300, 300);

            using (MagickImageCollection pages = new MagickImageCollection())
            {
                try
                {
                    pages.Read(pdfPath, settings);

                    MagickImage img = (MagickImage)pages[pageNum];
                    return img.ToBitmap(imageFormat);
                }
                catch (Exception ex)
                {
                    LogController.save_error_log(applicationId, null, "ConvertPDFPageToImage", ex, ModuleIdentifier.DCT);
                    return null;
                }
            }
        }

        public static void pdf2image(Guid applicationId,
            DocFileInfo pdf, string password, DocFileInfo destFile, ImageFormat imageFormat, bool repair)
        {
            //MagickNET.SetGhostscriptDirectory("[GhostScript DLL Dir]");
            //MagickNET.SetTempDirectory("[a temp dir]");

            bool? isProcessing = !pdf.FileID.HasValue ? null : PDF2ImageProcessing.is_processing(pdf.FileID.Value);

            if (!pdf.FileID.HasValue) return;
            else if (isProcessing.HasValue && (!repair || isProcessing.Value)) return;
            else PDF2ImageProcessing.is_processing(pdf.FileID.Value, true);

            if (destFile.file_exists_in_folder() && !repair) {
                PDF2ImageProcessing.is_processing(pdf.FileID.Value, false);
                return;
            }

            try
            {
                string cacheDir = PublicMethods.map_path(PublicConsts.MagickCacheDirectory, localPath: true);
                if (!Directory.Exists(cacheDir)) Directory.CreateDirectory(cacheDir);
                MagickAnyCPU.CacheDirectory = cacheDir;
            }
            catch (Exception ex)
            {
                LogController.save_error_log(applicationId, null, "SetMagickCacheDirectory", ex, ModuleIdentifier.DCT);
            }

            try
            {
                string tempDir = PublicMethods.map_path(PublicConsts.TempDirectory, localPath: true);
                if (!Directory.Exists(tempDir)) Directory.CreateDirectory(tempDir);
                MagickNET.SetTempDirectory(tempDir);

                if (!string.IsNullOrEmpty(RaaiVanSettings.GhostScriptDirectory))
                    MagickNET.SetGhostscriptDirectory(RaaiVanSettings.GhostScriptDirectory);
            }
            catch (Exception ex)
            {
                LogController.save_error_log(applicationId, null, "SetMagickTempDirectory", ex, ModuleIdentifier.DCT);
            }

            try
            {
                using (MagickImageCollection pages = new MagickImageCollection())
                {
                    MagickReadSettings settings = new MagickReadSettings() { Density = new Density(100, 100) };

                    bool invalidPassword = false;
                    
                    using (PdfReader reader = PDFUtil.open_pdf_file(pdf, password, ref invalidPassword))
                    {
                        byte[] pdfContent = PDFUtil.to_byte_array(reader);
                        pages.Read(pdfContent, settings);
                    }

                    int pageNum = 0;
                    bool errorLoged = false;

                    foreach (MagickImage p in pages)
                    {
                        ++pageNum;

                        destFile.FileName = pageNum.ToString();
                        destFile.Extension = imageFormat.ToString().ToLower();

                        if (destFile.exists()) continue;

                        try
                        {
                            using (MemoryStream stream = new MemoryStream())
                            {
                                p.ToBitmap(imageFormat).Save(stream, imageFormat);
                                destFile.store(stream.ToArray());
                            }
                        }
                        catch (Exception ex)
                        {
                            if (!errorLoged)
                            {
                                errorLoged = true;
                                LogController.save_error_log(applicationId, null,
                                    "ConvertPDFPageToImage", ex, ModuleIdentifier.DCT);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogController.save_error_log(applicationId, null, "ConvertPDFToImage", ex, ModuleIdentifier.DCT);
            }

            PDF2ImageProcessing.is_processing(pdf.FileID.Value, false);
        }

        public static byte[] merge_documents(List<object> docs)
        {
            MemoryStream stream = new MemoryStream();
            PdfConcatenate concat = new PdfConcatenate(stream);

            foreach (object doc in docs)
            {
                PdfReader reader;

                if (doc.GetType() == typeof(string))
                    reader = new PdfReader((string)doc);
                else if (doc.GetType() == typeof(byte[]))
                    reader = new PdfReader((byte[])doc);
                else if (doc.GetType() == typeof(Stream))
                    reader = new PdfReader((Stream)doc);
                else
                    continue;

                int pagesCount = reader.NumberOfPages;
                if (pagesCount == 0) continue;
                reader.SelectPages("1-" + pagesCount.ToString());
                concat.AddPages(reader);
                reader.Close();
            }

            concat.Close();

            return stream.ToArray();
        }

        public static byte[] set_password(byte[] pdfContent, string password)
        {
            if (string.IsNullOrEmpty(password)) return pdfContent;
            else
            {
                using (PdfReader reader = new PdfReader(pdfContent))
                using (MemoryStream m = new MemoryStream())
                {
                    PdfEncryptor.Encrypt(reader, m, true, password, password, PdfAWriter.ALLOW_SCREENREADERS);
                    return m.ToArray();
                }
            }
        }

        /*
        public static byte[] get_image(string pdfPath, int pageNum)
        {
            try
            {
                PageDrawer pagedrawer = new PageDrawer();
                PDDocument document = PDDocument.load(pdfPath);
                java.util.List pages = document.getDocumentCatalog().getAllPages();
                if (pages.size() < pageNum) return new byte[0];
                PDPage page = (PDPage)pages.get(pageNum - 1);

                java.awt.image.BufferedImage image = page.convertToImage();
                Bitmap bmp = image.getBitmap();
                bmp.SetResolution(1024, 768);

                byte[] ret = null;

                using (MemoryStream stream = new MemoryStream())
                {
                    bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                    ret = stream.ToArray();
                }

                return ret;
            }
            catch { return null; }
        }

        public static Bitmap get_image_ghostscript(string pdfPath, int pageNum)
        {
            try
            {
                Pdf2Image _pdf = new Pdf2Image(pdfPath);
                if (pageNum > _pdf.PageCount) return null;

                Pdf2ImageSettings settings;

                settings = new Pdf2ImageSettings();
                settings.AntiAliasMode = AntiAliasMode.High;
                settings.Dpi = 300;
                settings.GridFitMode = GridFitMode.Topological;
                settings.ImageFormat = Cyotek.GhostScript.ImageFormat.Tiff24;
                settings.TrimMode = PdfTrimMode.CropBox;

                return _pdf.GetImage(pageNum);
            }
            catch { return null; }
        }
        */

        /* -- Usefull Code, maybe sometime can be helpfull
        public static void merge_files(string destinationFile, List<string> sourceFiles)
        {
            using (Document document = new Document(PageSize.A4))
            {
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(destinationFile, FileMode.OpenOrCreate));

                document.Open();

                PdfContentByte cb = writer.DirectContent;
                document.SetPageSize(PageSize.A4);
                
                foreach (string _file in sourceFiles)
                {
                    PdfReader reader = new PdfReader(_file);

                    for (int i = 1, lnt = reader.NumberOfPages; i <= lnt; ++i)
                    {
                        document.NewPage();
                        PdfImportedPage page = writer.GetImportedPage(reader, i);
                        
                        int rotation = reader.GetPageRotation(i);

                        if (rotation == 90 || rotation == 270)
                            cb.AddTemplate(page, 0, -1f, 1f, 0, 0, reader.GetPageSizeWithRotation(i).Height);
                        else
                            cb.AddTemplate(page, 1f, 0, 0, 1f, 0, 0);
                    }
                }
            } //end of 'using(Document document = ...'
        }
        */
    }
}
