using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Web;
using HtmlAgilityPack;
using iTextSharp.text;
using iTextSharp.text.pdf;
using itsXml = iTextSharp.tool.xml;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.css;
using itsXmlHtml = iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.pipeline.html;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.Documents;
using RaaiVan.Modules.FormGenerator;

namespace RaaiVan.Web.API
{
    public class WikiPDFText
    {
        private string _RawText;
        private string _Text;
        private bool? _IsRTL;
        private bool? _IsProbablyRTL;
        private bool? _ForceRTL;

        private string RawText
        {
            get
            {
                if (_RawText == null)
                    _RawText = Expressions.replace(Text, Expressions.Patterns.HTMLTag);
                return _RawText;
            }
        }

        public string Text
        {
            get { return _Text; }
        }

        public bool IsRTL
        {
            get
            {
                if (!_IsRTL.HasValue) _IsRTL = PublicMethods.has_rtl_characters(RawText);
                return _IsRTL.Value;
            }
        }

        public bool IsProbablyRTL
        {
            get
            {
                if (!_IsProbablyRTL.HasValue) _IsProbablyRTL =
                        (double)PublicMethods.rtl_characters_count(RawText) / (double)RawText.Length >= 0.1;
                return _IsProbablyRTL.Value;
            }
        }

        public bool ForceRTL
        {
            get
            {
                return _ForceRTL.HasValue && _ForceRTL.Value;
            }
        }

        public WikiPDFText(Guid applicationId, string text)
        {
            if (string.IsNullOrEmpty(text)) text = string.Empty;

            _Text = text.Replace("&amp;nbsp;", " ").Replace("&amp;zwnj;", " ")
                    .Replace("&amp;laquo;", "«").Replace("&amp;raquo;", "»").Replace("<br>", "<br />").Trim();
            _Text = PublicMethods.markup2plaintext(applicationId, _Text, true);

            if (_Text.Length > 0 && _Text[0] != '<') _Text = "<span>" + _Text + "</span>";

            if (!(new System.Text.RegularExpressions.Regex(@"<.*?>")).IsMatch(_Text))
                _Text = _Text.Replace("\n", "<br />");
        }

        public void set_is_rtl(bool value)
        {
            _IsRTL = _ForceRTL = value;
        }
    }

