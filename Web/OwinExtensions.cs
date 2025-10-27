using RaaiVan.Modules.GlobalUtilities;

namespace RaaiVan.Web
{
    using Microsoft.Owin;
    using Owin;
    using System;
    using System.Web;

    /// <summary>
    /// Extensions methods
    /// </summary>
    public static class OwinExtensions
    {
        /// <summary>
        /// Add TenantCore to an AppBuilder
        /// </summary>
        /// <param name="app">The AppBuilder instance</param>
        /// <param name="resolver">The Tenant Resolver instance</param>
        /// <param name="callback">The callback method</param>
        /// <returns>AppBuilder with TenantCore</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IAppBuilder UseTenantCore(this IAppBuilder app, ITenantResolver resolver,
            Func<ITenant, bool> callback = null)
        {
            if (app == null)
            {
                throw new ArgumentNullException("app");
            }

            if (resolver == null)
            {
                throw new ArgumentNullException("resolver");
            }

            return app.Use(typeof (TenantCoreMiddleware), resolver, callback);
        }

        /// <summary>
        /// Retrieves the current Tenant from a HttpContext
        /// </summary>
        /// <param name="context">The Http context</param>
        /// <returns>The Tenant</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static ITenant GetCurrentTenant(this HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            return context.GetOwinContext().GetCurrentTenant();
        }

        /// <summary>
        /// Retrieves the current Tenant from a Owin Context
        /// </summary>
        /// <param name="context">The Owin context</param>
        /// <returns>The Tenant</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static ITenant GetCurrentTenant(this IOwinContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (RaaiVanSettings.SAASBasedMultiTenancy)
            {
                Application app = PublicMethods.get_current_application();
                return app == null ? null : app.toTenant();
            }
            else
                return context.Get<ITenant>(TenantCoreMiddleware.OwinPropertyName);
        }
    }
}