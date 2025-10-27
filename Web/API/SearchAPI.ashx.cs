using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.Search;
using RaaiVan.Modules.Log;
using RaaiVan.Modules.CoreNetwork;

namespace RaaiVan.Web.API
{
    /// <summary>
    /// Summary description for SearchAPI
    /// </summary>
    public class SearchAPI : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        private ParamsContainer paramsContainer = null;

        public void ProcessRequest(HttpContext context)
        {
            paramsContainer = new ParamsContainer(context, nullTenantResponse: true);
            if (!paramsContainer.ApplicationID.HasValue) return;

            string responseText = string.Empty;
            string command = PublicMethods.parse_string(context.Request.Params["Command"], false);

            switch (command)
            {
                case "Search":
                    List<SearchDocType> itemTypes = new List<SearchDocType>();

                    SearchDocType tempDt = new SearchDocType();
                    foreach (string str in ListMaker.get_string_items(context.Request.Params["ItemTypes"], '|'))
                        if (Enum.TryParse<SearchDocType>(str, out tempDt)) itemTypes.Add(tempDt);

                    search(itemTypes,
                        PublicMethods.parse_string(context.Request.Params["SearchText"]),
                        PublicMethods.parse_int(context.Request.Params["LowerBoundary"]),
                        PublicMethods.parse_int(context.Request.Params["Count"]),
                        PublicMethods.parse_bool(context.Request.Params["Title"]),
                        PublicMethods.parse_bool(context.Request.Params["Description"]),
                        PublicMethods.parse_bool(context.Request.Params["Content"]),
                        PublicMethods.parse_bool(context.Request.Params["Tags"]),
                        PublicMethods.parse_bool(context.Request.Params["FileContent"]),
                        PublicMethods.parse_bool(context.Request.Params["ForceHasContent"]),
                        ListMaker.get_guid_items(context.Request.Params["TypeIDs"], '|'),
                        ListMaker.get_string_items(context.Request.Params["Types"], '|'),
                        PublicMethods.parse_bool(context.Request.Params["ShowExactItems"]),
                        PublicMethods.parse_bool(context.Request.Params["SuggestNodeTypes"]),
                        PublicMethods.parse_bool(context.Request.Params["Excel"]),
                        PublicMethods.parse_bool(context.Request.Params["FormDetails"]),
                        PublicMethods.fromJSON(PublicMethods.parse_string(context.Request.Params["ColumnNames"])),
                        ref responseText);
                    _return_response(ref responseText);
                    return;
            }

            paramsContainer.return_response(PublicConsts.BadRequestResponse);
        }

        protected void _return_response(ref string responseText)
        {
            paramsContainer.return_response(ref responseText);
        }

        protected void search(List<SearchDocType> itemTypes, string searchText, int? lowerBoundary, int? count,
            bool? title, bool? description, bool? content, bool? tags, bool? fileContent, bool? forceHasContent,
            List<Guid> typeIds, List<string> types, bool? showExactItems, bool? suggestNodeTypes,
            bool? excel, bool? formDetails, Dictionary<string, object> columnNames, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBView) return;

            int initialLowerBoundary = !lowerBoundary.HasValue ? 0 : lowerBoundary.Value;
            if (!count.HasValue) count = 10;

            if (excel.HasValue && excel.Value)
            {
                itemTypes = new List<SearchDocType>() { SearchDocType.Node };
                showExactItems = suggestNodeTypes = false;
                lowerBoundary = 0;
                count = 1000000;
            }

            searchText = PublicMethods.convert_numbers_from_local(searchText.Trim());

            //'*' & '?' are not allowed as first character
            if (searchText.Length > 0 && (searchText.IndexOf("*") == 0 || searchText.IndexOf("?") == 0))
                searchText = searchText.Substring(1);

            List<SearchDoc> exactItems = new List<SearchDoc>();
            List<SearchDoc> nodeTypes = new List<SearchDoc>();

            if (!lowerBoundary.HasValue || lowerBoundary == 0)
            {
                string[] terms = searchText.Split(' ');
                
                if (terms != null && terms.Length == 1 && !string.IsNullOrEmpty(terms[0].Trim()) &&
                    showExactItems.HasValue && showExactItems.Value)
                {
                    SearchOptions exactOptions = new SearchOptions(
                        docTypes: itemTypes,
                        additionalId: true,
                        phrase: terms[0].Trim(),
                        count: count.Value);

                    exactItems = SearchUtilities.search(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID, exactOptions);
                }

                if (itemTypes.Exists(u => u == SearchDocType.Node) && suggestNodeTypes.HasValue && suggestNodeTypes.Value)
                {
                    SearchOptions typeOptions = new SearchOptions(
                        docTypes: new List<SearchDocType>() { SearchDocType.NodeType },
                        additionalId: true,
                        title: true,
                        description: true,
                        phrase: searchText,
                        count: count.Value);

                    nodeTypes = SearchUtilities.search(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID, typeOptions);
                }
            }

            SearchOptions options = new SearchOptions(
                docTypes: itemTypes,
                typeIds: typeIds,
                types: types,
                additionalId: true,
                title: title.HasValue && title.Value,
                description: description.HasValue && description.Value,
                content: content.HasValue && content.Value,
                tags: tags.HasValue && tags.Value,
                fileContent: fileContent.HasValue && fileContent.Value,
                forceHasContent: forceHasContent.HasValue && forceHasContent.Value,
                highlight: !excel.HasValue || !excel.Value,
                phrase: searchText,
                count: count.Value,
                lowerBoundary: initialLowerBoundary);

            List<SearchDoc> items = SearchUtilities.search(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID, options);

            if (excel.HasValue && excel.Value)
            {
                List<Node> nodes = items.Where(u => (!u.AccessIsDenied.HasValue || !u.AccessIsDenied.Value) &&
                    u.SearchDocType == SearchDocType.Node && u.TypeID.HasValue).Select(n => new Node()
                    {
                        NodeID = n.ID,
                        Name = n.Title,
                        NodeTypeID = n.TypeID,
                        NodeType = n.Type,
                        AdditionalID = n.AdditionalID
                    }).ToList();

                Dictionary<string, string> colsDic = new Dictionary<string, string>();
                columnNames.Keys.ToList().ForEach(k => { colsDic[k] = Base64.decode((string)columnNames[k]); });

                (new CNAPI() { paramsContainer = this.paramsContainer })
                    .export_nodes_to_excel(nodes, colsDic, formDetails.HasValue && formDetails.Value);

                return;
            }

            responseText = "{\"LastItem\":" + options.LowerBoundary.ToString() +
                (options.TotalCount == 0 ? string.Empty : ",\"TotalCount\":" + options.TotalCount.ToString()) +
                ",\"Items\":[" + string.Join(",", items.Select(u => u.toJson(paramsContainer.Tenant.Id, false))) + "]" +
                ",\"ExactItems\":[" + string.Join(",", exactItems.Select(u => u.toJson(paramsContainer.Tenant.Id, true))) + "]" +
                ",\"NodeTypes\":[" + string.Join(",", nodeTypes.Select(u => u.toJson(paramsContainer.Tenant.Id, true))) + "]" +
                ",\"NoMore\":" + (items.Count < count).ToString().ToLower() + "}";

            //Save Log
            if (paramsContainer.CurrentUserID.HasValue && initialLowerBoundary == 0 && !string.IsNullOrEmpty(searchText))
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.Search,
                    Info = "{\"Text\":\"" + (string.IsNullOrEmpty(searchText) ? string.Empty : Base64.encode(searchText)) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.SRCH
                });
            }
            //end of Save Log
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