    public class Wiki2PDF
    {
        public static byte[] export_as_pdf(Guid applicationId, Guid? currentUserId, bool isUser, DownloadedFileMeta meta,
            string title, string description, List<string> keywords, List<KeyValuePair<Guid, string>> wikiTitles,
            Dictionary<Guid, List<KeyValuePair<string, string>>> wikiParagraphs, Dictionary<string, string> metaData, 
            List<string> authors, Guid? coverId, Guid? coverOwnerNodeId, string password, HttpContext context)
        {
            bool hasCustomCover = coverId.HasValue && coverOwnerNodeId.HasValue && currentUserId.HasValue;

            List<WikiPDFText> wikiCover = hasCustomCover ? new List<WikiPDFText>() :
                get_default_cover(applicationId, isUser, title, metaData, authors);

            string strKeywords = string.Join(" - ", keywords);

            WikiPDFText wikiTitle = new WikiPDFText(applicationId,
                @"<br><br><p style='text-align:center;'><strong><span style='color:DarkSlateGray;'>" + title + "</span></strong></p><br><br>");
            List<WikiPDFText> wikiDesc = convert_div_to_p(applicationId, new List<WikiPDFText>() {
                new WikiPDFText(applicationId,
                    string.IsNullOrEmpty(description) ? "" : @"<br><p class='NodeAbs'>" + description + "</p><br>")
            });
            WikiPDFText wikiKeywords = new WikiPDFText(applicationId,
                string.IsNullOrEmpty(strKeywords) ? "" : @"<br><p> " + strKeywords + " </p><br>");

            string pdfTitles = string.Empty;
            WikiPDFText wikiPDFTitles = null;

            List<WikiPDFText> strHTMLParagraphs = new List<WikiPDFText>();

            foreach (KeyValuePair<Guid, string> t in wikiTitles)
            {
                if (!wikiParagraphs.ContainsKey(t.Key) || wikiParagraphs[t.Key].Count == 0) continue;

                bool hasTitle = !string.IsNullOrEmpty(t.Value);

                int counter = 0;

                if (hasTitle)
                {
                    pdfTitles += "<p><a href='#" + t.Key.ToString() + "'>" + t.Value + "</a></p><br>";
                    pdfTitles += ProviderUtil.list_to_string<string>(wikiParagraphs[t.Key].Where(
                        u => !string.IsNullOrEmpty(u.Key) && !string.IsNullOrEmpty(u.Value)).Select(
                        p => "<p><a style='color:gray;' href='#" + t.Key.ToString() + (++counter).ToString() + "'>" + p.Key + "</a></p><br>").ToList<string>(), ' ');
                }

                wikiPDFTitles = new WikiPDFText(applicationId, pdfTitles);

                counter = 0;

                WikiPDFText mt = !hasTitle ? null : new WikiPDFText(applicationId,
                    @"<br><a name='" + t.Key.ToString() + "'><strong style='color:#00a;'>" + t.Value + "</strong></a>");

                List<KeyValuePair<string, string>> prgrs = wikiParagraphs[t.Key].Where(
                    u => !string.IsNullOrEmpty(u.Value)).Select(x => x).ToList();

                WikiPDFText tempParagraphs = new WikiPDFText(applicationId,
                    ProviderUtil.list_to_string<string>(prgrs.Select(u => u.Value).ToList(), ' '));
                if (mt != null && tempParagraphs.IsProbablyRTL) mt.set_is_rtl(true);

                if (mt != null) strHTMLParagraphs.Add(mt);

                foreach (KeyValuePair<string, string> kvp in prgrs)
                {
                    WikiPDFText pp = new WikiPDFText(applicationId, kvp.Value);

                    if (!string.IsNullOrEmpty(kvp.Key))
                    {
                        WikiPDFText pt = new WikiPDFText(applicationId, "<p><a style='color:#444;' name='" +
                            t.Key.ToString() + (++counter).ToString() + "'><strong>" + kvp.Key + "</strong></a></p>");

                        if (pp.IsProbablyRTL) pt.set_is_rtl(true);

                        strHTMLParagraphs.Add(pt);
                    }

                    strHTMLParagraphs.Add(pp);
                }
            }

            strHTMLParagraphs = convert_div_to_p(applicationId, strHTMLParagraphs);

            using (MemoryStream mem = new MemoryStream())
            {
                using (Document pdfDoc = new Document(PageSize.A4))
                {
                    BaseFont baseFont = BaseFont.CreateFont(
                        PublicMethods.map_path("~/Fonts/windows/IRANSans.ttf"), BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    Font font = new Font(baseFont, size: 9);

                    PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, mem);

                    PageEventHelper pageEvents = new PageEventHelper(applicationId, title, font, meta);
                    pdfWriter.PageEvent = pageEvents;

                    pdfWriter.RgbTransparencyBlending = true;
                    pdfDoc.Open();

                    StyleAttrCSSResolver cssResolver = new StyleAttrCSSResolver();
                    cssResolver.AddCssFile(PublicMethods.map_path("~/CSS/pdf.css"), true);

                    PdfPTable mainTable = new PdfPTable(1)
                    {
                        WidthPercentage = 100,
                        RunDirection = PdfWriter.RUN_DIRECTION_RTL
                    };

                    mainTable.SplitLate = false;

                    PdfPCell coverCell = new PdfPCell()
                    {
                        FixedHeight = 750,
                        RunDirection = PdfWriter.RUN_DIRECTION_RTL,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    };

                    PdfPCell titlesCell = new PdfPCell
                    {
                        Border = 0,
                        RunDirection = PdfWriter.RUN_DIRECTION_RTL,
                        HorizontalAlignment = Element.ALIGN_RIGHT,
                        PaddingRight = 20f,
                        PaddingLeft = 20f,
                        BackgroundColor = BaseColor.LIGHT_GRAY
                    };

                    for (int c = 0; c < wikiCover.Count; c++)
                        _add_to_table(applicationId, context, mainTable, cssResolver, wikiCover[c], font, coverCell);
                    //_add_to_table(applicationId, context, mainTable, cssResolver, wikiTitle, font);
                    if (!string.IsNullOrEmpty(description))
                    {
                        for (int c = 0; c < wikiDesc.Count; c++)
                            _add_to_table(applicationId, context, mainTable, cssResolver, wikiDesc[c], font);
                    }
                    if (!string.IsNullOrEmpty(strKeywords))
                        _add_to_table(applicationId, context, mainTable, cssResolver, wikiKeywords, font);
                    //_add_to_table(applicationId, context, mainTable, cssResolver, wikiPDFTitles, font, titlesCell);
                    for (int c = 0; c < strHTMLParagraphs.Count; c++)
                        _add_to_table(applicationId, context, mainTable, cssResolver, strHTMLParagraphs[c], font);

                    pdfDoc.Add(mainTable);

                    pdfDoc.Close();

                    byte[] finalContent = mem.ToArray();

                    if (hasCustomCover) finalContent = add_custom_cover(applicationId, 
                        currentUserId.Value, finalContent, coverId.Value, coverOwnerNodeId.Value);

                    return string.IsNullOrEmpty(password) ? finalContent : PDFUtil.set_password(finalContent, password);
                }
            }
        }

