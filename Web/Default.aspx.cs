using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RaaiVan.Modules.GlobalUtilities;

namespace RaaiVan.Web
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!RaaiVanSettings.ServiceUnavailable || !string.IsNullOrEmpty(Request.Params["iamadmin"])) 
                Response.Redirect(PublicConsts.LoginPage + Request.Url.Query);
        }
    }
}