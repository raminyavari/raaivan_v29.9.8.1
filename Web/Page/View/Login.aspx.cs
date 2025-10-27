using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using RaaiVan.Web.API;
using RaaiVan.Modules.GlobalUtilities;

namespace RaaiVan.Web.Page.View
{
    public partial class Login : System.Web.UI.Page
    {
        ParamsContainer paramsContainer = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            paramsContainer = new ParamsContainer(HttpContext.Current);
            initialJson.Value = PublicMethods.toJSON(RouteList.get_data_server_side(paramsContainer, RouteName.login));
            
            Page.Title = RaaiVanSettings.SystemTitle(paramsContainer.ApplicationID);

            string returnUrl = Request.Params["ReturnUrl"];
            
            if (RaaiVanSettings.IgnoreReturnURLOnLogin(paramsContainer.ApplicationID) && !string.IsNullOrEmpty(returnUrl))
                Response.Redirect(PublicConsts.LoginPage);

            if (!string.IsNullOrEmpty(RaaiVanSettings.Google.Captcha.URL))
            {
                Page.Header.Controls.Add(new LiteralControl("<script type='text/javascript' src='" +
                    RaaiVanSettings.Google.Captcha.URL + "'></script>"));
            }
        }
    }
}