        private static List<WikiPDFText> get_default_cover(Guid applicationId, 
            bool isUser, string title, Dictionary<string, string> metaData, List<string> authors)
        {
            if (metaData == null) metaData = new Dictionary<string, string>();
            if (authors == null) authors = new List<string>();

            string coverTemplate = string.Empty;

            if (!isUser)
            {
                coverTemplate = "<div style=\"text-align: center;\">" +
                    "<img src='~[[Logo]]' style='max-width:500px; max-height:400px;' /><br><br>عنوان:<br>" +
                    "<br><strong>~[[Title]]</strong><br><br>طبقه بندی: <strong>~[[Confidentiality]]" +
                    "</strong><br><br>نویسنده/نویسندگان:<br><br>~[[Authors]]<br>" +
                    "<br>تاریخ انتشار: ~[[PublicationDate]]<br>تاریخ آخرین تغییرات: ~[[LastModificationDate]]<br>" +
                    "<br><br>تهیه شده در: <strong>~[[RegistrationArea]]</strong><br></div>";
            }
            else
            {
                coverTemplate = "<div style=\"text-align: center;\">" +
                    "<img src='~[[Logo]]' style='max-width:500px; max-height:400px;' /><br><br>عنوان:<br>" +
                    "<br><strong>~[[Title]]</strong></div>";
            }

            metaData["Logo"] = RaaiVanSettings.Organization.Logo(applicationId) == null ? string.Empty :
                "data:image/png;base64," + PublicMethods.image_to_base64(
                RaaiVanSettings.Organization.Logo(applicationId), System.Drawing.Imaging.ImageFormat.Png);
            metaData["Title"] = title;
            if (metaData.ContainsKey("RegistrationArea") && !string.IsNullOrEmpty(metaData["RegistrationArea"]))
            {
                metaData["RegistrationArea"] = metaData["RegistrationArea"] +
                    " (" + metaData["RegistrationAreaType"] + ")";
            }
            else
                metaData["RegistrationArea"] = "__";

            string strAutors = string.Empty;
            for (int i = 0; i < authors.Count; ++i)
            {
                strAutors += authors.Count > 10 ? authors[i] + (i == authors.Count - 1 ? string.Empty : ", ") :
                    "<strong>" + authors[i] + "</strong><br>";
            }

            metaData["Authors"] = string.IsNullOrEmpty(strAutors) ? "__" : strAutors;

            coverTemplate = Expressions.replace(coverTemplate, ref metaData, Expressions.Patterns.AutoTag);

            return convert_div_to_p(applicationId, new List<WikiPDFText>() { new WikiPDFText(applicationId, coverTemplate) });
        }

        public static byte[] add_custom_cover(Guid applicationId, Guid currentUserId, byte[] pdfContent, Guid coverId, Guid ownerNodeId)
        {
            DocFileInfo pdfCover = null;

            pdfCover = DocumentsController.get_file(applicationId, coverId);
            if (pdfCover == null || string.IsNullOrEmpty(pdfCover.Extension) || pdfCover.Extension.ToLower() != "pdf") pdfCover = null;

            byte[] coverContent = pdfContent == null ? null : pdfCover.toByteArray();

            if (coverContent == null || coverContent.Length == 0) return pdfContent;

            Dictionary<string, string> dic = CNAPI.get_replacement_dictionary(applicationId, currentUserId, ownerNodeId, true);

            List<FormElement> tempElems = dic.Keys.ToList().Select(key => new FormElement()
            {
                Name = key,
                Type = FormElementTypes.Text,
                TextValue = dic[key]
            }).ToList();

            byte[] cover = PDFTemplates.fill_template(coverContent, tempElems);

            return PDFUtil.merge_documents(new List<object>() { cover, pdfContent });
        }

