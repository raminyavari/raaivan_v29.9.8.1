using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using System.Web;
using System.Web.Routing;

namespace RaaiVan.Modules.GlobalUtilities
{
    public interface ITenantResolver
    {
        /// <summary>
        /// This method should take a Owin Request and
        /// resolve the possible Tenant
        /// </summary>
        /// <param name="request">The request</param>
        /// <returns>The tenant</returns>
        ITenant Resolve(IOwinRequest request);

        /// <summary>
        /// Retrieves the Tenants in the resolver
        /// </summary>
        /// <returns>The Tenant's list</returns>
        List<ITenant> GetTenants();
    }

    public interface ITenant
    {
        /// <summary>
        /// Tenant's Id
        Guid Id { get; }

        /// <summary>
        /// Tenant's name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Tenant's title
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Tenant's avatar
        /// </summary>
        string AvatarName { get; }

        /// <summary>
        /// The Tenant domains
        /// </summary>
        string Domain { get; }

        /// <summary>
        /// The Tenant protocol
        /// </summary>
        string Protocol { get; }

        /// <summary>
        /// The Resolver instance
        /// </summary>
        ITenantResolver Resolver { get; set; }
    }

    /// <summary>
    /// Represents a common Tenant
    /// </summary>
    public class Tenant : ITenant
    {
        /// <summary>
        /// Tenant's Id
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Tenant's name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Tenant's title
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Tenant's avatar
        /// </summary>
        public string AvatarName { get; private set; }

        /// <summary>
        /// The Tenant domains
        /// </summary>
        public string Domain { get; private set; }

        /// <summary>
        /// The Tenant protocol
        /// </summary>
        public string Protocol { get; private set; }

        /// <summary>
        /// The Resolver instance
        /// </summary>
        public ITenantResolver Resolver { get; set; }

        /// <summary>
        /// Creates a new Tenant
        /// </summary>
        /// <param name="id">The ID</param>
        /// <param name="name">The name</param>
        /// <param name="domain">The related Domain</param>
        public Tenant(Guid id, string name, string title, string domain, string protocol, string avatarName)
        {
            Id = id;
            Name = name;
            Title = title;
            AvatarName = avatarName;
            Domain = domain;
            Protocol = protocol;
        }
    }

    /// <summary>
    /// Base Tenant Resolver functionality with internal Tenant List
    /// </summary>
    public abstract class TenantResolverBase
    {
        /// <summary>
        /// The Tenants repository
        /// </summary>
        protected List<ITenant> Tenants = new List<ITenant>();

        /// <summary>
        /// Instances a new Resolver without tenants
        /// </summary>
        protected TenantResolverBase() { }

        /// <summary>
        /// Instances a new resolver with a tenant list
        /// </summary>
        /// <param name="tenants">The list of tenants</param>
        protected TenantResolverBase(IEnumerable<ITenant> tenants)
        {
            Tenants.AddRange(tenants);
        }

        /// <summary>
        /// Adds a new tenant to resolver
        /// </summary>
        /// <param name="tenant">The tenant instance</param>
        public void Add(ITenant tenant)
        {
            Tenants.Add(tenant);
        }

        /// <summary>
        /// Retrieves the Tenants in the resolver
        /// </summary>
        /// <returns>The Tenant's list</returns>
        public List<ITenant> GetTenants()
        {
            return Tenants;
        }
    }

    public class RouteTenantResolver : TenantResolverBase, ITenantResolver
    {
        /// <summary>
        /// Defines the RouteName identifier
        /// </summary>
        public const string RouteName = "tenant";

        /// <summary>
        /// Instances a new resolver with a tenant list
        /// </summary>
        /// <param name="tenants">The list of tenants</param>
        public RouteTenantResolver(IEnumerable<ITenant> tenants) : base(tenants) { }

        /// <summary>
        /// Resolves a tenant request using hostname
        /// </summary>
        /// <param name="request">The request</param>
        /// <returns>The tenant or null</returns>
        public ITenant Resolve(IOwinRequest request)
        {
            var httpContext = request.Context.Get<HttpContextWrapper>("System.Web.HttpContextBase");
            var routeData = RouteTable.Routes.GetRouteData(httpContext);

            if (routeData == null || routeData.Values.ContainsKey(RouteName) == false)
                return Tenants.First();

            var tenant = Tenants.FirstOrDefault(x => x.Name == routeData.Values[RouteName].ToString());

            return tenant ?? (Tenants.Count == 1 ? Tenants.First() : null);
        }
    }
}