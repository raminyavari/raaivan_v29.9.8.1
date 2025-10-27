using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.CoreNetwork;
using RaaiVan.Modules.RSS;

namespace RaaiVan.Web.API
{
    /// <summary>
    /// Summary description for RSSAPI
    /// </summary>
    public class RSSAPI : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        private ParamsContainer paramsContainer = null;

        public void ProcessRequest(HttpContext context)
        {
            paramsContainer = new ParamsContainer(HttpContext.Current, nullTenantResponse: true);
            if (!paramsContainer.ApplicationID.HasValue) return;

            string responseText = string.Empty;

            switch (PublicMethods.parse_string(context.Request.Params["Type"], false, "Node").ToLower())
            {
                case "node":
                    nodes_rss(PublicMethods.parse_guid(context.Request.Params["NodeTypeID"]),
                        HttpUtility.UrlDecode(PublicMethods.parse_string(context.Request.Params["Title"], false, string.Empty)),
                        HttpUtility.UrlDecode(PublicMethods.parse_string(context.Request.Params["Description"], false, string.Empty)),
                        PublicMethods.parse_int(context.Request.Params["Count"], 20),
                        PublicMethods.parse_long(context.Request.Params["LowerBoundary"]),
                        HttpUtility.UrlDecode(PublicMethods.parse_string(context.Request.Params["SearchText"], false, string.Empty)),
                        PublicMethods.parse_bool(context.Request.Params["IsDocument"]),
                        PublicMethods.parse_bool(context.Request.Params["IsKnowledge"]),
                        PublicMethods.parse_bool(context.Request.Params["Sitemap"]));
                    return;
                case "question":
                    questions_rss(HttpUtility.UrlDecode(PublicMethods.parse_string(context.Request.Params["Title"], false, string.Empty)),
                        HttpUtility.UrlDecode(PublicMethods.parse_string(context.Request.Params["Description"], false, string.Empty)),
                        PublicMethods.parse_int(context.Request.Params["Count"], 20));
                    return;
                case "external":
                    List<string> urlsArr = ListMaker.get_string_items(context.Request.Params["URLs"], '|').ToList();
                    List<KeyValuePair<string, string>> urls = new List<KeyValuePair<string, string>>();
                    foreach (string str in urlsArr)
                    {
                        int index = str.IndexOf(',');
                        urls.Add(new KeyValuePair<string, string>(str.Substring(index + 1), Base64.decode(str.Substring(0, index)).ToLower()));
                    }

                    external(urls,
                        PublicMethods.parse_int(context.Request.Params["Count"], 20),
                        PublicMethods.parse_guid(context.Request.Params["StoreAsNodeTypeID"]), ref responseText);
                    paramsContainer.return_response(ref responseText);
                    return;
                case "sitemapindex":
                    get_sitemap_index(PublicMethods.parse_int(context.Request.Params["Count"]));
                    return;
            }

            paramsContainer.return_response(PublicConsts.BadRequestResponse);
        }