        private static List<WikiPDFText> convert_div_to_p(Guid applicationId, List<WikiPDFText> strHTMLParagraphs)
        {
            List<WikiPDFText> xmlParagraphs = new List<WikiPDFText>();

            //Tabdil hame tag haye div be tag p
            //ITextSharp Tag div nemishnase
            foreach (WikiPDFText s in strHTMLParagraphs)
            {
                bool forceRTL = s.ForceRTL;

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(("<p>" + s.Text + "</p>").ToXHtml());

                XmlNodeList xnl = doc.GetElementsByTagName("div");

                while (xnl.Count != 0)
                {
                    XmlNode oldElem = xnl[0];
                    XmlElement newElem = doc.CreateElement("p");

                    while (oldElem.ChildNodes.Count != 0) newElem.AppendChild(oldElem.ChildNodes[0]);
                    while (oldElem.Attributes.Count != 0) newElem.Attributes.Append(oldElem.Attributes[0]);

                    oldElem.ParentNode.ReplaceChild(newElem, oldElem);
                }

                string toBeAdded = string.Empty;

                XmlNode firstChild = doc.LastChild.FirstChild;

                while (firstChild != null)
                {
                    if (firstChild.Name.ToLower() == "p")
                    {
                        if (!string.IsNullOrEmpty(toBeAdded))
                        {
                            WikiPDFText t = new WikiPDFText(applicationId, toBeAdded);
                            if (forceRTL) t.set_is_rtl(true);
                            xmlParagraphs.Add(t);
                        }

                        WikiPDFText p = new WikiPDFText(applicationId, firstChild.OuterXml.ToString());

                        if (forceRTL || p.Text.StartsWith("<p dir=\"RTL\"")) p.set_is_rtl(true);

                        bool hasNothing = !string.IsNullOrEmpty(p.Text) && p.Text.Trim().ToLower().StartsWith("<p>")
                            && string.IsNullOrEmpty(firstChild.InnerXml.ToLower());

                        if (!hasNothing)
                        {
                            xmlParagraphs.Add(p);
                            toBeAdded = string.Empty;
                        }
                    }
                    else
                        toBeAdded += firstChild.OuterXml.ToString();

                    firstChild = firstChild.NextSibling;

                    if (firstChild == null && !string.IsNullOrEmpty(toBeAdded))
                    {
                        WikiPDFText t = new WikiPDFText(applicationId, toBeAdded);
                        if (forceRTL) t.set_is_rtl(true);
                        xmlParagraphs.Add(t);
                    }
                }
            }

            return xmlParagraphs;
        }

        private static void _add_to_table(Guid applicationId, HttpContext context, PdfPTable mainTable,
            StyleAttrCSSResolver cssResolver, WikiPDFText text, Font font, PdfPCell cell = null)
        {
            if (cell == null) cell = new PdfPCell();

            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
            cell.RunDirection = text.IsRTL ? PdfWriter.RUN_DIRECTION_RTL : PdfWriter.RUN_DIRECTION_LTR;
            cell.SetLeading(0, 2);

            itsXmlHtml.DefaultTagProcessorFactory tagProcessors =
                (itsXmlHtml.DefaultTagProcessorFactory)itsXmlHtml.Tags.GetHtmlTagProcessorFactory();

            tagProcessors.RemoveProcessor(itsXmlHtml.HTML.Tag.IMG); // remove the default processor
            tagProcessors.AddProcessor(itsXmlHtml.HTML.Tag.IMG, new CustomImageTagProcessor(applicationId)); // use our new processor

            //tagProcessors.AddProcessor(itsXmlHtml.HTML.Tag.TABLE, new TableTagProcessor());

            HtmlPipelineContext htmlContext =
                new HtmlPipelineContext(new itsXmlHtml.CssAppliersImpl(new IranSansFontProvider(font)));
            htmlContext.SetImageProvider(new ImageProvider());
            htmlContext.CharSet(Encoding.UTF8);
            htmlContext.SetAcceptUnknown(true).AutoBookmark(true).SetTagFactory(tagProcessors);

            ElementsCollector elementsHandler = new ElementsCollector();
            CssResolverPipeline pipeline = new CssResolverPipeline(cssResolver,
                new HtmlPipeline(htmlContext, new ElementHandlerPipeline(elementsHandler, null)));

            itsXml.XMLWorker worker = new itsXml.XMLWorker(pipeline, parseHtml: true);
            XMLParser parser = new XMLParser(true, worker, Encoding.UTF8);

            parser.Parse(new StringReader(text.Text));

            cell.AddElement(elementsHandler.Paragraph);
            mainTable.AddCell(cell);
        }
    }

