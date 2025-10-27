using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using RaaiVan.Modules.CoreNetwork;
using RaaiVan.Modules.Knowledge;
using System.Data;
using System.Web.Services.Protocols;
using RaaiVan.Modules.Users;

namespace RaaiVan.Web.Services
{
    /// <summary>
    /// Summary description for Knowledge
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Knowledge : System.Web.Services.WebService
    {
        private List<Modules.CoreNetwork.Node> _get_nodes(ref DataTable nodesTbl)
        {
            List<Modules.CoreNetwork.Node> retList = new List<Modules.CoreNetwork.Node>();

            if (nodesTbl == null) return retList;

            for (int i = 0, lnt = nodesTbl.Rows.Count; i < lnt; ++i)
            {
                retList.Add(new Modules.CoreNetwork.Node()
                {
                    AdditionalID = nodesTbl.Rows[i]["NodeID"].ToString(),
                    TypeAdditionalID = nodesTbl.Rows[i]["NodeTypeID"].ToString()
                });
            }

            return retList;
        }
        
        /*
        private DataTable _parse_knowledges(ref List<Modules.Knowledge.Knowledge> knowledges)
        {
            DataTable ks = new DataTable("Knowledges");

            if (knowledges == null || knowledges.Count == 0) return ks;

            ks.Columns.Add("KnowledgeID", typeof(Guid));
            ks.Columns.Add("KnowledgeTypeID", typeof(int));
            ks.Columns.Add("KnowledgeType", typeof(string));
            ks.Columns.Add("Title", typeof(string));
            ks.Columns.Add("Description", typeof(string));
            ks.Columns.Add("ContentType", typeof(string));
            ks.Columns.Add("StatusID", typeof(int));
            ks.Columns.Add("CreationDate", typeof(DateTime));
            ks.Columns.Add("PublicationDate", typeof(DateTime));
            ks.Columns.Add("Score", typeof(double));
            ks.Columns.Add("ScoresWeight", typeof(double));
            ks.Columns.Add("CreatorUserID", typeof(Guid));
            ks.Columns.Add("CreatorUserName", typeof(string));
            ks.Columns.Add("CreatorFirstName", typeof(string));
            ks.Columns.Add("CreatorLastName", typeof(string));

            foreach (Modules.Knowledge.Knowledge _k in knowledges)
                ks.Rows.Add(_k.KnowledgeID, _k.KnowledgeTypeID, _k.KnowledgeType, _k.Title, _k.Abstract, _k.ContentType,
                    _k.StatusID, _k.CreationDate, _k.PublicationDate, _k.Score, _k.ScoresWeight, _k.CreatorUserID,
                    _k.CreatorUserName, _k.CreatorFirstName, _k.CreatorLastName);

            return ks;
        }
        */

        /*
        [WebMethod(EnableSession = true)]
        public DataTable GetRelatedKnowledges(string nodeId, string nodeTypeId)
        {
            //if (!Context.User.Identity.IsAuthenticated) return new DataTable();

            Guid gNodeId = Guid.Empty;
            bool result = Guid.TryParse(nodeId, out gNodeId);
            if (!result) gNodeId = CNController.get_node_id(nodeId, nodeTypeId);

            if (gNodeId == Guid.Empty) return new DataTable("Knowledges");

            Modules.CoreNetwork.Node node = CNController.get_node(gNodeId);
            bool isKd = node.NodeTypeAdditionalID.HasValue &&
                CNUtilities.get_node_type(node.NodeTypeAdditionalID.Value) == NodeTypes.KnowledgeDomain.ToString();

            List<Guid> relatedKnowledgeIds = isKd ? KnowledgeController.get_node_related_knowledge_ids(gNodeId) :
                CNController.get_related_node_ids(gNodeId, string.Empty, NodeTypes.Knowledge, null, null, true);

            if (relatedKnowledgeIds == null || relatedKnowledgeIds.Count == 0) return new DataTable("Knowledges");

            List<Modules.Knowledge.Knowledge> knowledges = KnowledgeController.get_knowledges(ref relatedKnowledgeIds);

            if (knowledges == null || knowledges.Count == 0) return new DataTable("Knowledges");
            
            return _parse_knowledges(ref knowledges);
        }
        */

        /*
        [WebMethod(EnableSession = true)]
        public DataTable SearchKnowledges(string viewerusername, string creatorusername, string searchText, DataTable relatedNodes, 
            DateTime? lowerCreationDateLimit, DateTime? upperCreationDateLimit,
            DateTime? lowerPublicationDateLimit, DateTime? upperPublicationDateLimit)
        {
            List<Guid> relatedNodeIds = CNController.get_node_ids(_get_nodes(ref relatedNodes));

            List<Modules.Knowledge.Knowledge> knowledges = KnowledgeController.search_knowledges(UsersController.get_user_id(viewerusername),
                searchText, null, 1000, null, UsersController.get_user_id(creatorusername), relatedNodeIds,
                lowerCreationDateLimit, upperCreationDateLimit, lowerPublicationDateLimit, upperPublicationDateLimit);

            return _parse_knowledges(ref knowledges);
        }
        */

        /*
        [WebMethod(EnableSession = true)]
        public int GetKnowledgesCount(string viewerusername, string creatorusername, string searchText, DataTable relatedNodes, 
            DateTime? lowerCreationDateLimit, DateTime? upperCreationDateLimit,
            DateTime? lowerPublicationDateLimit, DateTime? upperPublicationDateLimit)
        {
            List<Guid> relatedNodeIds = CNController.get_node_ids(_get_nodes(ref relatedNodes));

            List<Modules.Knowledge.Knowledge> knowledges = KnowledgeController.search_knowledges(UsersController.get_user_id(viewerusername),
                searchText, null, 1000, null, UsersController.get_user_id(creatorusername), relatedNodeIds,
                lowerCreationDateLimit, upperCreationDateLimit, lowerPublicationDateLimit, upperPublicationDateLimit);

            return knowledges == null ? 0 : knowledges.Count;
        }
        */

        [WebMethod(EnableSession = true)]
        public string TEST(int x, int y)
        {
            return "<Ramin>" + (x + y).ToString() + "</Ramin>";
        }
    }
}
