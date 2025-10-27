using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using RaaiVan.Modules.CoreNetwork;
using System.Data;
using System.Web.Services.Protocols;
using RaaiVan.Modules.Users;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.NotificationCenter;

namespace RaaiVan.Web.Services
{
    /// <summary>
    /// Summary description for CoreNetwork
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class CoreNetwork : System.Web.Services.WebService
    {
        [WebMethod(EnableSession=true)]
        public bool AddNodeType(string nodeTypeId, string typeName, string description)
        {
            ITenant tenant = HttpContext.Current.GetCurrentTenant();
            if (tenant == null) return false;

            NodeType nt = new NodeType()
            {
                NodeTypeID = Guid.NewGuid(),
                NodeTypeAdditionalID = nodeTypeId,
                Name = typeName,
                Description = description,
                CreationDate = DateTime.Now
            };

            return CNController.add_node_type(tenant.Id, nt);
        }

        [WebMethod(EnableSession=true)]
        public bool AddNode(string nodeId, string nodeTypeId, string name, string description = null, string parentNodeId = null)
        {
            ITenant tenant = HttpContext.Current.GetCurrentTenant();
            if (tenant == null) return false;

            Node node = new Node()
            {
                NodeID = Guid.NewGuid(),
                AdditionalID = nodeId,
                Name = name,
                Description = description,
                CreationDate = DateTime.Now
            };

            if (!string.IsNullOrEmpty(parentNodeId))
                node.ParentNodeID = CNController.get_node_id(tenant.Id, parentNodeId, nodeTypeId);

            return CNController.add_node(applicationId: tenant.Id, info: node, nodeTypeAdditionalId: nodeTypeId);
        }

        [WebMethod]
        public bool ModifyNode(string nodeId, string nodeTypeId, string name, string description)
        {
            ITenant tenant = HttpContext.Current.GetCurrentTenant();
            if (tenant == null) return false;

            Node node = new Node()
            {
                NodeID = CNController.get_node_id(tenant.Id, nodeId, nodeTypeId),
                Name = name,
                Description = description,
                LastModificationDate = DateTime.Now
            };

            return CNController.modify_node(tenant.Id, node);
        }

        [WebMethod]
        public bool MoveNode(string nodeId, string parentNodeId, string nodeTypeId)
        {
            ITenant tenant = HttpContext.Current.GetCurrentTenant();
            if (tenant == null) return false;

            Node node = new Node()
            {
                NodeID = CNController.get_node_id(tenant.Id, nodeId, nodeTypeId),
                ParentNodeID = CNController.get_node_id(tenant.Id, parentNodeId, nodeTypeId),
                LastModificationDate = DateTime.Now
            };

            string errorMessage = string.Empty;

            return CNController.set_direct_parent(tenant.Id,
                node.NodeID.Value, node.ParentNodeID, Guid.Empty, ref errorMessage);
        }

        [WebMethod]
        public bool AddMember(string nodeId, string nodeTypeId, string username)
        {
            ITenant tenant = HttpContext.Current.GetCurrentTenant();
            if (tenant == null) return false;

            Guid? userId = UsersController.get_user_id(tenant.Id, username);
            Guid nId = CNController.get_node_id(tenant.Id, nodeId, nodeTypeId);

            NodeMember nm = new NodeMember();
            nm.Node.NodeID = nId;
            nm.Member.UserID = userId;
            nm.MembershipDate = DateTime.Now;
            nm.AcceptionDate = DateTime.Now;

            List<Dashboard> retDashboards = new List<Dashboard>();

            bool result = userId.HasValue && CNController.add_member(tenant.Id, nm, ref retDashboards);

            if (result) NotificationController.transfer_dashboards(tenant.Id, retDashboards);

            return result;
        }

        [WebMethod]
        public bool RemoveMember(string nodeId, string nodeTypeId, string username)
        {
            ITenant tenant = HttpContext.Current.GetCurrentTenant();
            if (tenant == null) return false;

            Guid? userId = UsersController.get_user_id(tenant.Id, username);

            return userId.HasValue && CNController.remove_member(tenant.Id, 
                CNController.get_node_id(tenant.Id, nodeId, nodeTypeId), userId.Value);
        }

        [WebMethod]
        public bool SetAdmin(string nodeId, string nodeTypeId, string username)
        {
            ITenant tenant = HttpContext.Current.GetCurrentTenant();
            if (tenant == null) return false;

            Guid? userId = UsersController.get_user_id(tenant.Id, username);

            return userId.HasValue && CNController.set_unset_node_admin(tenant.Id, 
                CNController.get_node_id(tenant.Id, nodeId, nodeTypeId), userId.Value, true);
        }

        [WebMethod]
        public bool UnsetAdmin(string nodeId, string nodeTypeId, string username)
        {
            ITenant tenant = HttpContext.Current.GetCurrentTenant();
            if (tenant == null) return false;

            Guid? userId = UsersController.get_user_id(tenant.Id, username);

            return userId.HasValue && CNController.set_unset_node_admin(tenant.Id, 
                CNController.get_node_id(tenant.Id, nodeId, nodeTypeId), userId.Value, false);
        }
    }
}