    public static class XHtmlUtils
    {
        public static string ToXHtml(this string htmlContent)
        {
            StringBuilder sb = new StringBuilder();
            using (StringWriter stringWriter = new StringWriter(sb))
            {
                HtmlDocument doc = new HtmlDocument
                {
                    OptionOutputAsXml = true,
                    OptionCheckSyntax = true,
                    OptionFixNestedTags = true,
                    OptionAutoCloseOnEnd = true,
                    OptionDefaultStreamEncoding = Encoding.UTF8
                };

                doc.LoadHtml(htmlContent);
                doc.Save(stringWriter);
                return sb.ToString();
            }
        }
    }

    public class ElementsCollector : IElementHandler
    {
        private readonly Paragraph _paragraph;

        public ElementsCollector()
        {
            _paragraph = new Paragraph { Alignment = Element.ALIGN_JUSTIFIED };
            _paragraph.SetLeading(0, 2);
            _paragraph.SpacingAfter = (float)0.5;
        }

        public Paragraph Paragraph
        {
            get { return _paragraph; }
        }

        public void Add(IWritable htmlElement)
        {
            WritableElement writableElement = htmlElement as WritableElement;
            if (writableElement == null) return;

            foreach (IElement element in writableElement.Elements())
            {
                if (element is PdfDiv)
                {
                    PdfDiv div = element as PdfDiv;
                    foreach (IElement divChildElement in div.Content)
                        _paragraph.Add(divChildElement);
                }
                else
                    _paragraph.Add(element);
            }
        }
    }

    public class IranSansFontProvider : IFontProvider
    {
        private float Size;

        public bool IsRegistered(string fontname)
        {
            return false;
        }

        public IranSansFontProvider()
        {
            Size = 9;
        }

        public IranSansFontProvider(Font font)
        {
            Size = font.Size;
        }

        public Font GetFont(string fontname,
            string encoding, bool embedded, float size, int style, BaseColor color)
        {
            BaseFont customFont = BaseFont.CreateFont(
                    PublicMethods.map_path("~/Fonts/windows/IRANSans.ttf"), BaseFont.IDENTITY_H, true);
            Font font = new Font(customFont, Size, style, color);
            return font;
        }
    }

    public class UnicodeFontProvider : FontFactoryImp
    {
        public UnicodeFontProvider()
        {
            FontFactory.Register(PublicMethods.map_path("~/Fonts/tahoma.ttf"));
        }

        public override Font GetFont(string fontname, string encoding, bool embedded, float size, int style,
            BaseColor color, bool cached)
        {
            if (string.IsNullOrWhiteSpace(fontname)) fontname = "tahoma";
            return FontFactory.GetFont(fontname, BaseFont.IDENTITY_H, BaseFont.EMBEDDED, size, style, color);
        }
    }

    public class CustomImageTagProcessor : iTextSharp.tool.xml.html.Image
    {
        private static Guid ApplicationID;

        public CustomImageTagProcessor(Guid applicationId)
        {
            ApplicationID = applicationId;
        }

