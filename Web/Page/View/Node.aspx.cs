using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RaaiVan.Web.API;
using RaaiVan.Modules.CoreNetwork;
using RaaiVan.Modules.GlobalUtilities;

namespace RaaiVan.Web.Page.View
{
    public partial class Node : System.Web.UI.Page
    {
        ParamsContainer paramsContainer = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            paramsContainer = new ParamsContainer(HttpContext.Current);
            initialJson.Value = PublicMethods.toJSON(RouteList.get_data_server_side(paramsContainer, RouteName.node));

            try
            {
                Guid? nodeId = PublicMethods.parse_guid(Request.Params["ID"], alternatvieValue:
                    PublicMethods.parse_guid(Request.Params["NodeID"]));

                if (Request.Url.ToString().ToLower().Contains("_escaped_fragment_=") && nodeId.HasValue)
                {
                    ParamsContainer paramsContainer = new ParamsContainer(HttpContext.Current);

                    Modules.CoreNetwork.Node _nd = CNController.get_node(paramsContainer.Tenant.Id, nodeId.Value, true);

                    string htmlContent = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">" +
                        "<html xmlns=\"http://www.w3.org/1999/xhtml\"><head><title>" + _nd.Name + " - " + RaaiVanSettings.SystemTitle(paramsContainer.Tenant.Id) + "</title></head><body>" +
                        "<div>" + _nd.Name + "</div>" +
                        "<div>" + ProviderUtil.list_to_string<string>(_nd.Tags, ' ') + "</div>" +
                        "<div>" + PublicMethods.shuffle_text(PublicMethods.markup2plaintext(paramsContainer.Tenant.Id,
                            _nd.Description, true)) + "</div>" +
                        "<div>" + PublicMethods.markup2plaintext(paramsContainer.Tenant.Id,
                            Modules.Wiki.WikiController.get_wiki_content(paramsContainer.Tenant.Id, nodeId.Value), true) + "</div>" +
                        "</body></html>";

                    paramsContainer.return_response(htmlContent);

                    return;
                }
            }
            catch { }
        }
    }
}