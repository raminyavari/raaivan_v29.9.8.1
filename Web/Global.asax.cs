using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Security.Principal;
using System.Data.Common;
using RaaiVan.Web.API;
using RaaiVan.Modules.GlobalUtilities;

namespace RaaiVan.Web
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            //RVScheduler.run_jobs(HttpContext.Current);

            /* //Encrypt Connection Strings
            System.Configuration.ConfigurationSection sec = 
                (System.Configuration.ConfigurationSection)System.Configuration.ConfigurationManager.GetSection("connectionStrings");
            if (sec != null)
            {
                sec.SectionInformation.ProtectSection("RSAProtectedConfigurationProvider");
                sec.SectionInformation.ForceSave = true;
            }
            */
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            /*
            String cookieName = FormsAuthentication.FormsCookieName;
            HttpCookie authCookie = Context.Request.Cookies[cookieName];

            if (null == authCookie)
            {//There is no authentication cookie.
                return;
            }

            FormsAuthenticationTicket authTicket = null;

            try
            {
                authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            }
            catch (Exception ex)
            {
                //Write the exception to the Event Log.
                return;
            }

            if (null == authTicket)
            {//Cookie failed to decrypt.
                return;
            }

            //When the ticket was created, the UserData property was assigned a
            //pipe-delimited string of group names.
            String[] groups = authTicket.UserData.Split(new char[] { '|' });

            //Create an Identity.
            GenericIdentity id = new GenericIdentity(authTicket.Name, "LdapAuthentication");

            //This principal flows throughout the request.
            GenericPrincipal principal = new GenericPrincipal(id, groups);

            Context.User = principal;
            */
        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {
        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}