        protected void nodes_rss(Guid? nodeTypeId, string title, string description, int? count, long? lowerBoundary,
            string searchText, bool? isDocument, bool? isKnowledge, bool? sitemap)
        {
            if (!count.HasValue || count.Value <= 0) count = 20;
            if (count.HasValue && count > 5000) count = 5000;

            List<Node> nodes = nodeTypeId.HasValue ?
                CNController.get_nodes(applicationId: paramsContainer.Tenant.Id, nodeTypeIds: new List<Guid>() { nodeTypeId.Value }, 
                relatedToIds: null, searchText: searchText, isDocument: isDocument, isKnowledge: isKnowledge, 
                count: count.Value, lowerBoundary: lowerBoundary) :
                CNController.get_nodes(applicationId: paramsContainer.Tenant.Id, searchText: searchText, 
                isDocument: isDocument, isKnowledge: isKnowledge, count: count.Value, lowerBoundary: lowerBoundary);

            if (sitemap.HasValue && sitemap.Value)
            {
                paramsContainer.xml_response(
                    "<?xml version=\"1.0\" encoding=\"UTF-8\" ?><urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">" +
                    ProviderUtil.list_to_string<string>(nodes.Select(
                        u => "<url>" +
                            "<loc>" +
                            PublicConsts.get_complete_url(paramsContainer.Tenant.Id, PublicConsts.NodePage) +
                                "/" + u.NodeID.Value.ToString() +
                            "</loc>" +
                            "<changefreq>weekly</changefreq>" +
                            "<lastmod>" + String.Format("{0:yyyy-MM-dd}", DateTime.Now) + "</lastmod>" +
                            //"<priority>0.8</priority>" +
                            "</url>").ToList(), null) +
                    "</urlset>"
                );

                return;
            }

            List<RSSItem> rssItems = new List<RSSItem>();

            foreach (Modules.CoreNetwork.Node _nd in nodes)
            {
                rssItems.Add(new RSSItem()
                {
                    Title = _nd.Name,
                    Link = PublicConsts.get_complete_url(paramsContainer.Tenant.Id, PublicConsts.NodePage) +
                        "/" + _nd.NodeID.Value.ToString()
                });
            }

            RSSUtilities.send_feed(HttpContext.Current, rssItems, title, description);
        }

        protected void questions_rss(string title, string description, int? count)
        {
            /*
            List<Question> lastQuestions = QAController.get_last_questions(paramsContainer.Tenant.Id, count);

            List<RSSItem> rssItems = new List<RSSItem>();

            foreach (Question _qu in lastQuestions)
            {
                rssItems.Add(new RSSItem()
                {
                    Title = _qu.Title,
                    Link = RaaiVanSettings.RaaiVanURL(paramsContainer.Tenant.Id) + 
                        "/Node/Pages/pgQuestionView.aspx?qid=" + _qu.QuestionID.Value.ToString()
                });
            }

            RSSUtilities.send_feed(HttpContext.Current, rssItems, title, description);
            */
        }

        private void _rss_to_nodes(object obj)
        {
            List<Modules.CoreNetwork.Node> nodes = (List<Modules.CoreNetwork.Node>)obj;

            foreach (Modules.CoreNetwork.Node nd in nodes)
                CNController.add_node(paramsContainer.Tenant.Id, nd);
        }

        private static Dictionary<string, KeyValuePair<DateTime, List<RSSItem>>> externalRssFeeds =
            new Dictionary<string, KeyValuePair<DateTime, List<RSSItem>>>();