        public override IList<IElement> End(IWorkerContext ctx, Tag tag, IList<IElement> currentContent)
        {
            IDictionary<string, string> attributes = tag.Attributes;
            string src;
            if (!attributes.TryGetValue(itsXmlHtml.HTML.Attribute.SRC, out src))
                return new List<IElement>(1);

            if (string.IsNullOrEmpty(src))
                return new List<IElement>(1);

            //convert url to base64 image
            if (!src.StartsWith("data:image/", StringComparison.InvariantCultureIgnoreCase))
            {
                int index = src.ToLower().IndexOf("fileid=");

                if (index >= 0)
                {
                    string newSrc = src.Substring(index + "fileid=".Length);
                    if (newSrc.IndexOf("&") > 0) newSrc = newSrc.Substring(0, newSrc.IndexOf("&"));

                    Guid? fileId = PublicMethods.parse_guid(newSrc);
                    DocFileInfo fi = DocumentsController.get_file(ApplicationID, fileId.Value);
                    if (fi != null) fi.refresh_folder_name();
                    
                    if (!fi.exists())
                    {
                        try
                        {
                            System.Drawing.Image img = null;

                            using (MemoryStream stream = new MemoryStream(fi.toByteArray()))
                                img = System.Drawing.Bitmap.FromStream(stream);

                            string strWidth = tag == null || tag.CSS == null || !tag.CSS.ContainsKey("width") ?
                                string.Empty : tag.CSS["width"];
                            string strHeight = tag == null || tag.CSS == null || !tag.CSS.ContainsKey("height") ?
                                string.Empty : tag.CSS["height"];

                            if (!string.IsNullOrEmpty(strWidth)) strWidth = strWidth.ToLower().Replace("px", "");
                            if (!string.IsNullOrEmpty(strHeight)) strHeight = strHeight.ToLower().Replace("px", "");

                            int width = 0, height = 0, maxWidth = 650, maxHeight = 900;

                            if (string.IsNullOrEmpty(strWidth) ||
                                !int.TryParse(strWidth, out width) || width < 0) width = img.Width;
                            if (string.IsNullOrEmpty(strHeight) ||
                                !int.TryParse(strHeight, out height) || height < 0) height = img.Height;

                            double coeff = Math.Min(width <= maxWidth ? 1 : (double)maxWidth / (double)width,
                                height <= maxHeight ? 1 : (double)maxHeight / (double)height);

                            width = (int)Math.Floor(coeff * (double)width);
                            height = (int)Math.Floor(coeff * (double)height);

                            if (width != img.Width || height != img.Height)
                            {
                                string msg = string.Empty;
                                IconMeta meta = null;

                                if (RVGraphics.make_thumbnail(img, width, height, 0, 0, ref meta, ref img, ref msg))
                                {
                                    tag.CSS["width"] = (width = img.Width).ToString() + "px";
                                    tag.CSS["height"] = (height = img.Height).ToString() + "px";
                                }
                            }

                            newSrc = PublicMethods.image_to_base64(img, System.Drawing.Imaging.ImageFormat.Png);

                            if (!string.IsNullOrEmpty(newSrc)) src = "data:image/png;base64," + newSrc;
                        }
                        catch { }
                    }
                }
            }
            //end of convert url to base64 image

            if (src.StartsWith("data:image/", StringComparison.InvariantCultureIgnoreCase))
            {
                // data:[<MIME-type>][;charset=<encoding>][;base64],<data>
                var base64Data = src.Substring(src.IndexOf(",") + 1);
                var imagedata = Convert.FromBase64String(base64Data);
                var image = iTextSharp.text.Image.GetInstance(imagedata);

                image.ScaleToFitLineWhenOverflow = true;
                image.ScaleToFitHeight = false;
                var list = new List<IElement>();
                var htmlPipelineContext = GetHtmlPipelineContext(ctx);
                IElement imgElement = GetCssAppliers().Apply(new Chunk((iTextSharp.text.Image)GetCssAppliers()
                    .Apply(image, tag, htmlPipelineContext), 0, 0, true), tag, htmlPipelineContext);

                list.Add(imgElement);
                return list;
            }
            else
            {
                return base.End(ctx, tag, currentContent);
            }
        }
    }

