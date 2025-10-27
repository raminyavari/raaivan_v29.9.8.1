using RaaiVan.Modules.GlobalUtilities;

namespace RaaiVan.Web
{
    using Microsoft.Owin;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Tenant Resolver class using request's hostname as key to find
    /// a possible Tenant
    /// </summary>
    public class HostNameTenantResolver : TenantResolverBase, ITenantResolver
    {
        /// <summary>
        /// Instances a new Resolver without tenants
        /// </summary>
        public HostNameTenantResolver() { }

        /// <summary>
        /// Instances a new resolver with a tenant list
        /// </summary>
        /// <param name="tenants">The list of tenants</param>
        /// <param name="databaseIdentifier">The Database Identifier</param>
        public HostNameTenantResolver(IEnumerable<ITenant> tenants) : base(tenants) { }

        /// <summary>
        /// Resolves a tenant request using hostname
        /// </summary>
        /// <param name="request">The request</param>
        /// <returns>The tenant or null</returns>
        public ITenant Resolve(IOwinRequest request)
        {
            return PublicMethods.get_current_tenant(request, Tenants);
        }
    }
}