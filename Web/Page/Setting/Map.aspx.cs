using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Web.API;

namespace RaaiVan.Web.Page.Setting
{
    public partial class Map : System.Web.UI.Page
    {
        ParamsContainer paramsContainer = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            paramsContainer = new ParamsContainer(HttpContext.Current);
            initialJson.Value = PublicMethods.toJSON(RouteList.get_data_server_side(paramsContainer, RouteName.admin_map));
        }
    }
}