    public class ImageProvider : AbstractImageProvider
    {
        public override string GetImageRootPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\";
        }
    }

    public class TableTagProcessor : iTextSharp.tool.xml.html.table.Table
    {
        public override IList<IElement> End(IWorkerContext ctx, Tag tag, IList<IElement> currentContent)
        {
            //return new List<IElement>(); //to be removed

            //See if we've got anything to work with
            if (currentContent.Count > 0)
            {
                tag.CSS["direction"] = "rtl";
                tag.CSS["text-align"] = "right";

                //If so, let our parent class worry about it
                try
                {
                    return base.End(ctx, tag, currentContent);
                }
                catch (Exception ex)
                {
                }
            }

            //Otherwise return an empty list which should make everyone happy
            return new List<IElement>();
        }
    }

    public class PageEventHelper : PdfPageEventHelper
    {
        public Guid ApplicationID;
        private string DocumentTitle;
        private Font font;
        private PdfContentByte _pdfContentByte;
        private PdfTemplate _template;
        private DownloadedFileMeta Meta;

        public PageEventHelper(Guid applicationId, string documentTitle, Font font, DownloadedFileMeta meta)
        {
            this.ApplicationID = applicationId;
            this.DocumentTitle = documentTitle;
            this.font = new Font(font.BaseFont, 10, Font.NORMAL, BaseColor.GRAY);
            this.Meta = meta;
        }

        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            _pdfContentByte = writer.DirectContent;
            _template = _pdfContentByte.CreateTemplate(50, 50);
        }

        public override void OnStartPage(PdfWriter writer, Document document)
        {
            base.OnStartPage(writer, document);

            try
            {
                //watermark
                /*
                float fontSize = 80, xPosition = 300, yPosition = 400, angle = 315;
                PdfContentByte under = writer.DirectContentUnder;
                BaseFont baseFont = font.BaseFont;
                under.BeginText();
                under.SetColorFill(BaseColor.LIGHT_GRAY);
                under.SetFontAndSize(baseFont, fontSize);
                under.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "Ramin Yavari", xPosition, yPosition, angle);
                under.EndText();
                */
                //end of watermark
            }
            catch (Exception ex) { }
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);

            //page number
            if (writer.PageNumber > 1)
            {
                String text = PublicMethods.convert_numbers_to_local((writer.PageNumber - 1) + "/");
                float len = font.BaseFont.GetWidthPoint(text, 8);
                Rectangle pageSize = document.PageSize;
                _pdfContentByte.SetRGBColorFill(130, 130, 130);

                _pdfContentByte.BeginText();
                _pdfContentByte.SetFontAndSize(font.BaseFont, 8);
                _pdfContentByte.SetTextMatrix(pageSize.GetLeft(pageSize.Width / 2), pageSize.GetBottom(20));
                _pdfContentByte.ShowText(text);
                _pdfContentByte.EndText();

                _pdfContentByte.AddTemplate(_template, pageSize.GetLeft(pageSize.Width / 2) + len, pageSize.GetBottom(20));
            }
            //end of page number

            //header
            if (writer.PageNumber > 1)
            {
                string hOddMessage = RaaiVanSettings.Organization.Name(ApplicationID);
                string hEvenMessage = DocumentTitle;

                if (string.IsNullOrEmpty(hOddMessage)) hOddMessage = hEvenMessage;

                bool isOddPage = writer.PageNumber % 2 == 1;

                string headerMessage = isOddPage ? hOddMessage : hEvenMessage;

                bool isRTL = PublicMethods.has_rtl_characters(headerMessage);

                PdfPTable htbl = new PdfPTable(1);
                htbl.TotalWidth = 519;
                htbl.WidthPercentage = 100;
                htbl.HorizontalAlignment = isRTL ? Element.ALIGN_RIGHT : Element.ALIGN_LEFT;
                if (isRTL) htbl.RunDirection = PdfWriter.RUN_DIRECTION_RTL;

                Paragraph header = new Paragraph(headerMessage,
                    new Font(font.BaseFont, 8, Font.NORMAL, BaseColor.BLACK));

                PdfPCell hcell = new PdfPCell(header);
                hcell.Border = 0;
                hcell.PaddingBottom = 6;

                PdfPCell lcell = new PdfPCell();
                lcell.PaddingTop = 1;
                lcell.BackgroundColor = BaseColor.BLACK;

                htbl.AddCell(hcell);
                htbl.AddCell(lcell);

                htbl.WriteSelectedRows(0, -1, 38, 830, writer.DirectContent);
            }
            //end of header

            //footer
            if (writer.PageNumber > 1 && Meta != null)
            {
                PdfPTable htbl = new PdfPTable(1)
                {
                    TotalWidth = 519,
                    WidthPercentage = 100,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    RunDirection = PdfWriter.RUN_DIRECTION_RTL
                };

                Paragraph footer = new Paragraph(Meta.toString(), new Font(font.BaseFont, 7, Font.NORMAL, BaseColor.DARK_GRAY));
                PdfPCell hcell = new PdfPCell(footer) { Border = 0, PaddingBottom = 0 };

                htbl.AddCell(hcell);

                htbl.WriteSelectedRows(0, -1, 38, 15, writer.DirectContent);
            }
            //end of footer
        }

        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);

            _template.BeginText();
            _template.SetFontAndSize(font.BaseFont, 8);
            _template.SetTextMatrix(0, 0);
            _template.ShowText(PublicMethods.convert_numbers_to_local((writer.PageNumber - 1).ToString()));
            _template.EndText();
        }
    }
}