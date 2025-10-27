using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RaaiVan.Web.API;

namespace RaaiVan.Web.Page.Setting
{
    public partial class Classifications : System.Web.UI.Page
    {
        ParamsContainer paramsContainer = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            paramsContainer = new ParamsContainer(HttpContext.Current);
            RouteList.get_data_server_side(paramsContainer, RouteName.admin_confidentiality);
        }
    }
}