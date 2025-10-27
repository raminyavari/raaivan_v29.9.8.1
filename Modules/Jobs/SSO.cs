using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using RaaiVan.Modules.GlobalUtilities;

namespace RaaiVan.Modules.Jobs
{
    public static class SSO
    {
        public static string get_login_url(Guid? applicationId)
        {
            return RaaiVanSettings.SSO.LoginURL(applicationId);
        }

        public static string get_ticket(Guid? applicationId, HttpContext context)
        {
            return context.Request.Params[RaaiVanSettings.SSO.TicketVariableName(applicationId)];
        }

        public static bool validate_ticket(Guid? applicationId, HttpContext context, string ticket, ref string validatedUserName)
        {
            validatedUserName = string.Empty;

            string validateUrl = RaaiVanSettings.SSO.ValidateURL(applicationId, ticket);

            string res = PublicMethods.web_request(validateUrl, null, HTTPMethod.GET);
            if (!string.IsNullOrEmpty(res)) res = res.Trim();

            if (!string.IsNullOrEmpty(res) && !string.IsNullOrEmpty(RaaiVanSettings.SSO.InvalidTicketCode(applicationId)) &&
                res.IndexOf(RaaiVanSettings.SSO.InvalidTicketCode(applicationId)) > 0) return false;

            if (!string.IsNullOrEmpty(res) && res[0] == '{')
            {
                Dictionary<string, object> jsonRes = PublicMethods.fromJSON(res);

                string[] jsonNames = RaaiVanSettings.SSO.JSONUserNamePath(applicationId).Split('.');

                if (jsonNames.Length > 0 && !string.IsNullOrEmpty(jsonNames[0]))
                {
                    for (int i = 0; jsonRes != null && i < jsonNames.Length; ++i)
                    {
                        if (i < jsonNames.Length - 1)
                        {
                            jsonRes = jsonRes.ContainsKey(jsonNames[i]) ?
                                (Dictionary<string, object>)jsonRes[jsonNames[i]] : null;
                        }
                        else if (jsonRes.ContainsKey(jsonNames[i])) validatedUserName = (string)jsonRes[jsonNames[i]];
                    }
                }
            }
            else if (!string.IsNullOrEmpty(res))
            {
                string _pre = "<" + RaaiVanSettings.SSO.XMLUserNameTag(applicationId) + ">";
                string _post = "</" + RaaiVanSettings.SSO.XMLUserNameTag(applicationId) + ">";

                if (res.IndexOf(_pre) > 0 && res.IndexOf(_post) > 0)
                {
                    validatedUserName = res.Substring(res.IndexOf(_pre) + _pre.Length);
                    validatedUserName = validatedUserName.Substring(0, validatedUserName.IndexOf(_post));
                }
            }

            return !string.IsNullOrEmpty(validatedUserName);
        }
    }
}
