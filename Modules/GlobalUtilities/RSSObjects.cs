using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.ServiceModel.Syndication;
using System.IO;
using System.Web;
using System.Net;

namespace RaaiVan.Modules.RSS
{
    public static class RSSUtilities
    {
        public static string generate_feed(List<RSSItem> rssItems, string title, string description)
        {
            try
            {
                List<SyndicationItem> items = new List<SyndicationItem>();

                foreach (RSSItem _itm in rssItems)
                {
                    SyndicationItem item = new SyndicationItem();

                    if (!string.IsNullOrEmpty(_itm.Title)) item.Title = new TextSyndicationContent(_itm.Title);
                    if (!string.IsNullOrEmpty(_itm.Description)) item.Content = new TextSyndicationContent(_itm.Description);

                    if (!string.IsNullOrEmpty(_itm.Link))
                    {
                        XmlElement lnk = (new XmlDocument()).CreateElement("link");
                        lnk.SetAttribute("rel", "replies");
                        lnk.SetAttribute("type", "text/html");
                        lnk.SetAttribute("href", _itm.Link);
                        lnk.InnerText = _itm.Link;

                        item.ElementExtensions.Add(lnk);
                    }

                    items.Add(item);
                }

                SyndicationFeed feed = new SyndicationFeed(items);

                feed.Title = new TextSyndicationContent(title);
                feed.Description = new TextSyndicationContent(description);

                StringBuilder output = new StringBuilder();

                XmlWriter rssWriter = XmlWriter.Create(output);
                Rss20FeedFormatter rssFormatter = new Rss20FeedFormatter(feed);
                rssFormatter.WriteTo(rssWriter);
                rssWriter.Close();

                return output.ToString();
            }
            catch { return string.Empty; }
        }

        public static void send_feed(HttpContext context, List<RSSItem> rssItems, string title, string description)
        {
            string xmlData = generate_feed(rssItems, title, description);

            context.Response.Clear();
            context.Response.ClearContent();
            context.Response.ClearHeaders();
            context.Response.Buffer = true;
            context.Response.ContentType = "text/xml";
            context.Response.ContentEncoding = Encoding.UTF8;
            //Response.AddHeader("Content-Disposition", "RSS; filename=RSS.xml");

            context.Response.Write(xmlData);

            context.Response.Flush();
            context.Response.End();
        }

        public static List<RSSItem> get_feed(string url, string id = "")
        {
            List<RSSItem> lst = new List<RSSItem>();

            try
            {
                XmlReaderSettings settings = new XmlReaderSettings() { DtdProcessing = DtdProcessing.Parse };
                XmlReader r = XmlReader.Create(url, settings);
                //XmlReader r = XmlReader.Create(new StringReader(new WebClient().DownloadString(url)), settings);

                SyndicationFeed albums = SyndicationFeed.Load(r);
                
                r.Close();

                foreach (SyndicationItem album in albums.Items)
                {
                    string _url = album.Links[0].Uri.ToString();
                    string title = album.Title.Text;
                    string summary = album.Summary != null ? album.Summary.Text : string.Empty;
                    string content = album.Content != null ? (album.Content as TextSyndicationContent).Text : string.Empty;
                    DateTime pubDate = album.PublishDate.DateTime;

                    RSSItem newItem = new RSSItem()
                    {
                        ID = id,
                        Title = album.Title.Text,
                        Summary = album.Summary != null ? album.Summary.Text : string.Empty,
                        Description = album.Content != null ? (album.Content as TextSyndicationContent).Text : string.Empty,
                        Link = album.Links[0].Uri.ToString(),
                        PublicationDate = album.PublishDate.DateTime
                    };

                    lst.Add(newItem);
                }
            }
            catch (Exception ex) { string strEx = ex.ToString(); }

            return lst;
        }
    }

    public class RSSItem
    {
        public string ID;
        public string Title;
        public string Summary;
        public string Description;
        public string Link;
        public string URL;
        public DateTime? PublicationDate;
    }
}
