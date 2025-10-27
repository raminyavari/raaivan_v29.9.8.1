using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Common;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.Users;

namespace RaaiVan.Web.API
{
    public static class AuthorizationManager
    {
        public static List<AccessRoleName> has_right(List<AccessRoleName> roles, Guid? userId)
        {
            ITenant tenant = HttpContext.Current.GetCurrentTenant();

            if (!userId.HasValue || userId == Guid.Empty || tenant == null /*|| 
                !RaaiVanUtil.is_authenticated(HttpContext.Current)*/) return new List<AccessRoleName>();

            roles = AccessRole.remove_ref_tenant_specific_roles(tenant.Id, roles);
            
            if (PublicMethods.is_system_admin(tenant.Id, userId.Value)) return roles;

            List<AccessRoleName> ret = UsersController.check_user_group_permissions(tenant.Id, userId.Value, roles);

            return ret;
        }

        public static bool has_right(AccessRoleName rightName, Guid? userId)
        {
            return has_right(new List<AccessRoleName>() { rightName }, userId).Count > 0;
        }
        
        public static void redirect_if_no_access(AccessRoleName role, Guid? userId, 
            System.Web.UI.Page page, string redirectUrl)
        {
            if (!has_right(role, userId)) page.Response.Redirect(redirectUrl);
        }

        public static void redirect_if_no_access(AccessRoleName role, Guid? userId, System.Web.UI.Page page)
        {
            redirect_if_no_access(role, userId, page, PublicConsts.NoAccessPage.ToString());
        }
    }
}