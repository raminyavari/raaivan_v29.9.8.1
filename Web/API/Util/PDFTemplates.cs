using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.FormGenerator;

namespace RaaiVan.Web.API
{
    public class PDFTemplates
    {
        public static string FontName = "IRANSans";
        public static string FontAddress = PublicConsts.FontIranSansWindows;

        public static Font GetFont()
        {
            if (!FontFactory.IsRegistered(FontName))
                FontFactory.Register(PublicMethods.map_path(FontAddress));
            
            return FontFactory.GetFont(FontName, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
        }

        public static BaseFont GetBaseFont()
        {
            return BaseFont.CreateFont(PublicMethods.map_path(FontAddress), BaseFont.IDENTITY_H, true, true);
        }

        public static byte[] fill_template(byte[] templateFile, List<FormElement> elements)
        {
            using (MemoryStream output = new MemoryStream())
            using (PdfReader pdfReader = new PdfReader(templateFile))
            using (PdfStamper stamper = new PdfStamper(pdfReader, output))
            {
                //without this line of code, farsi fonts would not be visible
                stamper.AcroFields.AddSubstitutionFont(GetFont().BaseFont);

                AcroFields form = stamper.AcroFields;
                form.GenerateAppearances = true;

                elements.Where(e => !string.IsNullOrEmpty(e.Name) && form.Fields.ContainsKey(e.Name))
                    .ToList().ForEach(e =>
                    {
                        switch (form.GetFieldType(e.Name))
                        {
                            case AcroFields.FIELD_TYPE_TEXT:
                            case AcroFields.FIELD_TYPE_COMBO:
                            case AcroFields.FIELD_TYPE_RADIOBUTTON:
                                form.SetField(e.Name, e.Type == FormElementTypes.Numeric ? e.FloatValue.ToString() : e.TextValue);
                                break;
                            case AcroFields.FIELD_TYPE_CHECKBOX:
                                form.SetField(e.Name, e.BitValue.HasValue && e.BitValue.Value ? "Yes" : "Off");
                                break;
                            case AcroFields.FIELD_TYPE_LIST:
                                form.SetListOption(e.Name,
                                    e.TextValue.Split('~').Select(t => t.Trim()).Where(t => !string.IsNullOrEmpty(t)).ToArray(), null);
                                form.SetField(e.Name, null);
                                break;
                            case AcroFields.FIELD_TYPE_PUSHBUTTON:
                                break;
                            case AcroFields.FIELD_TYPE_SIGNATURE:
                                break;
                        }
                    });

                //by doing this, the form will not be editable anymore
                stamper.FormFlattening = true;

                //to make a specific field not editable
                //stamper.PartialFormFlattening("[field_name]");

                stamper.Close();
                pdfReader.Close();

                return output.GetBuffer();
            }
        }

        public static byte[] append_footer(byte[] fileContent, string text, string password = null)
        {
            try
            {
                using (PdfReader reader = string.IsNullOrEmpty(password) ? new PdfReader(fileContent) :
                    new PdfReader(fileContent, ownerPassword: Encoding.UTF8.GetBytes(password)))
                using (MemoryStream output = new MemoryStream())
                using (PdfStamper stamper = new PdfStamper(reader, output))
                {
                    PdfContentByte pdfContent;
                    Rectangle pageSize;
                    BaseFont baseFont = GetBaseFont();

                    Enumerable.Range(1, reader.NumberOfPages).ToList().ForEach(pageNum =>
                    {
                        pdfContent = stamper.GetOverContent(pageNum);
                        pageSize = reader.GetPageSizeWithRotation(pageNum);

                        pdfContent.BeginText();

                        pdfContent.SetFontAndSize(baseFont, 8);
                        pdfContent.SetRGBColorFill(0, 0, 0);

                        pdfContent.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, text, pageSize.Width - 8, 8, 0);

                        pdfContent.EndText();
                    });

                    stamper.Close();

                    return output.GetBuffer();
                }
            }
            catch (Exception ex) {
                return null;
            }
        }
    }
}