        protected void external(List<KeyValuePair<string, string>> urls, int? count,
            Guid? storeAsNodeTypeId, ref string responseText)
        {
            if (!count.HasValue || count.Value <= 0) count = 20;
            if (count.HasValue && count.Value > 5000) count = 5000;

            List<RSSItem> items = new List<RSSItem>();

            DateTime now = DateTime.Now;
            TimeSpan fiveMins = now.AddMinutes(5) - now;

            int maxCount = count.Value * 2 / urls.Count;

            List<KeyValuePair<RSSItem, string>> hashLinks = new List<KeyValuePair<RSSItem, string>>();

            foreach (KeyValuePair<string, string> u in urls)
            {
                if (!externalRssFeeds.ContainsKey(u.Value) || !(now - externalRssFeeds[u.Value].Key <= fiveMins))
                {
                    externalRssFeeds[u.Value] =
                        new KeyValuePair<DateTime, List<RSSItem>>(now, RSSUtilities.get_feed(u.Value, u.Key));

                    if (storeAsNodeTypeId.HasValue)
                    {
                        hashLinks.AddRange(externalRssFeeds[u.Value].Value
                            .Select(x => new KeyValuePair<RSSItem, string>(x, PublicMethods.sha1(x.Link))));
                    }
                }

                items.AddRange(externalRssFeeds[u.Value].Value.OrderByDescending(v => v.PublicationDate).Take(maxCount));
            }

            items = items.OrderByDescending(u => u.PublicationDate).Take(count.Value).ToList();

            if (storeAsNodeTypeId.HasValue)
            {
                List<KeyValuePair<string, Guid>> guids = GlobalController.get_guids(paramsContainer.Tenant.Id,
                    hashLinks.Select(u => u.Value).ToList(), "RSS", false, true);

                List<KeyValuePair<Guid, RSSItem>> toBeStored =
                    guids.Select(u => new KeyValuePair<Guid, RSSItem>(u.Value, hashLinks.Where(v => v.Value == u.Key).Select(y => y.Key).FirstOrDefault())).ToList();

                List<Node> nodes = toBeStored.Select(
                    u => new Modules.CoreNetwork.Node()
                    {
                        NodeID = u.Key,
                        NodeTypeID = storeAsNodeTypeId.Value,
                        Name = u.Value.Title,
                        CreationDate = u.Value.PublicationDate.HasValue ? u.Value.PublicationDate : DateTime.Now,
                        Description = (!u.Value.PublicationDate.HasValue ? string.Empty :
                            "<div style='text-align:center; margin-bottom:20px;'>" + GenericDate.get_local_date(u.Value.PublicationDate.Value, true) + "</div>") +
                            (string.IsNullOrEmpty(u.Value.Description) ? u.Value.Summary : u.Value.Description) +
                            (string.IsNullOrEmpty(u.Value.Link) ? string.Empty : "<div style='margin-top:12px;'><span style='margin:0px 4px 0px 4px;'>@[[Dic:Dic:" + Base64.encode("Source") + "]]:</span><a href='" + u.Value.Link + "'>" + u.Value.Link + "</a></div>")
                    }).ToList();

                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(_rss_to_nodes), nodes);
            }

            responseText = "{\"Items\":[" + ProviderUtil.list_to_string<string>(items.Select(
                u => "{\"ID\":\"" + u.ID + "\"" +
                    ",\"Title\":\"" + Base64.encode(u.Title) + "\"" +
                    ",\"Summary\":\"" + (string.IsNullOrEmpty(u.Summary) ? string.Empty :
                        Base64.encode(u.Summary.Substring(0, Math.Min(u.Summary.Length, 300)))) + "\"" +
                    ",\"Description\":\"" + (string.IsNullOrEmpty(u.Description) ? string.Empty :
                        Base64.encode(u.Description.Substring(0, Math.Min(u.Description.Length, 300)))) + "\"" +
                    ",\"Link\":\"" + Base64.encode(u.Link) + "\"" +
                    ",\"PublicationDate\":\"" + (u.PublicationDate.HasValue ?
                        GenericDate.get_local_date(u.PublicationDate.Value, true) : string.Empty) + "\"" +
                    "}").ToList()) +
                "]}";
        }

        protected void get_sitemap_index(int? count)
        {
            if (!count.HasValue) count = 1000;

            long nodesCount = CNController.get_nodes_count(paramsContainer.Tenant.Id)
                .Sum(u => u.Count.HasValue ? u.Count.Value : 0);

            double pagesCount = Math.Ceiling((double)nodesCount / (double)count);

            string response = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" +
                "<sitemapindex xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">";

            for (int i = 0; i < pagesCount; ++i)
            {
                response += "<sitemap>" +
                "<loc>" +
                    PublicConsts.get_complete_url(paramsContainer.Tenant.Id, PublicConsts.RSSPage) + "/node?" +
                    ("Sitemap=true&LowerBoundary=" + ((count * i) + 1).ToString() + "&Count=" + count.ToString()).Replace("&", "&amp;") +
                "</loc>" +
                "<lastmod>" + String.Format("{0:yyyy-MM-dd}", DateTime.Now) + "</lastmod>" +
                "</sitemap>";
            }

            response += "</sitemapindex>";

            paramsContainer.xml_response(response);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}