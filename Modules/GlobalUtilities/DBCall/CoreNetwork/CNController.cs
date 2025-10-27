using System;
using System.Collections.Generic;
using System.Linq;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.Users;
using RaaiVan.Modules.FormGenerator;
using RaaiVan.Modules.Privacy;
using RaaiVan.Modules.GlobalUtilities.DBCompositeTypes;

namespace RaaiVan.Modules.CoreNetwork
{
    public static class CNController
    {
        public static string GetFullyQualifiedName(string name)
        {
            return "[dbo]." + "[CN_" + name + "]"; //'[dbo].' is database owner and 'CN_' is module qualifier
        }

        public static bool initialize(Guid applicationId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("Initialize"), applicationId, DateTime.Now);
        }

        public static bool add_node_type(Guid applicationId, NodeType info, Guid? templateFormId = null)
        {
            if (info.AdditionalID.HasValue) info.NodeTypeAdditionalID = info.AdditionalID.Value.ToString();

            return DBConnector.succeed(applicationId, GetFullyQualifiedName("AddNodeType"),
                applicationId, info.NodeTypeID, info.NodeTypeAdditionalID, info.Name, info.ParentID,
                info.IsService, info.TemplateTypeID, templateFormId, info.CreatorUserID, DateTime.Now);
        }

        public static bool rename_node_type(Guid applicationId, NodeType Info)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("RenameNodeType"),
                applicationId, Info.NodeTypeID, Info.Name, Info.LastModifierUserID, Info.LastModificationDate);
        }

        public static bool set_node_type_additional_id(Guid applicationId, Guid nodeTypeId,
            string additionalId, Guid currentUserId, ref string errorMessage)
        {
            return DBConnector.succeed(applicationId, ref errorMessage, GetFullyQualifiedName("SetNodeTypeAdditionalID"),
                applicationId, nodeTypeId, additionalId, currentUserId, DateTime.Now);
        }

        public static bool set_additional_id_pattern(Guid applicationId, NodeType Info)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetAdditionalIDPattern"),
                applicationId, Info.NodeTypeID, Info.AdditionalIDPattern, Info.LastModifierUserID, Info.LastModificationDate);
        }
        
        public static bool move_node_type(Guid applicationId, List<Guid> nodeTypeIds, Guid? parentId, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("MoveNodeType"),
                applicationId, ProviderUtil.list_to_string<Guid>(nodeTypeIds), ',', parentId, currentUserId, DateTime.Now);
        }
        
        public static List<NodeType> get_node_types(Guid applicationId, List<Guid> nodeTypeIds, bool grabSubNodeTypes = false)
        {
            DBResultSet results = DBConnector.read(applicationId, GetFullyQualifiedName("GetNodeTypesByIDs"),
                applicationId, ProviderUtil.list_to_string<Guid>(nodeTypeIds), ',', grabSubNodeTypes);

            long totalCount = 0;

            return CNParsers.node_types(results, ref totalCount);
        }

        public static List<NodeType> get_node_types(Guid applicationId, string searchText, bool? isKnowledge,
            bool? isDocument, bool? archive, List<ExtensionType> extensions, int? count, long? lowerBoundary, ref long totalCount)
        {
            if (extensions == null) extensions = new List<ExtensionType>();

            DBResultSet results = DBConnector.read(applicationId, GetFullyQualifiedName("GetNodeTypes"),
                applicationId, ProviderUtil.get_search_text(searchText), isKnowledge, isDocument, 
                archive, string.Join(",", extensions), ',', count, lowerBoundary);

            return CNParsers.node_types(results, ref totalCount);
        }

        public static List<NodeType> get_node_types(Guid applicationId, 
            string searchText, List<ExtensionType> extensions, bool? archive = false)
        {
            long totalCount = 0;
            return get_node_types(applicationId, searchText, null, null, archive, extensions, null, null, ref totalCount);
        }

        private static NodeType _get_node_type(Guid applicationId, Guid? nodeTypeId, NodeTypes? nodeType, Guid? nodeId)
        {
            string strNodeTypeAdditionalId = null;
            if (nodeType.HasValue) strNodeTypeAdditionalId = CNUtilities.get_node_type_additional_id(nodeType.Value).ToString();

            DBResultSet results = DBConnector.read(applicationId, GetFullyQualifiedName("GetNodeType"),
                applicationId, nodeTypeId, strNodeTypeAdditionalId, nodeId);

            long totalCount = 0;

            return CNParsers.node_types(results, ref totalCount).FirstOrDefault();
        }

        public static NodeType get_node_type_by_node_id(Guid applicationId, Guid nodeId)
        {
            return _get_node_type(applicationId: applicationId, nodeTypeId: null, nodeType: null, nodeId: nodeId);
        }

        public static NodeType get_node_type(Guid applicationId, Guid nodeTypeId)
        {
            return _get_node_type(applicationId: applicationId, nodeTypeId: nodeTypeId, nodeType: null, nodeId: null);
        }

        public static NodeType get_node_type(Guid applicationId, NodeTypes nodeType)
        {
            return _get_node_type(applicationId: applicationId, nodeTypeId: null, nodeType: nodeType, nodeId: null);
        }

        public static List<Guid> have_child_node_types(Guid applicationId, List<Guid> nodeTypeIds)
        {
            return DBConnector.get_guid_list(applicationId, GetFullyQualifiedName("HaveChildNodeTypes"),
                applicationId, ProviderUtil.list_to_string<Guid>(nodeTypeIds), ',');
        }

        public static List<NodeType> get_child_node_types(Guid applicationId, Guid? parentId, bool? archive = false)
        {
            DBResultSet results = DBConnector.read(applicationId, GetFullyQualifiedName("GetChildNodeTypes"),
                applicationId, parentId, archive);

            long totalCount = 0;

            return CNParsers.node_types(results, ref totalCount);
        }

        public static bool remove_node_types(Guid applicationId, List<Guid> nodeTypeIds, bool? removeHierarchy, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ArithmeticDeleteNodeTypes"),
                applicationId, ProviderUtil.list_to_string<Guid>(nodeTypeIds), ',', removeHierarchy, currentUserId, DateTime.Now);
        }

        public static bool remove_node_type(Guid applicationId, Guid nodeTypeId, bool? removeHierarchy, Guid currentUserId)
        {
            return remove_node_types(applicationId, new List<Guid>() { nodeTypeId }, removeHierarchy, currentUserId);
        }

        public static bool recover_node_type(Guid applicationId, NodeType Info)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("RecoverNodeType"),
                applicationId, Info.NodeTypeID, Info.LastModifierUserID, Info.LastModificationDate);
        }

        public static bool add_relation_type(Guid applicationId, RelationType Info)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("AddRelationType"),
                applicationId, Info.RelationTypeID, Info.Name, Info.Description, Info.CreatorUserID, Info.CreationDate);
        }

        public static bool modify_relation_type(Guid applicationId, RelationType Info)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ModifyRelationType"),
                applicationId, Info.RelationTypeID, Info.Name, Info.Description, Info.LastModifierUserID, Info.LastModificationDate);
        }

        public static List<RelationType> get_relation_types(Guid applicationId)
        {
            return CNParsers.relation_types(DBConnector.read(applicationId, GetFullyQualifiedName("GetRelationTypes"), applicationId));
        }

        public static bool remove_relation_type(Guid applicationId, RelationType Info)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ArithmeticDeleteRelationType"),
                applicationId, Info.RelationTypeID, Info.LastModifierUserID, DateTime.Now);
        }

        public static bool add_relations(Guid applicationId, List<Relation> relations, Guid creatorUserId)
        {
            List<KeyValuePair<KeyValuePair<Guid, Guid>, Guid>> _lstRelations = new List<KeyValuePair<KeyValuePair<Guid, Guid>, Guid>>();

            relations.Where(r => r.Source.NodeID.HasValue && r.Destination.NodeID.HasValue).ToList().ForEach(r => {
                Guid relTypeId = Guid.Empty;
                if (r.RelationType.RelationTypeID.HasValue) relTypeId = r.RelationType.RelationTypeID.Value;

                _lstRelations.Add(new KeyValuePair<KeyValuePair<Guid, Guid>, Guid>(
                    new KeyValuePair<Guid, Guid>(r.Source.NodeID.Value, r.Destination.NodeID.Value),
                    relTypeId));
            });

            string strRelations = ProviderUtil.triple_list_to_string<Guid, Guid, Guid>(ref _lstRelations, '|', ',');

            return DBConnector.succeed(applicationId, GetFullyQualifiedName("AddRelation"),
                applicationId, strRelations, '|', ',', creatorUserId, DateTime.Now);
        }

        public static bool save_relations(Guid applicationId, Guid nodeId, List<Guid> relatedNodeIds, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SaveRelations"),
                applicationId, nodeId, ProviderUtil.list_to_string<Guid>(relatedNodeIds), ',', currentUserId, DateTime.Now);
        }

        public static bool make_correlations(Guid applicationId, ref List<Relation> relations, Guid creatorUserId)
        {
            List<KeyValuePair<Guid, Guid>> _lstRelations = new List<KeyValuePair<Guid, Guid>>();

            relations.Where(r => r.Source.NodeID.HasValue && r.Destination.NodeID.HasValue).ToList().ForEach(r => {
                _lstRelations.Add(new KeyValuePair<Guid, Guid>(r.Source.NodeID.Value, r.Destination.NodeID.Value));
            });

            string strRelations = ProviderUtil.pair_list_to_string<Guid, Guid>(ref _lstRelations, '|', ',');

            return DBConnector.succeed(applicationId, GetFullyQualifiedName("MakeCorrelation"),
                applicationId, strRelations, '|', ',', creatorUserId, DateTime.Now);
        }

        public static bool remove_relations(Guid applicationId, 
            List<Relation> relations, Guid? relationTypeId, Guid lastModifierUserId, bool? reverseAlso = null)
        {
            List<KeyValuePair<Guid, Guid>> _lstRelations = new List<KeyValuePair<Guid, Guid>>();

            relations.Where(r => r.Source.NodeID.HasValue && r.Destination.NodeID.HasValue).ToList().ForEach(r => {
                _lstRelations.Add(new KeyValuePair<Guid, Guid>(r.Source.NodeID.Value, r.Destination.NodeID.Value));
            });

            string strRelations = ProviderUtil.pair_list_to_string<Guid, Guid>(ref _lstRelations, '|', ',');

            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ArithmeticDeleteRelations"),
                applicationId, strRelations, '|', ',', relationTypeId, lastModifierUserId, DateTime.Now, reverseAlso);
        }

        public static bool remove_relation(Guid applicationId, 
            Relation relation, Guid? relationTypeId, Guid lastModifierUserId, bool? reverseAlso = null)
        {
            return remove_relations(applicationId, new List<Relation>() { relation }, relationTypeId, lastModifierUserId, reverseAlso);
        }

        private static bool _add_node(Guid applicationId, Node info, bool? addMember = null, 
            NodeTypes? nodeType = null, string nodeTypeAdditionalId = null)
        {
            if (nodeType.HasValue) nodeTypeAdditionalId = CNUtilities.get_node_type_additional_id(nodeType.Value).ToString();
            if (string.IsNullOrEmpty(info.AdditionalID)) info.AdditionalID = null;

            return DBConnector.succeed(applicationId, GetFullyQualifiedName("AddNode"),
                applicationId, info.NodeID, info.AdditionalID_Main, info.AdditionalID, info.NodeTypeID, nodeTypeAdditionalId,
                    info.DocumentTreeNodeID, info.Name, info.Description, ProviderUtil.get_tags_text(info.Tags),
                    info.Searchable, info.Creator.UserID, info.CreationDate, info.ParentNodeID, addMember);
        }

        public static bool add_node(Guid applicationId, Node info, bool? addMember = null, string nodeTypeAdditionalId = null)
        {
            NodeTypes? nodeType = null;
            return _add_node(applicationId, info, addMember, nodeType, nodeTypeAdditionalId);
        }

        public static bool add_node(Guid applicationId, Node info, NodeTypes nodeType, bool? addMember = null)
        {
            string nodeTypeAdditionalId = null;
            return _add_node(applicationId, info, addMember, nodeType, nodeTypeAdditionalId);
        }

        public static bool set_additional_id(Guid applicationId, 
            Guid id, string additionalId_main, string additionalId, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetAdditionalID"),
                applicationId, id, additionalId_main, additionalId, currentUserId, DateTime.Now);
        }

        public static bool modify_node(Guid applicationId, Node info)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ModifyNode"),
                applicationId, info.NodeID, info.Name, info.Description, 
                ProviderUtil.get_tags_text(info.Tags), info.LastModifierUserID, DateTime.Now);
        }

        public static bool change_node_type(Guid applicationId, Guid nodeId, Guid nodeTypeId, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ChangeNodeType"),
                applicationId, nodeId, nodeTypeId, currentUserId, DateTime.Now);
        }

        public static bool set_document_tree_node_id(Guid applicationId, 
            List<Guid> nodeIds, Guid? documentTreeNodeId, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetDocumentTreeNodeID"),
                applicationId, ProviderUtil.list_to_string<Guid>(nodeIds), ',', documentTreeNodeId, currentUserId, DateTime.Now);
        }

        public static bool set_document_tree_node_id(Guid applicationId, 
            Guid nodeId, Guid? documentTreeNodeId, Guid currentUserId)
        {
            return set_document_tree_node_id(applicationId, new List<Guid>() { nodeId }, documentTreeNodeId, currentUserId);
        }

        public static bool modify_node_name(Guid applicationId, Node info)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ModifyNodeName"),
                applicationId, info.NodeID, info.Name, info.LastModifierUserID, DateTime.Now);
        }

        public static bool modify_node_description(Guid applicationId, Node info)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ModifyNodeDescription"),
                applicationId, info.NodeID, info.Description, info.LastModifierUserID, DateTime.Now);
        }

        public static bool modify_node_public_description(Guid applicationId, Guid nodeId, string description)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ModifyNodePublicDescription"),
                applicationId, nodeId, description);
        }

        public static bool set_node_expiration_date(Guid applicationId, Guid nodeId, DateTime? expirationDate)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetNodeExpirationDate"),
                applicationId, nodeId, expirationDate);
        }

        public static bool set_expired_nodes_as_not_searchable(Guid applicationId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetExpiredNodesAsNotSearchable"),
                applicationId, DateTime.Now);
        }

        public static List<Guid> get_node_ids_that_will_be_expired_soon(Guid applicationId)
        {
            DateTime date = DateTime.Now.AddDays(RaaiVanSettings.Knowledge.AlertExpirationInDays(applicationId));
            return DBConnector.get_guid_list(applicationId, GetFullyQualifiedName("GetNodeIDsThatWillBeExpiredSoon"), applicationId, date);
        }

        public static bool notify_node_expiration(Guid applicationId, Guid nodeId, Guid userId, ref List<Dashboard> retDashboards)
        {
            return DBConnector.get_dashboards(applicationId, ref retDashboards, GetFullyQualifiedName("NotifyNodeExpiration"),
                applicationId, nodeId, userId, DateTime.Now) > 0;
        }

        public static bool set_previous_version(Guid applicationId, Guid nodeId, Guid? previousVersionId, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetPreviousVersion"),
                applicationId, nodeId, previousVersionId, currentUserId, DateTime.Now);
        }

        public static List<Node> get_previous_versions(Guid applicationId,
            Guid nodeId, Guid? currentUserId = null, bool? checkPrivacy = false)
        {
            return CNParsers.nodes(DBConnector.read(applicationId, GetFullyQualifiedName("GetPreviousVersions"),
                applicationId, nodeId, currentUserId, checkPrivacy, DateTime.Now, RaaiVanSettings.DefaultPrivacy(applicationId)));
        }

        public static List<Node> get_new_versions(Guid applicationId, Guid nodeId)
        {
            return CNParsers.nodes(DBConnector.read(applicationId, GetFullyQualifiedName("GetNewVersions"), applicationId, nodeId));
        }

        public static bool modify_node_tags(Guid applicationId, Node info)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ModifyNodeTags"),
                applicationId, info.NodeID, ProviderUtil.get_tags_text(info.Tags), info.LastModifierUserID, DateTime.Now);
        }

        public static string get_node_description(Guid applicationId, Guid nodeId)
        {
            return DBConnector.get_string(applicationId, GetFullyQualifiedName("GetNodeDescription"), applicationId, nodeId);
        }

        public static bool set_nodes_searchability(Guid applicationId, List<Guid> nodeIds, bool searchable, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetNodesSearchability"),
                applicationId, ProviderUtil.list_to_string<Guid>(nodeIds), ',', searchable, currentUserId, DateTime.Now);
        }

        public static bool set_node_searchability(Guid applicationId, Guid nodeId, bool searchable, Guid currentUserId)
        {
            return set_nodes_searchability(applicationId, new List<Guid>() { nodeId }, searchable, currentUserId);
        }

        public static bool remove_nodes(Guid applicationId, List<Guid> nodeIds, bool? removeHierarchy, Guid lastModifierUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ArithmeticDeleteNodes"),
                applicationId, ProviderUtil.list_to_string<Guid>(nodeIds), ',', removeHierarchy, lastModifierUserId, DateTime.Now);
        }

        public static bool remove_node(Guid applicationId, Guid nodeId, bool? removeHierarchy, Guid lastModifierUserId)
        {
            return remove_nodes(applicationId, new List<Guid>() { nodeId }, removeHierarchy, lastModifierUserId);
        }

        public static bool recycle_nodes(Guid applicationId, List<Guid> nodeIds, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("RecycleNodes"),
                applicationId, ProviderUtil.list_to_string<Guid>(nodeIds), ',', currentUserId, DateTime.Now);
        }

        public static bool set_node_types_order(Guid applicationId, List<Guid> nodeTypeIds)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetNodeTypesOrder"),
                applicationId, ProviderUtil.list_to_string<Guid>(nodeTypeIds), ',');
        }

        public static bool set_nodes_order(Guid applicationId, List<Guid> nodeIds)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetNodesOrder"),
                applicationId, ProviderUtil.list_to_string<Guid>(nodeIds), ',');
        }

        private static List<NodesCount> _get_nodes_count(Guid applicationId, 
            ref List<Guid> nodeTypeIds, NodeTypes? nodeType, DateTime? lowerCreationDateLimit = null, 
            DateTime? upperCreationDateLimit = null, bool? archive = false, bool? root = null)
        {
            string strAdditionalId = null;
            if (nodeType.HasValue) strAdditionalId = CNUtilities.get_node_type_additional_id(nodeType.Value).ToString();

            return CNParsers.nodes_count(DBConnector.read(applicationId, GetFullyQualifiedName("GetNodesCount"),
                applicationId, ProviderUtil.list_to_string<Guid>(nodeTypeIds), ',',
                strAdditionalId, lowerCreationDateLimit, upperCreationDateLimit, root, archive));
        }

        public static List<NodesCount> get_nodes_count(Guid applicationId, DateTime? lowerCreationDateLimit = null,
            DateTime? upperCreationDateLimit = null, bool? archive = false, bool? root = null)
        {
            List<Guid> lst = new List<Guid>();
            return _get_nodes_count(applicationId, ref lst, null, lowerCreationDateLimit, 
                upperCreationDateLimit, archive, root);
        }

        public static NodesCount get_nodes_count(Guid applicationId, Guid nodeTypeId, 
            DateTime? lowerCreationDateLimit = null, DateTime? upperCreationDateLimit = null, 
            bool? archive = false, bool? root = null)
        {
            List<Guid> lst = new List<Guid>();
            lst.Add(nodeTypeId);
            return _get_nodes_count(applicationId, ref lst, null, lowerCreationDateLimit, upperCreationDateLimit, 
                archive, root).FirstOrDefault();
        }

        public static List<NodesCount> get_nodes_count(Guid applicationId, ref List<Guid> nodeTypeIds, 
            DateTime? lowerCreationDateLimit = null, DateTime? upperCreationDateLimit = null, 
            bool? archive = false, bool? root = null)
        {
            return _get_nodes_count(applicationId, ref nodeTypeIds, null, 
                lowerCreationDateLimit, upperCreationDateLimit, archive, root);
        }

        public static NodesCount get_nodes_count(Guid applicationId, NodeTypes nodeType, 
            DateTime? lowerCreationDateLimit = null, DateTime? upperCreationDateLimit = null, 
            bool? archive = false, bool? root = null)
        {
            List<Guid> lst = new List<Guid>();
            return _get_nodes_count(applicationId, ref lst, nodeType, lowerCreationDateLimit, 
                upperCreationDateLimit, archive, root).FirstOrDefault();
        }

        public static List<NodesCount> get_most_populated_node_types(Guid applicationId, 
            int? count = null, int? lowerBoundary = null)
        {
            return CNParsers.nodes_count(DBConnector.read(applicationId, GetFullyQualifiedName("GetMostPopulatedNodeTypes"),
                applicationId, count, lowerBoundary));
        }

        public static int get_node_records_count(Guid applicationId)
        {
            return DBConnector.get_int(applicationId, GetFullyQualifiedName("GetNodeRecordsCount"), applicationId);
        }

        public static List<Guid> get_node_type_ids(Guid applicationId, List<string> nodeTypeAdditionalIds)
        {
            return DBConnector.get_guid_list(applicationId, GetFullyQualifiedName("GetNodeTypeIDs"),
                applicationId, string.Join(",", nodeTypeAdditionalIds), ',');
        }

        public static Guid? get_node_type_id(Guid applicationId, string nodeTypeAdditionalId)
        {
            List<Guid> ids = get_node_type_ids(applicationId, new List<string>() { nodeTypeAdditionalId });

            if (ids.Count == 0) return null;
            else return ids[0];
        }

        private static List<Guid> _get_node_ids(Guid applicationId, List<string> nodeAdditionalIds, 
            Guid? nodeTypeId, NodeTypes? nodeType, string nodeTypeAdditionalId)
        {
            //prepare
            if (!nodeTypeId.HasValue && !nodeType.HasValue && string.IsNullOrEmpty(nodeTypeAdditionalId)) return new List<Guid>();
            if (nodeType.HasValue) nodeTypeAdditionalId = CNUtilities.get_node_type_additional_id(nodeType.Value).ToString();
            //end of prepare

            return DBConnector.get_guid_list(applicationId, GetFullyQualifiedName("GetNodeIDs"),
                applicationId, StringTableType.getCompositeType(nodeAdditionalIds), nodeTypeId, nodeTypeAdditionalId);
        }

        public static List<Guid> get_node_ids(Guid applicationId, List<string> nodeAdditionalIds, Guid nodeTypeId)
        {
            return _get_node_ids(applicationId, nodeAdditionalIds, nodeTypeId, null, null);
        }

        public static List<Guid> get_node_ids(Guid applicationId, List<string> nodeAdditionalIds, NodeTypes nodeType)
        {
            return _get_node_ids(applicationId, nodeAdditionalIds, null, nodeType, null);
        }

        public static List<Guid> get_node_ids(Guid applicationId, List<string> nodeAdditionalIds, string nodeTypeAdditionalId)
        {
            return _get_node_ids(applicationId, nodeAdditionalIds, null, null, nodeTypeAdditionalId);
        }

        public static Guid get_node_id(Guid applicationId, string nodeAdditionalId, Guid nodeTypeId)
        {
            List<string> _aIds = new List<string>();
            _aIds.Add(nodeAdditionalId);
            return _get_node_ids(applicationId, _aIds, nodeTypeId, null, null).FirstOrDefault();
        }

        public static Guid get_node_id(Guid applicationId, string nodeAdditionalId, NodeTypes nodeType)
        {
            List<string> _aIds = new List<string>();
            _aIds.Add(nodeAdditionalId);
            return _get_node_ids(applicationId, _aIds, null, nodeType, null).FirstOrDefault();
        }

        public static Guid get_node_id(Guid applicationId, string nodeAdditionalId, string nodeTypeAdditionalId)
        {
            List<string> _aIds = new List<string>();
            _aIds.Add(nodeAdditionalId);
            return _get_node_ids(applicationId, _aIds, null, null, nodeTypeAdditionalId).FirstOrDefault();
        }

        public static List<Guid> get_node_ids(Guid applicationId, List<Node> nodes)
        {
            if (nodes == null) nodes = new List<Node>();

            DBCompositeType<StringPairTableType> addIds = new DBCompositeType<StringPairTableType>()
                .add(nodes.Select(nd => new StringPairTableType(nd.AdditionalID, nd.TypeAdditionalID)).ToList());

            return DBConnector.get_guid_list(applicationId, GetFullyQualifiedName("GetNodeIDsByAdditionalIDs"), applicationId, addIds);
        }

        public static Dictionary<string, Guid> get_node_ids(Guid applicationId, Guid nodeTypeId, List<string> additionalIds)
        {
            return CNParsers.node_ids(DBConnector.read(applicationId, GetFullyQualifiedName("GetNodeIDsByAdditionalID"),
                applicationId, nodeTypeId, string.Join("~", additionalIds), '~'));
        }

        public static List<Node> get_nodes(Guid applicationId, 
            List<Guid> nodeIds, bool? full = null, Guid? currentUserId = null)
        {
            if (nodeIds.Count == 0) return new List<Node>();

            DBResultSet resutls = DBConnector.read(applicationId, GetFullyQualifiedName("GetNodesByIDs"),
                applicationId, ProviderUtil.list_to_string<Guid>(nodeIds), ',', full, currentUserId);

            return CNParsers.nodes(resutls, full: full);
        }

        public static Node get_node(Guid applicationId, 
            Guid nodeId, bool? full = null, Guid? currentUserId = null)
        {
            List<Guid> _nIds = new List<Guid>();
            _nIds.Add(nodeId);
            return get_nodes(applicationId, _nIds, full, currentUserId).FirstOrDefault();
        }

        private static List<Node> get_nodes(Guid applicationId,
            ref long totalCount, ref List<NodesCount> nodesCount, ref Dictionary<string, object> groupedResults,
            List<Guid> nodeTypeIds = null, NodeTypes? nodeType = null, bool? useNodeTypeHierarchy = null, 
            List<Guid> relatedToIds = null, string searchText = null, bool? isDocument = null, bool? isKnowledge = null, 
            DateTime? lowerCreationDateLimit = null, DateTime? upperCreationDateLimit = null,
            int? count = null, long? lowerBoundary = null, bool? searchable = null, bool? archive = null, 
            bool? grabNoContentServices = null, bool? isFavorite = null, bool? isGroup = null, bool? isExpertiseDomain = null,
            List<FormFilter> filters = null, bool? matchAllFilters = null, bool? fetchCounts = null, Guid? currentUserId = null, 
            List<Guid> creatorUserIds = null, bool checkAccess = false, Guid? groupByFormElementId = null)
        {
            //prepare
            string strAddId = null;
            if (nodeType.HasValue) strAddId = CNUtilities.get_node_type_additional_id(nodeType.Value).ToString();
            //end of prepare

            DBResultSet results = DBConnector.read(applicationId, GetFullyQualifiedName("GetNodes"),
                applicationId,
                currentUserId,
                GuidTableType.getCompositeType(nodeTypeIds),
                string.IsNullOrEmpty(strAddId) ? null : strAddId,
                useNodeTypeHierarchy,
                GuidTableType.getCompositeType(relatedToIds),
                string.IsNullOrEmpty(searchText) ? null : ProviderUtil.get_search_text(searchText),
                isDocument,
                isKnowledge,
                GuidTableType.getCompositeType(creatorUserIds),
                searchable,
                archive,
                grabNoContentServices,
                lowerCreationDateLimit,
                upperCreationDateLimit,
                count,
                lowerBoundary,
                isFavorite,
                isGroup,
                isExpertiseDomain,
                FormFilterTableType.getCompositeType(filters),
                matchAllFilters,
                fetchCounts,
                checkAccess,
                RaaiVanSettings.DefaultPrivacy(applicationId),
                groupByFormElementId,
                DateTime.Now);

            if (groupByFormElementId.HasValue && groupByFormElementId != Guid.Empty)
            {
                groupedResults = CNParsers.node_counts_grouped_by_element(results);
                return new List<Node>();
            }
            else
            {
                return CNParsers.nodes(results, ref nodesCount, ref totalCount, full: false,
                    fetchCounts: fetchCounts.HasValue && fetchCounts.Value);
            }
        }

        public static List<Node> get_nodes(Guid applicationId,
            List<Guid> nodeTypeIds = null, NodeTypes? nodeType = null, bool? useNodeTypeHierarchy = null,
            List<Guid> relatedToIds = null, string searchText = null, bool? isDocument = null, bool? isKnowledge = null,
            DateTime? lowerCreationDateLimit = null, DateTime? upperCreationDateLimit = null,
            int? count = null, long? lowerBoundary = null, bool? searchable = null, bool? archive = null,
            bool? grabNoContentServices = null, bool? isFavorite = null, bool? isGroup = null, bool? isExpertiseDomain = null,
            List<FormFilter> filters = null, bool? matchAllFilters = null,
            Guid? currentUserId = null, List<Guid> creatorUserIds = null, bool checkAccess = false)
        {
            long totalCount = 0;
            List<NodesCount> retNodesCount = new List<NodesCount>();
            Dictionary<string, object> groupedResults = new Dictionary<string, object>();
            bool? fetchCounts = null;
            Guid? groupByFormElementId = null;

            return get_nodes(applicationId, ref totalCount, ref retNodesCount, ref groupedResults, nodeTypeIds,
                nodeType, useNodeTypeHierarchy, relatedToIds, searchText, isDocument, isKnowledge,
                lowerCreationDateLimit, upperCreationDateLimit, count, lowerBoundary, searchable, archive,
                grabNoContentServices, isFavorite, isGroup, isExpertiseDomain, filters, matchAllFilters, fetchCounts,
                currentUserId, creatorUserIds, checkAccess, groupByFormElementId);
        }

        public static List<Node> get_nodes(Guid applicationId, ref long totalCount, ref List<NodesCount> nodesCount,
            List<Guid> nodeTypeIds = null, NodeTypes? nodeType = null, bool? useNodeTypeHierarchy = null,
            List<Guid> relatedToIds = null, string searchText = null, bool? isDocument = null, bool? isKnowledge = null,
            DateTime? lowerCreationDateLimit = null, DateTime? upperCreationDateLimit = null,
            int? count = null, long? lowerBoundary = null, bool? searchable = null, bool? archive = null,
            bool? grabNoContentServices = null, bool? isFavorite = null, bool? isGroup = null, bool? isExpertiseDomain = null, 
            List<FormFilter> filters = null, bool? matchAllFilters = null,
            bool? fetchCounts = null, Guid ? currentUserId = null, List<Guid> creatorUserIds = null, bool checkAccess = false)
        {
            Dictionary<string, object> groupedResults = new Dictionary<string, object>();
            Guid? groupByFormElementId = null;

            return get_nodes(applicationId, ref totalCount, ref nodesCount, ref groupedResults, nodeTypeIds,
                nodeType, useNodeTypeHierarchy, relatedToIds, searchText, isDocument, isKnowledge,
                lowerCreationDateLimit, upperCreationDateLimit, count, lowerBoundary, searchable, archive,
                grabNoContentServices, isFavorite, isGroup, isExpertiseDomain, filters, matchAllFilters, fetchCounts, 
                currentUserId, creatorUserIds, checkAccess, groupByFormElementId);
        }

        public static List<Node> get_nodes(Guid applicationId, ref long totalCount,
            List<Guid> nodeTypeIds = null, NodeTypes? nodeType = null, bool? useNodeTypeHierarchy = null,
            List<Guid> relatedToIds = null, string searchText = null, bool? isDocument = null, bool? isKnowledge = null,
            DateTime? lowerCreationDateLimit = null, DateTime? upperCreationDateLimit = null,
            int? count = null, long? lowerBoundary = null, bool? searchable = null, bool? archive = null,
            bool? grabNoContentServices = null, bool? isFavorite = null, bool? isGroup = null, bool? isExpertiseDomain = null, 
            List<FormFilter> filters = null, bool? matchAllFilters = null,
            Guid? currentUserId = null, List<Guid> creatorUserIds = null, bool checkAccess = false)
        {
            List<NodesCount> nodesCount = new List<NodesCount>();
            bool? fetchCounts = null;
            Dictionary<string, object> groupedResults = new Dictionary<string, object>();
            Guid? groupByFormElementId = null;

            return get_nodes(applicationId, ref totalCount, ref nodesCount, ref groupedResults, nodeTypeIds,
                nodeType, useNodeTypeHierarchy, relatedToIds, searchText, isDocument, isKnowledge,
                lowerCreationDateLimit, upperCreationDateLimit, count, lowerBoundary, searchable, archive,
                grabNoContentServices, isFavorite, isGroup, isExpertiseDomain, filters, matchAllFilters, fetchCounts, 
                currentUserId, creatorUserIds, checkAccess, groupByFormElementId);
        }

        public static Dictionary<string, object> get_nodes_grouped(Guid applicationId, Guid nodeTypeId, 
            Guid groupByFormElementId, List<Guid> relatedToIds = null, string searchText = null, bool? isDocument = null, 
            bool? isKnowledge = null, DateTime? lowerCreationDateLimit = null, DateTime? upperCreationDateLimit = null, 
            bool? searchable = null, bool? isFavorite = null, bool? isGroup = null, bool? isExpertiseDomain = null,
            List<FormFilter> filters = null, bool? matchAllFilters = null,
            Guid? currentUserId = null, List<Guid> creatorUserIds = null, bool checkAccess = false)
        {
            long totalCount = 0;
            List<NodesCount> nodesCount = new List<NodesCount>();
            Dictionary<string, object> groupedResults = new Dictionary<string, object>();

            get_nodes(applicationId: applicationId, totalCount: ref totalCount, nodesCount: ref nodesCount, 
                groupedResults: ref groupedResults, nodeTypeIds: new List<Guid>() { nodeTypeId },
                relatedToIds: relatedToIds, searchText: searchText, isDocument: isDocument, isKnowledge: isKnowledge,
                lowerCreationDateLimit: lowerCreationDateLimit, upperCreationDateLimit: upperCreationDateLimit, 
                searchable: searchable, isFavorite: isFavorite, isGroup: isGroup, isExpertiseDomain: isExpertiseDomain, 
                filters: filters, matchAllFilters: matchAllFilters, currentUserId: currentUserId, 
                creatorUserIds: creatorUserIds, checkAccess: checkAccess, groupByFormElementId: groupByFormElementId);

            return groupedResults == null ? new Dictionary<string, object>() : groupedResults;
        }

        public static List<Node> get_most_popular_nodes(Guid applicationId, List<Guid> nodeTypeIds, Guid? parentNodeId,
            int count, long? lowerBoundary, ref long totalCount, bool? archive = false, bool? searchable = true)
        {
            DBResultSet results = DBConnector.read(applicationId, GetFullyQualifiedName("GetMostPopularNodes"),
                applicationId, ProviderUtil.list_to_string<Guid>(nodeTypeIds), ',', parentNodeId, count, lowerBoundary);

            return CNParsers.popular_nodes(results, ref totalCount);
        }

        public static List<Node> get_most_popular_nodes(Guid applicationId, Guid? nodeTypeId, Guid? parentNodeId, 
            int count, long? lowerBoundary, ref long totalCount)
        {
            List<Guid> nodeTypeIds = new List<Guid>();
            if (nodeTypeId.HasValue && nodeTypeId != Guid.Empty) nodeTypeIds.Add(nodeTypeId.Value);
            return get_most_popular_nodes(applicationId, nodeTypeIds, parentNodeId, count, lowerBoundary, ref totalCount);
        }

        public static List<Node> get_parent_nodes(Guid applicationId, Guid nodeId)
        {
            return CNParsers.nodes(DBConnector.read(applicationId, GetFullyQualifiedName("GetParentNodes"), applicationId, nodeId));
        }

        public static List<Node> get_child_nodes(Guid applicationId, Guid nodeId)
        {
            return CNParsers.nodes(DBConnector.read(applicationId, GetFullyQualifiedName("GetChildNodes"), applicationId, nodeId));
        }

        public static List<Node> get_direct_childs(Guid applicationId, Guid? nodeId, Guid? nodeTypeId, 
            string nodeTypeAdditionalId, bool? searchable, double? lowerBoundary, int? count, 
            string orderBy, bool? orderByDesc, string searchText, bool? checkAccess, Guid? currentUserId, ref long totalCount)
        {
            DBResultSet results = DBConnector.read(applicationId, GetFullyQualifiedName("GetDirectChilds"),
                applicationId, nodeId, nodeTypeId, nodeTypeAdditionalId, searchable, lowerBoundary, count,
                orderBy, orderByDesc, ProviderUtil.get_search_text(searchText), checkAccess,
                currentUserId, DateTime.Now, RaaiVanSettings.DefaultPrivacy(applicationId));

            return CNParsers.nodes(results, full: false, totalCount: ref totalCount);
        }

        public static List<Node> get_direct_childs(Guid applicationId, Guid nodeId)
        {
            long totalCount = 0;
            return get_direct_childs(applicationId, nodeId, null, null, true, null, null, null, null, null, null, null, ref totalCount);
        }

        public static Node get_direct_parent(Guid applicationId, Guid nodeId)
        {
            return CNParsers.nodes(DBConnector.read(applicationId, GetFullyQualifiedName("GetDirectParent"),
                applicationId, nodeId)).FirstOrDefault();
        }

        public static bool set_direct_parent(Guid applicationId, 
            List<Guid> nodeIds, Guid? parentNodeId, Guid currentUserId, ref string errorMessage)
        {
            return DBConnector.succeed(applicationId, ref errorMessage, GetFullyQualifiedName("SetDirectParent"),
                applicationId, ProviderUtil.list_to_string<Guid>(nodeIds), ',', parentNodeId, currentUserId, DateTime.Now);
        }

        public static bool set_direct_parent(Guid applicationId, Guid nodeId, Guid? parentNodeId, 
            Guid currentUserId, ref string errorMessage)
        {
            return set_direct_parent(applicationId, new List<Guid>() { nodeId }, parentNodeId, currentUserId, ref errorMessage);
        }

        public static List<Guid> have_childs(Guid applicationId, List<Guid> nodeIds)
        {
            return DBConnector.get_guid_list(applicationId, GetFullyQualifiedName("HaveChilds"),
                applicationId, ProviderUtil.list_to_string<Guid>(nodeIds), ',');
        }

        public static bool has_childs(Guid applicationId, Guid nodeId)
        {
            return have_childs(applicationId, new List<Guid>() { nodeId }).Count > 0;
        }

        public static List<Guid> get_related_node_ids(Guid applicationId, Guid nodeId, Guid? nodeTypeId, 
            string searchText, bool? inRelations, bool? outRelations,
            bool? inTagRelations = null, bool? outTagRelations = null, int? count = null, int? lowerBoundary = null)
        {
            return DBConnector.get_guid_list(applicationId, GetFullyQualifiedName("GetRelatedNodeIDs"),
                applicationId, nodeId, nodeTypeId, ProviderUtil.get_search_text(searchText),
                inRelations, outRelations, inTagRelations, outTagRelations, count, lowerBoundary);
        }
        
        public static List<Node> get_related_nodes(Guid applicationId, Guid nodeId, Guid? nodeTypeId, 
            string searchText, bool? inRelations, bool? outRelations,
            bool? inTagRelations = null, bool? outTagRelations = null, int? count = null, int? lowerBoundary = null)
        {
            if (string.IsNullOrEmpty(searchText)) searchText = null;

            return CNParsers.nodes(DBConnector.read(applicationId, GetFullyQualifiedName("GetRelatedNodes"),
                applicationId, nodeId, nodeTypeId, ProviderUtil.get_search_text(searchText), 
                inRelations, outRelations, inTagRelations, outTagRelations, count, lowerBoundary));
        }

        public static List<NodesCount> get_related_nodes_count(Guid applicationId, Guid nodeId, Guid? nodeTypeId,
            string searchText, bool? inRelations, bool? outRelations,
            bool? inTagRelations = null, bool? outTagRelations = null, int? count = null, int? lowerBoundary = null)
        {
            if (string.IsNullOrEmpty(searchText)) searchText = null;

            return CNParsers.nodes_count(DBConnector.read(applicationId, GetFullyQualifiedName("GetRelatedNodesCount"),
                applicationId, nodeId, nodeTypeId, ProviderUtil.get_search_text(searchText), 
                inRelations, outRelations, inTagRelations, outTagRelations, count, lowerBoundary));
        }

        public static List<Node> get_related_nodes_partitioned(Guid applicationId, Guid nodeId, List<Guid> nodeTypeIds,
            bool? inRelations, bool? outRelations, bool? inTagRelations = null, bool? outTagRelations = null, int? count = null)
        {
            return CNParsers.nodes(DBConnector.read(applicationId, GetFullyQualifiedName("GetRelatedNodesPartitioned"),
                applicationId, nodeId, string.Join(",", nodeTypeIds), ',',
                inRelations, outRelations, inTagRelations, outTagRelations, count));
        }

        public static bool relation_exists(Guid applicationId, 
            Guid sourceNodeId, Guid destinationNodeId, bool? reverseAlso = null)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("RelationExists"),
                applicationId, sourceNodeId, destinationNodeId, reverseAlso);
        }
        
        public static bool add_member(Guid applicationId, NodeMember info, ref List<Dashboard> retDashboards)
        {
            bool isPending = info.Status == NodeMemberStatuses.Pending.ToString() ? true : false;

            return DBConnector.succeed(applicationId, ref retDashboards, GetFullyQualifiedName("AddMember"),
                applicationId, info.Node.NodeID, info.Member.UserID, info.MembershipDate, info.IsAdmin, isPending,
                    info.AcceptionDate, info.Position);
        }

        public static bool remove_member(Guid applicationId, Guid nodeId, Guid userId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ArithmeticDeleteMember"),
                applicationId, nodeId, userId);
        }

        public static bool accept_member(Guid applicationId, Guid nodeId, Guid userId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("AcceptMember"),
                applicationId, nodeId, userId, DateTime.Now);
        }

        public static bool set_member_position(Guid applicationId, Guid nodeId, Guid userId, string position)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetMemberPosition"),
                applicationId, nodeId, userId, position);
        }

        public static bool is_node_creator(Guid applicationId, Guid? nodeId, Guid? nodeTypeId, string additionalId, Guid userId)
        {
            if (string.IsNullOrEmpty(additionalId)) additionalId = null;

            return DBConnector.succeed(applicationId, GetFullyQualifiedName("IsNodeCreator"),
                applicationId, nodeId, nodeTypeId, additionalId, userId);
        }

        public static bool is_node_creator(Guid applicationId, Guid nodeId, Guid userId)
        {
            return is_node_creator(applicationId, nodeId, null, null, userId);
        }

        public static bool is_node_creator(Guid applicationId, Guid? nodeTypeId, string additionalId, Guid userId)
        {
            return is_node_creator(applicationId, null, nodeTypeId, additionalId, userId);
        }

        public static bool is_node_member(Guid applicationId, 
            Guid nodeId, Guid userId, bool? isAdmin = null, NodeMemberStatuses? status = null)
        {
            string strStatus = status.HasValue ? status.ToString() : null;

            return DBConnector.succeed(applicationId, GetFullyQualifiedName("IsNodeMember"),
                applicationId, nodeId, userId, isAdmin, strStatus);
        }

        public static bool has_admin(Guid applicationId, Guid nodeId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("HasAdmin"), applicationId, nodeId);
        }

        public static bool set_unset_node_admin(Guid applicationId, 
            Guid nodeId, Guid userId, bool admin, bool unique = false)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetUnsetNodeAdmin"),
                applicationId, nodeId, userId, admin, unique);
        }

        public static bool is_admin_member(Guid applicationId, Guid nodeId, Guid userId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("IsNodeAdmin"), applicationId, nodeId, userId);
        }

        public static List<Guid> get_complex_admins(Guid applicationId, Guid listIdOrNodeId)
        {
            return DBConnector.get_guid_list(applicationId, GetFullyQualifiedName("GetComplexAdmins"), applicationId, listIdOrNodeId);
        }

        public static bool is_complex_admin(Guid applicationId, Guid nodeId, Guid userId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("IsComplexAdmin"), applicationId, nodeId, userId);
        }

        public static Guid? get_complex_type_id(Guid applicationId, Guid listId)
        {
            return DBConnector.get_guid(applicationId, GetFullyQualifiedName("GetComplexTypeID"), applicationId, listId);
        }

        public static List<HierarchyAdmin> get_area_admins(Guid applicationId, 
            Guid nodeId, ref Node node, Service service = null)
        {
            if (service == null) service = get_service(applicationId, 
                node != null && node.NodeTypeID.HasValue ? node.NodeTypeID.Value : nodeId);
            if (service == null) return new List<HierarchyAdmin>();

            switch (service.AdminType)
            {
                case ServiceAdminType.AreaAdmin:
                case ServiceAdminType.ComplexAdmin:
                    if (node == null) node = get_node(applicationId, nodeId);

                    if (node == null || !node.AdminAreaID.HasValue)
                        return new List<HierarchyAdmin>();
                    else if (service.AdminType == ServiceAdminType.ComplexAdmin)
                        return !node.AdminAreaID.HasValue ? new List<HierarchyAdmin>() : 
                            get_complex_admins(applicationId, node.AdminAreaID.Value)
                            .Select(u => new HierarchyAdmin() { User = new User() { UserID = u }, Level = 0 }).ToList();
                    else
                        return !node.AdminAreaID.HasValue ? new List<HierarchyAdmin>() : 
                            get_hierarchy_admins(applicationId, node.AdminAreaID.Value).Where(
                            u => !service.MaxAcceptableAdminLevel.HasValue || service.MaxAcceptableAdminLevel < 0 ||
                                u.Level <= service.MaxAcceptableAdminLevel).Select(u => new HierarchyAdmin()
                                {
                                    User = new User() { UserID = u.User.UserID },
                                    Level = u.Level
                                }).ToList();
                case ServiceAdminType.SpecificNode:
                    return !service.AdminNode.NodeID.HasValue ? new List<HierarchyAdmin>() :
                        get_members(applicationId, service.AdminNode.NodeID.Value,
                            pending: false, admin: true).Select(u => new HierarchyAdmin()
                            {
                                User = new User() { UserID = u.Member.UserID },
                                Level = 0
                            }).ToList();
                case ServiceAdminType.Registerer:
                    if (node == null) node = get_node(applicationId, nodeId);
                    List<Guid> retList = new List<Guid>();
                    if (node != null && node.Creator.UserID.HasValue) retList.Add(node.Creator.UserID.Value);
                    return retList.Select(u => new HierarchyAdmin() { User = new User() { UserID = u }, Level = 0 }).ToList();
            }

            return new List<HierarchyAdmin>();
        }

        public static List<HierarchyAdmin> get_node_admins(Guid applicationId, 
            Guid nodeId, Node nodeObject = null, Service service = null)
        {
            if (nodeObject == null && (nodeObject = get_node(applicationId, nodeId)) == null)
                nodeObject = new Node();
            if (service == null && (service = get_service(applicationId, 
                nodeObject != null && nodeObject.NodeTypeID.HasValue ? nodeObject.NodeTypeID.Value : nodeId)) == null)
                service = new Service();
            
            List<HierarchyAdmin> admins = new List<HierarchyAdmin>();

            switch (service.AdminType)
            {
                case ServiceAdminType.AreaAdmin:
                    if (nodeObject.AdminAreaID.HasValue)
                        admins = get_area_admins(applicationId, nodeId, ref nodeObject);
                    break;
                case ServiceAdminType.ComplexAdmin:
                    if (nodeObject.AdminAreaID.HasValue)
                        admins = get_complex_admins(applicationId, nodeObject.AdminAreaID.Value)
                            .Select(u => new HierarchyAdmin() { User = new User() { UserID = u }, Level = 0 }).ToList(); 
                    break;
                case ServiceAdminType.SpecificNode:
                    if (service.AdminNode.NodeID.HasValue)
                    {
                        admins = get_members(applicationId, service.AdminNode.NodeID.Value,
                            pending: false, admin: true).Select(u => new HierarchyAdmin()
                            {
                                User = new User() { UserID = u.Member.UserID },
                                Level = 0
                            }).ToList();
                    }
                    break;
                case ServiceAdminType.Registerer:
                    if (nodeObject.Creator.UserID.HasValue) admins.Add(new HierarchyAdmin()
                    {
                        User = new User() { UserID = nodeObject.Creator.UserID },
                        Level = 0
                    });
                    break;
            }

            return admins;
        }

        public static bool is_node_admin(Guid applicationId, Guid userId, Guid nodeId, Guid? nodeTypeId, 
            Guid? areaId, bool? isCreator, Service service = null, List<NodeCreator> contributors = null)
        {
            if (service == null)
                service = get_service(applicationId, nodeTypeId.HasValue ? nodeTypeId.Value : nodeId);
            if (service == null) service = new Service();

            if (!areaId.HasValue && 
                (service.AdminType == ServiceAdminType.AreaAdmin || service.AdminType == ServiceAdminType.ComplexAdmin))
            {
                Node nd = get_node(applicationId, nodeId);
                if (nd != null) areaId = nd.AdminAreaID;
            }
            
            switch (service.AdminType)
            {
                case ServiceAdminType.AreaAdmin:
                    if (!areaId.HasValue) return false;
                    else
                    {
                        if (contributors == null) contributors = get_node_creators(applicationId, nodeId);

                        List<Guid> contributorIds = contributors
                            .Where(c => c.User.UserID.HasValue).Select(u => u.User.UserID.Value).ToList();
                        int maxLevel = !service.MaxAcceptableAdminLevel.HasValue ? 0 : service.MaxAcceptableAdminLevel.Value;

                        return service.MaxAcceptableAdminLevel.HasValue ? 
                            is_hierarchy_admin(applicationId, areaId.Value, userId, contributorIds, maxLevel) :
                            is_hierarchy_admin(applicationId, areaId.Value, userId, contributorIds);
                    }
                case ServiceAdminType.ComplexAdmin:
                    return !areaId.HasValue ? false : is_complex_admin(applicationId, areaId.Value, userId);
                case ServiceAdminType.SpecificNode:
                    return !service.AdminNode.NodeID.HasValue ? false :
                        is_admin_member(applicationId, service.AdminNode.NodeID.Value, userId);
                case ServiceAdminType.Registerer:
                    return isCreator.HasValue ? isCreator.Value : 
                        is_node_creator(applicationId, nodeId, userId);
            }

            return false;
        }

        public static bool get_user2node_status(Guid applicationId, Guid userId, Guid nodeId, 
            ref bool isCreator, ref bool isContributor, ref bool isExpert, ref bool isMember, ref bool isAdminMember, 
            ref bool isServiceAdmin, ref bool isAreaAdmin, ref bool editable, ref bool noOptionSet, 
            ref bool editSuggestion, Service service = null, List<NodeCreator> contributors = null)
        {
            //noOptionSet: All edit permissions are not set

            Guid? nodeTypeId = null;
            Guid? areaId = null;

            DBResultSet results = DBConnector.read(applicationId, GetFullyQualifiedName("GetUser2NodeStatus"), 
                applicationId, userId, nodeId);

            CNParsers.user2node_status(results, ref nodeTypeId, ref areaId, ref isCreator, ref isContributor,
                ref isExpert, ref isMember, ref isAdminMember, ref isServiceAdmin);

            if (!nodeTypeId.HasValue) return false;

            if (service == null) service = CNController.get_service(applicationId, nodeTypeId.Value);
            if (service == null) service = new Service();

            isAreaAdmin = CNController.is_node_admin(applicationId, userId, nodeId, nodeTypeId.Value, 
                areaId, isCreator, service, contributors);

            noOptionSet = !service.EditableForAdmin.HasValue && !service.EditableForCreator.HasValue && 
                !service.EditableForContributors.HasValue && !service.EditableForExperts.HasValue && 
                !service.EditableForMembers.HasValue;

            if (!service.EditableForAdmin.HasValue) service.EditableForAdmin = false;
            if (!service.EditableForCreator.HasValue) service.EditableForCreator = false;
            if (!service.EditableForContributors.HasValue) service.EditableForContributors = false;
            if (!service.EditableForExperts.HasValue) service.EditableForExperts = false;
            if (!service.EditableForMembers.HasValue) service.EditableForMembers = false;

            editable = PublicMethods.is_system_admin(applicationId, userId) || isServiceAdmin || 
                (isAreaAdmin && service.EditableForAdmin.Value) || (isCreator && service.EditableForCreator.Value) || 
                (isContributor && service.EditableForContributors.Value) || 
                (isExpert && service.EditableForExperts.Value) || (isMember && service.EditableForMembers.Value);

            editSuggestion = !service.EditSuggestion.HasValue || service.EditSuggestion.Value;

            return true;
        }

        public static bool get_user2node_status(Guid applicationId, Guid userId, Guid nodeId, 
            ref bool isCreator, ref bool isContributor, ref bool isExpert, ref bool isMember, 
            ref bool isAdminMember, ref bool isServiceAdmin, ref bool isAreaAdmin, 
            ref bool editable, Service service = null, List<NodeCreator> contributors = null)
        {
            bool noOptionSet = false;
            bool editSuggestion = false;

            return get_user2node_status(applicationId, userId, nodeId, ref isCreator, ref isContributor,
                ref isExpert, ref isMember, ref isAdminMember, ref isServiceAdmin, ref isAreaAdmin,
                ref editable, ref noOptionSet, ref editSuggestion, service, contributors);
        }

        public static bool has_edit_permission(Guid applicationId, 
            Guid userId, Guid nodeId, bool defaultPermissionForExperts, ref bool editSuggestion)
        {
            bool isCreator = false, isContributor = false, isExpert = false, isMember = false, isAdminMember = false,
                isServiceAdmin = false, isAreaAdmin = false, editable = false, noOptionSet = false;

            get_user2node_status(applicationId, userId, nodeId, ref isCreator, ref isContributor, ref isExpert, 
                ref isMember, ref isAdminMember, ref isServiceAdmin, ref isAreaAdmin, ref editable,
                ref noOptionSet, ref editSuggestion);

            return editable || (noOptionSet && defaultPermissionForExperts && isExpert) ||
                PrivacyController.check_access(applicationId, userId, nodeId, PrivacyObjectType.Node, PermissionType.Modify);
        }

        public static bool has_edit_permission(Guid applicationId, 
            Guid userId, Guid nodeId, bool defaultPermissionForExperts)
        {
            bool editSuggestion = false;
            return has_edit_permission(applicationId, 
                userId, nodeId, defaultPermissionForExperts, ref editSuggestion);
        }

        public static bool has_edit_permission(Guid applicationId, Guid userId, Guid nodeId)
        {
            return has_edit_permission(applicationId, userId, nodeId, false);
        }

        public static List<Guid> get_users_with_edit_permission(Guid applicationId, 
            Guid nodeId, bool defaultPermissionForExperts, ref bool editSuggestion)
        {
            List<Guid> retList = new List<Guid>();

            Service service = get_service(applicationId, nodeId);

            editSuggestion = service == null || !service.EditSuggestion.HasValue || service.EditSuggestion.Value;

            if (defaultPermissionForExperts && (
                    service == null || (
                        !service.EditableForAdmin.HasValue && !service.EditableForCreator.HasValue &&
                        !service.EditableForContributors.HasValue && !service.EditableForExperts.HasValue &&
                        !service.EditableForMembers.HasValue
                    )
                )
            )
            {
                return get_experts(applicationId, nodeId)
                    .Where(u => u.User.UserID.HasValue)
                    .Select(u => u.User.UserID.Value).ToList();
            }
            else if (service == null) return new List<Guid>();

            Node node = null;

            if (service.EditableForAdmin.HasValue && service.EditableForAdmin.Value)
                get_area_admins(applicationId, nodeId, ref node, service);

            if (service.EditableForContributors.HasValue && service.EditableForContributors.Value)
                retList.AddRange(get_node_creators(applicationId, nodeId).Select(u => u.User.UserID.Value));

            if (service.EditableForCreator.HasValue && service.EditableForCreator.Value)
                retList.Add(get_node(applicationId, nodeId).Creator.UserID.Value);

            if (service.EditableForExperts.HasValue && service.EditableForExperts.Value)
                retList.AddRange(get_experts(applicationId, nodeId).Select(u => u.User.UserID.Value));

            if (service.EditableForMembers.HasValue && service.EditableForMembers.Value)
                retList.AddRange(get_members(applicationId, nodeId, pending: false, admin: null)
                    .Select(u => u.Member.UserID.Value));

            return retList.Distinct().ToList();
        }

        public static List<Guid> get_users_with_edit_permission(Guid applicationId, 
            Guid nodeId, bool defaultPermissionForExperts)
        {
            bool editSuggestion = false;
            return get_users_with_edit_permission(applicationId,
                nodeId, defaultPermissionForExperts, ref editSuggestion);
        }

        public static List<Guid> get_users_with_edit_permission(Guid applicationId, Guid nodeId)
        {
            return get_users_with_edit_permission(applicationId, nodeId, false);
        }

        public static List<Hierarchy> get_node_hierarchy(Guid applicationId, Guid nodeId, bool? sameType = true)
        {
            return DBConnector.get_hierarchy(applicationId, GetFullyQualifiedName("GetNodeHierarchy"),
                applicationId, nodeId, sameType);
        }

        public static List<Hierarchy> get_node_types_hierarchy(Guid applicationId, List<Guid> nodeTypeIds)
        {
            return DBConnector.get_hierarchy(applicationId, GetFullyQualifiedName("GetNodeTypesHierarchy"),
                applicationId, ProviderUtil.list_to_string<Guid>(nodeTypeIds), ',');
        }

        public static List<Hierarchy> get_node_type_hierarchy(Guid applicationId, Guid nodeTypeId)
        {
            return get_node_types_hierarchy(applicationId, new List<Guid>() { nodeTypeId });
        }

        public static int get_tree_depth(Guid applicationId, Guid nodeTypeId)
        {
            return DBConnector.get_int(applicationId, GetFullyQualifiedName("GetTreeDepth"), applicationId, nodeTypeId);
        }

        public static List<HierarchyAdmin> get_hierarchy_admins(Guid applicationId, Guid nodeId, bool? sameType = true)
        {
            return CNParsers.hierarchy_admins(DBConnector.read(applicationId, GetFullyQualifiedName("GetNodeHierarchyAdminIDs"),
                applicationId, nodeId, sameType));
        }

        public static bool is_hierarchy_admin(Guid applicationId, 
            Guid nodeId, Guid userId, List<Guid> contributorUserIds, int maxLevel = 2)
        {
            List<HierarchyAdmin> admins = get_hierarchy_admins(applicationId, nodeId).Where(
                u => maxLevel < 0 || u.Level <= maxLevel).OrderBy(v => v.Level).ToList();

            if (admins.Count == 0) return false;

            if (admins.Any(u => u.User.UserID == userId) && contributorUserIds.Any(u => u == userId) &&
                admins.Any(u => u.User.UserID != userId && !contributorUserIds.Any(x => x == u.User.UserID)))
                return false;

            for (int i = 0, lnt = admins.Count; i < lnt; ++i)
                if (admins[i].User.UserID == userId && !contributorUserIds.Any(u => u == userId)) return true;

            return admins.Any(u => u.User.UserID == userId);
        }

        public static bool is_hierarchy_admin(Guid applicationId, Guid nodeId, Guid userId, int maxLevel = 2)
        {
            return is_hierarchy_admin(applicationId, nodeId, userId, maxLevel);
        }

        public static List<NodeMember> get_members(Guid applicationId, List<Guid> nodeIds, 
            bool? pending, bool? admin, string searchText, int? count, long? lowerBoundary, ref long totalCount)
        {
            if (nodeIds == null || nodeIds.Count == 0) return new List<NodeMember>();

            DBResultSet results = DBConnector.read(applicationId, GetFullyQualifiedName("GetMembers"),
                applicationId, ProviderUtil.list_to_string<Guid>(nodeIds), ',',
                pending, admin, ProviderUtil.get_search_text(searchText), count, lowerBoundary);

            return CNParsers.node_members(results, parseNode: false, parseUser: true, totalCount: ref totalCount);
        }

        public static List<NodeMember> get_members(Guid applicationId, List<Guid> nodeIds, bool? pending, bool? admin)
        {
            long totalCount = 0;
            return get_members(applicationId, nodeIds, pending, admin, null, null, null, ref totalCount);
        }

        public static List<NodeMember> get_members(Guid applicationId, 
            Guid nodeId, bool? pending, bool? admin, string searchText, int? count, long? lowerBoundary, ref long totalCount)
        {
            return get_members(applicationId, new List<Guid>() { nodeId },
                pending, admin, searchText, count, lowerBoundary, ref totalCount);
        }

        public static List<NodeMember> get_members(Guid applicationId, Guid nodeId, bool? pending, bool? admin)
        {
            long totalCount = 0;
            return get_members(applicationId, nodeId, pending, admin, null, null, null, ref totalCount);
        }

        public static NodeMember get_member(Guid applicationId, Guid nodeId, Guid userId)
        {
            DBResultSet results = DBConnector.read(applicationId, GetFullyQualifiedName("GetMember"),
                applicationId, nodeId, userId);

            return CNParsers.node_members(results, parseNode: false, parseUser: true).FirstOrDefault();
        }

        public static List<Guid> get_member_user_ids(Guid applicationId, 
            List<Guid> nodeIds, NodeMemberStatuses? status = null, bool? admin = null)
        {
            string strStatus = status.ToString();
            if (string.IsNullOrEmpty(strStatus)) strStatus = null;

            return DBConnector.get_guid_list(applicationId, GetFullyQualifiedName("GetMemberUserIDs"),
                applicationId, ProviderUtil.list_to_string<Guid>(nodeIds), ',', strStatus, admin);
        }

        public static List<Guid> get_member_user_ids(Guid applicationId, 
            Guid nodeId, NodeMemberStatuses? status = null, bool? admin = null)
        {
            return get_member_user_ids(applicationId, new List<Guid>() { nodeId }, status, admin);
        }

        public static int get_members_count(Guid applicationId, Guid nodeId, NodeMemberStatuses? status = null, bool? admin = null)
        {
            string strStatus = status.ToString();
            if (string.IsNullOrEmpty(strStatus)) strStatus = null;

            return DBConnector.get_int(applicationId, GetFullyQualifiedName("GetMembersCount"),
                applicationId, nodeId, strStatus, admin);
        }

        public static List<NodesCount> get_membership_domains_count(Guid applicationId, Guid userId, Guid? nodeTypeId,
            Guid? nodeId, string additionalId, DateTime? lowerDateLimit, DateTime? upperDateLimit)
        {
            return CNParsers.nodes_count(DBConnector.read(applicationId, GetFullyQualifiedName("GetMembershipDomainsCount"),
                applicationId, userId, nodeTypeId, nodeId, additionalId, lowerDateLimit, upperDateLimit));
        }

        public static List<Node> get_membership_domains(Guid applicationId, Guid userId, List<Guid> nodeTypeIds, 
            Guid? nodeId, string additionalId, string searchText, DateTime? lowerDateLimit, 
            DateTime? upperDateLimit, int? lowerBoundary, int? count, ref long totalCount)
        {
            DBResultSet results = DBConnector.read(applicationId, GetFullyQualifiedName("GetMembershipDomains"),
                applicationId, userId, string.Join(",", nodeTypeIds), ',', nodeId, additionalId, 
                ProviderUtil.get_search_text(searchText), lowerDateLimit, upperDateLimit, lowerBoundary, count);

            return CNParsers.nodes(results, full: null, totalCount: ref totalCount);
        }

        private static List<NodeMember> _get_member_nodes(Guid applicationId, List<Guid> userIds, 
            ref List<Guid> nodeTypeIds, NodeTypes? nodeType, bool? admin = null)
        {
            string additionalTypeId = null;
            if (nodeType.HasValue) additionalTypeId = CNUtilities.get_node_type_additional_id(nodeType.Value).ToString();

            DBResultSet results = DBConnector.read(applicationId, GetFullyQualifiedName("GetMemberNodes"),
                applicationId, ProviderUtil.list_to_string<Guid>(userIds),
                ProviderUtil.list_to_string<Guid>(nodeTypeIds), ',', additionalTypeId, admin);

            return CNParsers.node_members(results, parseNode: true, parseUser: false);
        }

        public static List<NodeMember> get_member_nodes(Guid applicationId, 
            Guid userId, ref List<Guid> nodeTypeIds, bool? admin = null)
        {
            List<Guid> userIds = new List<Guid>();
            userIds.Add(userId);
            return _get_member_nodes(applicationId, userIds, ref nodeTypeIds, null, admin);
        }

        public static List<NodeMember> get_member_nodes(Guid applicationId, 
            Guid userId, Guid nodeTypeId, bool? admin = null)
        {
            List<Guid> lst = new List<Guid>();
            lst.Add(nodeTypeId);
            List<Guid> userIds = new List<Guid>();
            userIds.Add(userId);
            return _get_member_nodes(applicationId, userIds, ref lst, null, admin);
        }

        public static List<NodeMember> get_member_nodes(Guid applicationId, 
            Guid userId, NodeTypes nodeType, bool? admin = null)
        {
            List<Guid> lst = new List<Guid>();
            List<Guid> userIds = new List<Guid>();
            userIds.Add(userId);
            return _get_member_nodes(applicationId, userIds, ref lst, nodeType, admin);
        }

        public static List<NodeMember> get_member_nodes(Guid applicationId, Guid userId, bool? admin = null)
        {
            List<Guid> lst = new List<Guid>();
            List<Guid> userIds = new List<Guid>();
            userIds.Add(userId);
            return _get_member_nodes(applicationId, userIds, ref lst, null, admin);
        }

        public static List<NodeMember> get_member_nodes(Guid applicationId, 
            List<Guid> userIds, List<Guid> nodeTypeIds, bool? admin = null)
        {
            return _get_member_nodes(applicationId, userIds, ref nodeTypeIds, null, admin);
        }

        public static List<Guid> get_child_hierarchy_member_ids(Guid applicationId,
            Guid nodeId, string searchText, int? count, long? lowerBoundary, ref long totalCount)
        {
            return DBConnector.get_guid_list(applicationId, ref totalCount, GetFullyQualifiedName("GetChildHierarchyMemberIDs"),
                applicationId, nodeId, ProviderUtil.get_search_text(searchText), count, lowerBoundary);
        }

        public static List<Guid> get_child_hierarchy_expert_ids(Guid applicationId,
            Guid nodeId, string searchText, int? count, long? lowerBoundary, ref long totalCount)
        {
            return DBConnector.get_guid_list(applicationId, ref totalCount, GetFullyQualifiedName("GetChildHierarchyExpertIDs"),
                applicationId, nodeId, ProviderUtil.get_search_text(searchText), count, lowerBoundary);
        }

        public static List<NodeMember> get_users_departments(Guid applicationId, List<Guid> userIds)
        {
            DBResultSet results = DBConnector.read(applicationId, GetFullyQualifiedName("GetUsersDepartments"),
                applicationId, ProviderUtil.list_to_string<Guid>(userIds), ',');

            return CNParsers.node_members(results, parseNode: true, parseUser: false);
        }

        public static NodeMember get_user_department(Guid applicationId, Guid userId)
        {
            List<Guid> _uIds = new List<Guid>();
            _uIds.Add(userId);
            return get_users_departments(applicationId, _uIds).FirstOrDefault();
        }

        public static bool like_nodes(Guid applicationId, List<Guid> nodeIds, Guid userId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("LikeNodes"),
                applicationId, ProviderUtil.list_to_string<Guid>(nodeIds), ',', userId, DateTime.Now);
        }

        public static bool like_node(Guid applicationId, Guid nodeId, Guid userId)
        {
            return like_nodes(applicationId, new List<Guid>() { nodeId }, userId);
        }

        public static bool unlike_nodes(Guid applicationId, List<Guid> nodeIds, Guid userId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("UnlikeNodes"),
                applicationId, ProviderUtil.list_to_string<Guid>(nodeIds), ',', userId);
        }

        public static bool unlike_node(Guid applicationId, Guid nodeId, Guid userId)
        {
            return unlike_nodes(applicationId, new List<Guid>() { nodeId }, userId);
        }

        public static List<Guid> is_fan(Guid applicationId, List<Guid> nodeIds, Guid userId)
        {
            return DBConnector.get_guid_list(applicationId, GetFullyQualifiedName("IsFan"),
                applicationId, ProviderUtil.list_to_string<Guid>(nodeIds), ',', userId);
        }

        public static bool is_fan(Guid applicationId, Guid nodeId, Guid userId)
        {
            return is_fan(applicationId, new List<Guid>() { nodeId }, userId).Count > 0;
        }

        public static List<Guid> get_node_fans_user_ids(Guid applicationId, Guid nodeId, 
            int? count, long? lowerBoundary, ref long totalCount)
        {
            DBResultSet results = DBConnector.read(applicationId, GetFullyQualifiedName("GetNodeFansUserIDs"),
                applicationId, nodeId, count, lowerBoundary);

            return CNParsers.fan_user_ids(results, ref totalCount);
        }

        public static List<Guid> get_node_fans_user_ids(Guid applicationId, 
            Guid nodeId, int? count = null, long? lowerBoundary = null)
        {
            long totalCount = 0;
            return get_node_fans_user_ids(applicationId, nodeId, count, lowerBoundary, ref totalCount);
        }

        public static List<User> get_node_fans(Guid applicationId, 
            Guid nodeId, int? count, long? lowerBoundary, ref long totalCount)
        {
            return UsersController.get_users(applicationId, get_node_fans_user_ids(applicationId,
                nodeId, count, lowerBoundary, ref totalCount));
        }

        public static List<NodesCount> get_favorite_nodes_count(Guid applicationId, Guid userId, Guid? nodeTypeId,
            Guid? nodeId, string additionalId, bool? isDocument, DateTime? lowerDateLimit, DateTime? upperDateLimit)
        {
            return CNParsers.nodes_count(DBConnector.read(applicationId, GetFullyQualifiedName("GetFavoriteNodesCount"),
                applicationId, userId, nodeTypeId, nodeId, additionalId, isDocument, lowerDateLimit, upperDateLimit));
        }

        public static NodesCount get_favorite_nodes_count(Guid applicationId, Guid userId, Guid nodeTypeId,
            bool? isDocument, DateTime? lowerDateLimit, DateTime? upperDateLimit)
        {
            return get_favorite_nodes_count(applicationId,
                userId, nodeTypeId, null, null, isDocument, lowerDateLimit, upperDateLimit).FirstOrDefault();
        }

        public static List<Node> get_favorite_nodes(Guid applicationId, Guid userId, List<Guid> nodeTypeIds, 
            Guid? nodeId, string additionalId, string searchText, bool? isDocument, DateTime? lowerDateLimit,
            DateTime? upperDateLimit, int? lowerBoundary, int? count, ref long totalCount)
        {
            if (nodeTypeIds == null) nodeTypeIds = new List<Guid>();

            DBResultSet results = DBConnector.read(applicationId, GetFullyQualifiedName("GetFavoriteNodes"),
                applicationId, userId, ProviderUtil.list_to_string<Guid>(nodeTypeIds), ',', 
                nodeId, additionalId, ProviderUtil.get_search_text(searchText), isDocument, 
                lowerDateLimit, upperDateLimit, lowerBoundary, count);

            return CNParsers.nodes(results, full: false, totalCount: ref totalCount);
        }

        public static bool add_complex(Guid applicationId, NodeList info)
        {
            if (!info.CreationDate.HasValue) info.CreationDate = DateTime.Now;

            return DBConnector.succeed(applicationId, GetFullyQualifiedName("AddComplex"),
                applicationId, info.ListID, info.NodeTypeID, info.Name, info.Description, info.CreatorUserID, 
                DateTime.Now, info.ParentListID, info.OwnerID, info.OwnerType);
        }

        public static bool modify_complex(Guid applicationId, NodeList info)
        {
            if (!info.LastModificationDate.HasValue) info.LastModificationDate = DateTime.Now;

            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ModifyComplex"),
                applicationId, info.ListID, info.Name, info.Description, info.LastModifierUserID, DateTime.Now);
        }

        public static bool remove_complexes(Guid applicationId, List<Guid> listIds, Guid lastModifierUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ArithmeticDeleteComplexes"),
                applicationId, ProviderUtil.list_to_string<Guid>(listIds), ',', lastModifierUserId, DateTime.Now);
        }

        public static bool remove_complex(Guid applicationId, Guid listId, Guid lastModifierUserId)
        {
            return remove_complexes(applicationId, new List<Guid>() { listId }, lastModifierUserId);
        }

        public static bool add_complex_admin(Guid applicationId, Guid listId, Guid userId, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("AddComplexAdmin"),
                applicationId, listId, userId, currentUserId, DateTime.Now);
        }

        public static bool remove_complex_admin(Guid applicationId, Guid listId, Guid userId, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("RemoveComplexAdmin"),
                applicationId, listId, userId, currentUserId, DateTime.Now);
        }

        public static List<NodeList> get_lists(Guid applicationId, List<Guid> listIds)
        {
            return CNParsers.lists(DBConnector.read(applicationId, GetFullyQualifiedName("GetListsByIDs"),
                applicationId, ProviderUtil.list_to_string<Guid>(listIds), ','));
        }

        public static NodeList get_list(Guid applicationId, Guid listId)
        {
            return get_lists(applicationId, new List<Guid>() { listId }).FirstOrDefault();
        }
        
        private static List<NodeList> _get_lists(Guid applicationId, 
            Guid? nodeTypeId, NodeTypes? nodeType, string searchText, Guid? minId, int? count)
        {
            string strNodeTypeAdditionalId = null;
            if (nodeType.HasValue) strNodeTypeAdditionalId = CNUtilities.get_node_type_additional_id(nodeType.Value).ToString();

            return CNParsers.lists(DBConnector.read(applicationId, GetFullyQualifiedName("GetLists"),
                applicationId, nodeTypeId, strNodeTypeAdditionalId, ProviderUtil.get_search_text(searchText), count, minId));
        }

        public static List<NodeList> get_lists(Guid applicationId, 
            Guid nodeTypeId, string searchText = null, Guid? minId = null, int? count = 1000)
        {
            return _get_lists(applicationId, nodeTypeId, null, searchText, minId, count);
        }

        public static List<NodeList> get_lists(Guid applicationId, 
            NodeTypes nodeType, string searchText = null, Guid? minId = null, int count = 1000)
        {
            return _get_lists(applicationId, null, nodeType, searchText, minId, count);
        }

        public static bool add_nodes_to_complex(Guid applicationId, Guid listId, List<Guid> nodeIds, Guid creatorUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("AddNodesToComplex"),
                applicationId, listId, ProviderUtil.list_to_string<Guid>(nodeIds), ',', creatorUserId, DateTime.Now);
        }

        public static bool add_node_to_complex(Guid applicationId, Guid listId, Guid nodeId, Guid creatorUserId)
        {
            return add_nodes_to_complex(applicationId, listId, new List<Guid>() { nodeId }, creatorUserId);
        }

        public static bool remove_complex_nodes(Guid applicationId, Guid listId, List<Guid> nodeIds, Guid lastModifierUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ArithmeticDeleteComplexNodes"),
                applicationId, listId, ProviderUtil.list_to_string<Guid>(nodeIds), ',', lastModifierUserId, DateTime.Now);
        }

        public static bool remove_complex_node(Guid applicationId, Guid listId, Guid nodeId, Guid lastModifierUserId)
        {
            return remove_complex_nodes(applicationId, listId, new List<Guid>() { nodeId }, lastModifierUserId);
        }

        private static List<Node> _get_list_nodes(Guid applicationId, Guid listId, Guid? nodeTypeId, NodeTypes? nodeType)
        {
            string strNodeTypeAdditionalId = null;
            if (nodeType.HasValue) strNodeTypeAdditionalId = CNUtilities.get_node_type_additional_id(nodeType.Value).ToString();

            return CNParsers.nodes(DBConnector.read(applicationId, GetFullyQualifiedName("GetListNodes"),
                applicationId, listId, nodeTypeId, strNodeTypeAdditionalId));
        }

        public static List<Node> get_list_nodes(Guid applicationId, Guid listId)
        {
            return _get_list_nodes(applicationId, listId, null, null);
        }

        public static List<Node> get_list_nodes(Guid applicationId, Guid listId, Guid nodeTypeId)
        {
            return _get_list_nodes(applicationId, listId, nodeTypeId, null);
        }

        public static List<Node> get_list_nodes(Guid applicationId, Guid listId, NodeTypes nodeType)
        {
            return _get_list_nodes(applicationId, listId, null, nodeType);
        }
        
        public static Guid? add_tags(Guid applicationId, List<Tag> tags, Guid? currentUserId)
        {
            if (tags == null) tags = new List<Tag>();
            
            DBCompositeType<StringTableType> tagsList = new DBCompositeType<StringTableType>()
                .add(tags.Select(t => new StringTableType(t.Text)).ToList());

            return DBConnector.get_guid(applicationId, GetFullyQualifiedName("AddTags"),
                applicationId, tagsList, currentUserId, DateTime.Now);
        }

        public static Guid? add_tag(Guid applicationId, Tag info)
        {
            return info == null ? null : add_tags(applicationId, new List<Tag>() { info }, info.CreatorUserID);
        }

        public static List<Tag> search_tags(Guid applicationId, string searchText, int? count = null, int? lowerBoundary = null)
        {
            return CNParsers.tags(DBConnector.read(applicationId, GetFullyQualifiedName("SearchTags"),
                applicationId, ProviderUtil.get_search_text(searchText), count, lowerBoundary));
        }

        public static List<NodeCreator> get_node_creators(Guid applicationId, Guid nodeId, bool? full = null)
        {
            DBResultSet results = DBConnector.read(applicationId, GetFullyQualifiedName("GetNodeCreators"), 
                applicationId, nodeId, full);

            return CNParsers.node_creators(results, full);
        }

        private static List<Node> _get_creator_nodes(Guid applicationId, Guid userId, Guid? nodeTypeId)
        {
            return CNParsers.nodes(DBConnector.read(applicationId, GetFullyQualifiedName("GetCreatorNodes"),
                applicationId, userId, nodeTypeId));
        }

        public static List<Node> get_creator_nodes(Guid applicationId, Guid userId)
        {
            return _get_creator_nodes(applicationId, userId, null);
        }

        public static List<Node> get_creator_nodes(Guid applicationId, Guid userId, Guid nodeTypeId)
        {
            return _get_creator_nodes(applicationId, userId, nodeTypeId);
        }

        public static bool add_experts(Guid applicationId, Guid nodeId, List<Guid> userIds)
        {
            if (userIds == null || userIds.Count == 0) return false;

            return DBConnector.succeed(applicationId, GetFullyQualifiedName("AddExperts"),
                applicationId, nodeId, ProviderUtil.list_to_string<Guid>(userIds), ',');
        }

        public static bool add_expert(Guid applicationId, Guid nodeId, Guid userId)
        {
            return add_experts(applicationId, nodeId, new List<Guid>() { userId });
        }

        public static bool remove_experts(Guid applicationId, Guid nodeId, List<Guid> userIds)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ArithmeticDeleteExperts"),
                applicationId, nodeId, ProviderUtil.list_to_string<Guid>(userIds), ',');
        }

        public static bool remove_expert(Guid applicationId, Guid nodeId, Guid userId)
        {
            return remove_experts(applicationId, nodeId, new List<Guid>() { userId });
        }

        public static List<Expert> get_experts(Guid applicationId, List<Guid> nodeIds, 
            string searchText, int? count, long? lowerBoundary, ref long totalCount, bool hierarchy = false)
        {
            DBResultSet results = DBConnector.read(applicationId, GetFullyQualifiedName("GetExperts"),
                applicationId, ProviderUtil.list_to_string<Guid>(nodeIds), ',',
                ProviderUtil.get_search_text(searchText), hierarchy, count, lowerBoundary);

            return CNParsers.experts(results, ref totalCount);
        }

        public static List<Expert> get_experts(Guid applicationId, List<Guid> nodeIds, bool hierarchy = false)
        {
            long totalCount = 0;
            return get_experts(applicationId, nodeIds, null, null, null, ref totalCount, hierarchy: hierarchy);
        }

        public static List<Expert> get_experts(Guid applicationId, Guid nodeId, 
            string searchText, int? count, long? lowerBoundary, ref long totalCount, bool hierarchy = false)
        {
            return get_experts(applicationId, new List<Guid>() { nodeId }, 
                searchText, count, lowerBoundary, ref totalCount, hierarchy: hierarchy);
        }

        public static List<Expert> get_experts(Guid applicationId, Guid nodeId, bool hierarchy = false)
        {
            long totalCount = 0;
            return get_experts(applicationId, nodeId, null, null, null, ref totalCount, hierarchy: hierarchy);
        }

        public static List<NodesCount> get_expertise_domains_count(Guid applicationId, Guid userId, Guid? nodeTypeId,
            Guid? nodeId, string additionalId, DateTime? lowerDateLimit, DateTime? upperDateLimit)
        {
            return CNParsers.nodes_count(DBConnector.read(applicationId, GetFullyQualifiedName("GetExpertiseDomainsCount"),
                applicationId, userId, nodeTypeId, nodeId, additionalId, lowerDateLimit, upperDateLimit));
        }

        public static NodesCount get_expertise_domains_count(Guid applicationId, Guid userId, Guid nodeTypeId,
            DateTime? lowerDateLimit, DateTime? upperDateLimit)
        {
            return get_expertise_domains_count(applicationId, 
                userId, nodeTypeId, null, null, lowerDateLimit, upperDateLimit).FirstOrDefault();
        }

        public static List<Node> get_expertise_domains(Guid applicationId, Guid userId, List<Guid> nodeTypeIds, 
            Guid? nodeId, string additionalId, string searchText, DateTime? lowerDateLimit, 
            DateTime? upperDateLimit, int? lowerBoundary, int? count, ref long totalCount)
        {
            DBResultSet results = DBConnector.read(applicationId, GetFullyQualifiedName("GetExpertiseDomains"),
                applicationId, userId, string.Join(",", nodeTypeIds), ',', nodeId, additionalId,
                ProviderUtil.get_search_text(searchText), lowerDateLimit, upperDateLimit, lowerBoundary, count);

            return CNParsers.nodes(results, full: null, totalCount: ref totalCount);
        }

        public static List<Expert> get_expertise_domains(Guid applicationId, List<Guid> userIds, bool? approved, 
            bool? socialApproved, bool? all = null, Guid? nodeTypeId = null)
        {
            return CNParsers.experts(DBConnector.read(applicationId, GetFullyQualifiedName("GetUsersExpertiseDomains"),
                applicationId, ProviderUtil.list_to_string<Guid>(userIds), ',', nodeTypeId, approved, socialApproved, all));
        }

        public static List<Expert> get_expertise_domains(Guid applicationId, Guid userId, bool? approved, 
            bool? socialApproved, bool? all = null, Guid? nodeTypeId = null)
        {
            return get_expertise_domains(applicationId, new List<Guid>() { userId }, approved, socialApproved, all, nodeTypeId);
        }

        public static List<Guid> get_expertise_domain_ids(Guid applicationId, 
            List<Guid> userIds, bool? approved, bool? socialApproved)
        {
            return DBConnector.get_guid_list(applicationId, GetFullyQualifiedName("GetUsersExpertiseDomainIDs"),
                applicationId, ProviderUtil.list_to_string<Guid>(userIds), ',', approved, socialApproved);
        }

        public static List<Guid> get_expertise_domain_ids(Guid applicationId, 
            Guid userId, bool? approved, bool? socialApproved)
        {
            return get_expertise_domain_ids(applicationId, new List<Guid>() { userId }, approved, socialApproved);
        }

        public static bool is_expert(Guid applicationId, Guid userId, Guid nodeId, bool? approved, bool? socialApproved)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("IsExpert"),
                applicationId, userId, nodeId, approved, socialApproved);
        }

        public static int get_experts_count(Guid applicationId, 
            bool? approved, bool? socialApproved, Guid? nodeId = null, bool? distinctUsers = null)
        {
            return DBConnector.get_int(applicationId, GetFullyQualifiedName("GetExpertsCount"),
                applicationId, nodeId, distinctUsers, approved, socialApproved);
        }

        public static bool vote_expertise(Guid applicationId, Guid referrerUserId, Guid nodeId, Guid userId, bool? confirmStatus)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("VoteExpertise"),
                applicationId, referrerUserId, nodeId, userId, confirmStatus, DateTime.Now,
                RaaiVanSettings.CoreNetwork.MinAcceptableExpertiseReferralsCount(applicationId),
                RaaiVanSettings.CoreNetwork.MinAcceptableExpertiseConfirmsPercentage(applicationId));
        }

        public static Guid? i_am_expert(Guid applicationId, Guid userId, string expertiseDomain)
        {
            return DBConnector.get_guid(applicationId, GetFullyQualifiedName("IAmExpert"),
                applicationId, userId, expertiseDomain, DateTime.Now,
                RaaiVanSettings.CoreNetwork.MinAcceptableExpertiseReferralsCount(applicationId),
                RaaiVanSettings.CoreNetwork.MinAcceptableExpertiseConfirmsPercentage(applicationId));
        }

        public static bool i_am_not_expert(Guid applicationId, Guid userId, Guid nodeId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("IAmNotExpert"), applicationId, userId, nodeId);
        }

        public static int get_referrals_count(Guid applicationId, Guid userId, Guid nodeId)
        {
            return DBConnector.get_int(applicationId, GetFullyQualifiedName("GetReferralsCount"), applicationId, userId, nodeId);
        }

        public static List<Expert> get_expertise_suggestions(Guid applicationId, Guid userId, int count, int? lowerBoundary)
        {
            return CNParsers.expertise_suggestions(DBConnector.read(applicationId, GetFullyQualifiedName("GetExpertiseSuggestions"),
                applicationId, userId, count, lowerBoundary));
        }

        public static List<Node> suggest_node_relations(Guid applicationId, Guid userId, Guid? relatedNodeTypeId, int? count = 20)
        {
            return CNParsers.nodes(DBConnector.read(applicationId, GetFullyQualifiedName("SuggestNodeRelations"),
                applicationId, userId, null, relatedNodeTypeId, count, DateTime.Now));
        }

        public static List<NodeType> suggest_node_types_for_relations(Guid applicationId, Guid userId, int? count = 10)
        {
            return CNParsers.node_types(DBConnector.read(applicationId, GetFullyQualifiedName("SuggestNodeTypesForRelations"),
                applicationId, userId, null, count, DateTime.Now));
        }

        public static List<SimilarNode> suggest_similar_nodes(Guid applicationId, Guid nodeId, int? count)
        {
            List<SimilarNode> lst = CNParsers.similar_nodes(DBConnector.read(applicationId,
                GetFullyQualifiedName("SuggestSimilarNodes"), applicationId, nodeId, count));

            if (lst == null || lst.Count == 0) return new List<SimilarNode>();

            List<Node> nds = get_nodes(applicationId, lst.Select(u => u.Suggested.NodeID.Value).ToList(), full: null);

            for (int i = 0; i < lst.Count; ++i)
                lst[i].Suggested = nds.Where(u => u.NodeID == lst[i].Suggested.NodeID).FirstOrDefault();

            return lst;
        }

        public static List<KnowledgableUser> suggest_knowledgable_users(Guid applicationId, Guid nodeId, int? count)
        {
            List<KnowledgableUser> lst = CNParsers.knowledgable_users(DBConnector.read(applicationId,
                GetFullyQualifiedName("SuggestKnowledgableUsers"), applicationId, nodeId, count));

            if (lst == null || lst.Count == 0) return new List<KnowledgableUser>();

            List<User> usrs = UsersController.get_users(applicationId, lst.Select(u => u.User.UserID.Value).ToList());

            for (int i = 0; i < lst.Count; ++i)
                lst[i].User = usrs.Where(u => u.UserID == lst[i].User.UserID).FirstOrDefault();

            return lst;
        }

        public static List<Guid> get_existing_node_ids(Guid applicationId, List<Guid> nodeIds, 
            bool? searchable, bool? noContent)
        {
            List<Guid> retIds = new List<Guid>();

            PublicMethods.split_list<Guid>(nodeIds, 200, ids =>
            {
                List<Guid> newNodeIds = DBConnector.get_guid_list(applicationId, GetFullyQualifiedName("GetExistingNodeIDs"),
                    applicationId, ProviderUtil.list_to_string(nodeIds), ',', searchable, noContent);

                if (newNodeIds.Count > 0) retIds.AddRange(newNodeIds);
            });

            return retIds;
        }

        public static List<Guid> get_existing_node_type_ids(Guid applicationId, List<Guid> nodeTypeIds, bool? noContent)
        {
            return DBConnector.get_guid_list(applicationId, GetFullyQualifiedName("GetExistingNodeTypeIDs"),
                applicationId, ProviderUtil.list_to_string(nodeTypeIds), ',', noContent);
        }

        public static List<NodeInfo> get_node_info(Guid applicationId, List<Guid> nodeIds, Guid? currentUserId, 
            bool? tags, bool? description, bool? creator, bool? contributorsCount, bool? likesCount, bool? visitsCount, 
            bool? expertsCount, bool? membersCount, bool? childsCount, bool? relatedNodesCount, bool? likeStatus)
        {
            if (!currentUserId.HasValue || currentUserId == Guid.Empty) likeStatus = null;

            return CNParsers.node_info(DBConnector.read(applicationId, GetFullyQualifiedName("GetNodeInfo"),
                applicationId, ProviderUtil.list_to_string<Guid>(nodeIds), ',', currentUserId, tags, description, creator, 
                contributorsCount, likesCount, visitsCount, expertsCount, membersCount, childsCount, relatedNodesCount, likeStatus));
        }

        public static NodeInfo get_node_info(Guid applicationId, Guid nodeId, Guid? currentUserId, 
            bool? tags, bool? description, bool? creator, bool? contributorsCount, bool? likesCount, bool? visitsCount, 
            bool? expertsCount, bool? membersCount, bool? childsCount, bool? relatedNodesCount, bool? likeStatus)
        {
            return get_node_info(applicationId, new List<Guid>() { nodeId }, currentUserId, tags, description, creator, 
                contributorsCount, likesCount, visitsCount, expertsCount, membersCount, childsCount, 
                relatedNodesCount, likeStatus).FirstOrDefault();
        }

        public static bool initialize_extensions(Guid applicationId, Guid ownerId, Guid currentUserId, bool ignoreDefault = false)
        {
            List<Extension> lst = CNUtilities.extend_extensions(applicationId, new List<Extension>(), ignoreDefault);

            List<ExtensionType> enabledExtensions = lst.Where(u => !u.Disabled.HasValue || u.Disabled == false).Select(
                u => u.ExtensionType).ToList();

            List<ExtensionType> disabledExtensions = lst.Where(u => u.Disabled.HasValue && u.Disabled == true).Select(
                u => u.ExtensionType).ToList();

            return DBConnector.succeed(applicationId, GetFullyQualifiedName("InitializeExtensions"),
                applicationId, ownerId, ProviderUtil.list_to_string<ExtensionType>(enabledExtensions),
                ProviderUtil.list_to_string<ExtensionType>(disabledExtensions), ',', currentUserId, DateTime.Now);
        }

        private static bool _enable_disable_extension(Guid applicationId,
            Guid ownerId, ExtensionType extensionType, bool disable, Guid currentUserId)
        {
            if (ownerId == Guid.Empty || extensionType == ExtensionType.NotSet) return false;

            return DBConnector.succeed(applicationId, GetFullyQualifiedName("EnableDisableExtension"),
                applicationId, ownerId, extensionType.ToString(), disable, currentUserId, DateTime.Now);
        }

        public static bool enable_extension(Guid applicationId, Guid ownerId, ExtensionType extensionType, Guid currentUserId)
        {
            return _enable_disable_extension(applicationId, ownerId, extensionType, false, currentUserId);
        }

        public static bool disable_extension(Guid applicationId, Guid ownerId, ExtensionType extensionType, Guid currentUserId)
        {
            return _enable_disable_extension(applicationId, ownerId, extensionType, true, currentUserId);
        }

        public static bool set_extension_title(Guid applicationId, 
            Guid ownerId, ExtensionType extensionType, string title, Guid currentUserId)
        {
            if (ownerId == Guid.Empty || extensionType == ExtensionType.NotSet) return false;

            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetExtensionTitle"),
                applicationId, ownerId, extensionType.ToString(), title, currentUserId, DateTime.Now);
        }

        public static bool move_extension(Guid applicationId, Guid ownerId, ExtensionType extensionType, bool moveDown)
        {
            if (ownerId == Guid.Empty || extensionType == ExtensionType.NotSet) return false;

            return DBConnector.succeed(applicationId, GetFullyQualifiedName("MoveExtension"),
                applicationId, ownerId, extensionType.ToString(), moveDown);
        }

        public static bool save_extensions(Guid applicationId, Guid ownerId, List<Extension> extensions, Guid currentUserId)
        {
            if (extensions == null) extensions = new List<Extension>();

            int seq = 1;

            DBCompositeType<CNExtensionTableType> exts = new DBCompositeType<CNExtensionTableType>()
                .add(extensions.Select(f => new CNExtensionTableType(
                    ownerId: null,
                    extension: f.ExtensionType.ToString(),
                    title: f.Title,
                    seq++,
                    disabled: f.Disabled)).ToList());

            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SaveExtensions"),
                applicationId, ownerId, exts, currentUserId, DateTime.Now);
        }

        public static List<Extension> get_extensions(Guid applicationId, Guid ownerId)
        {
            return CNParsers.extensions(DBConnector.read(applicationId, GetFullyQualifiedName("GetExtensions"),
                applicationId, ownerId));
        }

        public static bool has_extension(Guid applicationId, Guid ownerId, ExtensionType extensionType)
        {
            if (ownerId == Guid.Empty || extensionType == ExtensionType.NotSet) return false;

            return DBConnector.succeed(applicationId, GetFullyQualifiedName("HasExtension"),
                applicationId, ownerId, extensionType.ToString());
        }

        public static List<NodeType> get_node_types_with_extension(Guid applicationId, List<ExtensionType> exts)
        {
            return CNParsers.node_types(DBConnector.read(applicationId, GetFullyQualifiedName("GetNodeTypesWithExtension"),
                applicationId, ProviderUtil.list_to_string<ExtensionType>(exts), ','));
        }

        public static List<NodeType> get_node_types_with_extension(Guid applicationId, ExtensionType ext)
        {
            return get_node_types_with_extension(applicationId, new List<ExtensionType>() { ext });
        }

        public static List<NodesCount> get_intellectual_properties_count(Guid applicationId, Guid userId, 
            Guid? nodeTypeId, Guid? nodeId, string additionalId, Guid? currentUserId, bool? isDocument,
            DateTime? lowerDateLimit, DateTime? upperDateLimit)
        {
            return CNParsers.nodes_count(DBConnector.read(applicationId, GetFullyQualifiedName("GetIntellectualPropertiesCount"),
                applicationId, userId, nodeTypeId, nodeId, additionalId, currentUserId, isDocument, lowerDateLimit, upperDateLimit));
        }

        public static NodesCount get_intellectual_properties_count(Guid applicationId, Guid userId, Guid nodeTypeId,
            Guid? currentUserId, bool? isDocument, DateTime? lowerDateLimit, DateTime? upperDateLimit)
        {
            return get_intellectual_properties_count(applicationId,
                userId, nodeTypeId, null, null, currentUserId, isDocument, lowerDateLimit, upperDateLimit).FirstOrDefault();
        }

        public static List<Node> get_intellectual_properties(Guid applicationId, Guid userId, List<Guid> nodeTypeIds, 
            Guid? nodeId, string additionalId, Guid? currentUserId, string searchText, bool? isDocument,
            DateTime? lowerDateLimit, DateTime? upperDateLimit, int? lowerBoundary, int? count, ref long totalCount)
        {
            DBResultSet results = DBConnector.read(applicationId, GetFullyQualifiedName("GetIntellectualProperties"),
                applicationId, userId, string.Join(",", nodeTypeIds), ',', nodeId, additionalId, currentUserId, 
                ProviderUtil.get_search_text(searchText), isDocument, lowerDateLimit, upperDateLimit, lowerBoundary, count);

            return CNParsers.nodes(results, full: null, totalCount: ref totalCount);
        }

        public static List<Node> get_intellectual_properties_of_friends(Guid applicationId, Guid userId, 
            Guid? nodeTypeId, int? lowerBoundary, int? count)
        {
            return CNParsers.nodes(DBConnector.read(applicationId, GetFullyQualifiedName("GetIntellectualPropertiesOfFriends"),
                applicationId, userId, nodeTypeId, lowerBoundary, count));
        }

        public static List<Node> get_document_tree_node_items(Guid applicationId, 
            Guid documentTreeNodeId, Guid? currenrUserId, bool? checkPrivacy, int? count, int? lowerBoundary)
        {
            return CNParsers.nodes(DBConnector.read(applicationId, GetFullyQualifiedName("GetDocumentTreeNodeItems"),
                applicationId, documentTreeNodeId, currenrUserId, checkPrivacy, DateTime.Now, 
                RaaiVanSettings.DefaultPrivacy(applicationId), count, lowerBoundary));
        }

        public static List<Node> get_document_tree_node_contents(Guid applicationId, Guid documentTreeNodeId, 
            Guid? currenrUserId, bool? checkPrivacy, int? count, int? lowerBoundary, string searchText, ref long totalCount)
        {
            DBResultSet results = DBConnector.read(applicationId, GetFullyQualifiedName("GetDocumentTreeNodeContents"),
                applicationId, documentTreeNodeId, currenrUserId, checkPrivacy, DateTime.Now,
                RaaiVanSettings.DefaultPrivacy(applicationId), count, lowerBoundary, ProviderUtil.get_search_text(searchText));

            return CNParsers.nodes(results, full: null, totalCount: ref totalCount);
        }

        public static List<Guid> is_node_type(Guid applicationId, List<Guid> ids)
        {
            return DBConnector.get_guid_list(applicationId, GetFullyQualifiedName("IsNodeType"),
                applicationId, ProviderUtil.list_to_string<Guid>(ids), ',');
        }

        public static bool is_node_type(Guid applicationId, Guid id)
        {
            return is_node_type(applicationId, new List<Guid>() { id }).Count == 1;
        }

        public static List<Guid> is_node(Guid applicationId, List<Guid> ids)
        {
            return DBConnector.get_guid_list(applicationId, GetFullyQualifiedName("IsNode"),
                applicationId, ProviderUtil.list_to_string<Guid>(ids), ',');
        }

        public static bool is_node(Guid applicationId, Guid id)
        {
            return is_node(applicationId, new List<Guid>() { id }).Count == 1;
        }

        public static List<ExploreItem> explore(Guid applicationId, Guid? baseId, Guid? relatedId,
            List<Guid> baseTypeIds, List<Guid> relatedTypeIds, Guid? secondLevelNodeId, 
            bool? registrationArea, bool? tags, bool? relations, int? lowerBoundary, int? count, string orderBy, 
            bool? orderByDesc, string searchText, bool? checkAccess, Guid? currentUserId, ref long totalCount)
        {
            DBResultSet results = DBConnector.read(applicationId, GetFullyQualifiedName("Explore"),
                applicationId, baseId, relatedId, ProviderUtil.list_to_string<Guid>(baseTypeIds), 
                ProviderUtil.list_to_string<Guid>(relatedTypeIds), ',', secondLevelNodeId, registrationArea, tags, relations, 
                lowerBoundary, count, orderBy, orderByDesc, ProviderUtil.get_search_text(searchText), checkAccess, currentUserId,
                DateTime.Now, RaaiVanSettings.DefaultPrivacy(applicationId));

            return CNParsers.explore_items(results, ref totalCount);
        }

        private static bool _update_form_and_wiki_tags(Guid applicationId, 
            List<Guid> nodeIds = null, Guid? creatorUserId = null, int? count = null)
        {
            bool form = RaaiVanConfig.Modules.FormGenerator(applicationId);
            bool wiki = true;

            return DBConnector.succeed(applicationId, GetFullyQualifiedName("UpdateFormAndWikiTags"),
                applicationId, ProviderUtil.list_to_string<Guid>(nodeIds), ',', creatorUserId, count, form, wiki);
        }

        public static bool update_form_and_wiki_tags(Guid applicationId, int? count)
        {
            return _update_form_and_wiki_tags(applicationId: applicationId, count: count);
        }

        public static bool update_form_and_wiki_tags(Guid applicationId, List<Guid> nodeIds, Guid currentUserId)
        {
            return nodeIds != null && nodeIds.Count > 0 && currentUserId != Guid.Empty &&
                _update_form_and_wiki_tags(applicationId: applicationId, nodeIds: nodeIds, creatorUserId: currentUserId);
        }

        public static bool update_form_and_wiki_tags(Guid applicationId, Guid nodeId, Guid currentUserId)
        {
            return update_form_and_wiki_tags(applicationId, new List<Guid>() { nodeId }, currentUserId);
        }

        /* Service */

        public static bool initialize_service(Guid applicationId, Guid nodeTypeId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("InitializeService"), applicationId, nodeTypeId);
        }

        private static List<Service> _get_services(Guid applicationId, Guid? nodeTypeIdOrNodeId, 
            Guid? currentUserId, bool? isDocument, bool? isKnowledge, bool? checkPrivacy)
        {
            return CNParsers.services(DBConnector.read(applicationId, GetFullyQualifiedName("GetServices"),
                applicationId, nodeTypeIdOrNodeId, currentUserId, isDocument, isKnowledge, 
                checkPrivacy, DateTime.Now, RaaiVanSettings.DefaultPrivacy(applicationId)));
        }

        public static List<Service> get_services(Guid applicationId, 
            Guid currentUserId, bool? isDocument = null, bool? isKnowledge = null, bool? checkPrivacy = true)
        {
            return _get_services(applicationId, null, currentUserId, isDocument, isKnowledge, checkPrivacy);
        }

        public static List<Service> get_services(Guid applicationId, List<Guid> nodeTypeIds)
        {
            return CNParsers.services(DBConnector.read(applicationId, GetFullyQualifiedName("GetServicesByIDs"),
                applicationId, ProviderUtil.list_to_string<Guid>(nodeTypeIds), ','));
        }

        public static Service get_service(Guid applicationId, Guid nodeTypeIdOrNodeId)
        {
            return _get_services(applicationId, nodeTypeIdOrNodeId, null, null, null, false).FirstOrDefault();
        }

        public static bool set_service_title(Guid applicationId, Guid nodeTypeId, string title)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetServiceTitle"), applicationId, nodeTypeId, title);
        }

        public static bool set_service_description(Guid applicationId, Guid nodeTypeId, string description)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetServiceDescription"), 
                applicationId, nodeTypeId, description);
        }

        public static bool set_service_success_message(Guid applicationId, Guid nodeTypeId, string message)
        {
            if (string.IsNullOrEmpty(message)) message = null;

            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetServiceSuccessMessage"),
                applicationId, nodeTypeId, message);
        }

        public static string get_service_success_message(Guid applicationId, Guid nodeTypeId)
        {
            return DBConnector.get_string(applicationId, GetFullyQualifiedName("GetServiceSuccessMessage"), applicationId, nodeTypeId);
        }

        public static bool set_service_admin_type(Guid applicationId, Guid serviceTypeId, ServiceAdminType adminType,
            Guid? adminNodeId, List<Guid> limitNodeTypeIds, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetServiceAdminType"),
                applicationId, serviceTypeId, adminType.ToString(), adminNodeId,
                ProviderUtil.list_to_string<Guid>(limitNodeTypeIds), ',', currentUserId, DateTime.Now);
        }

        public static List<NodeType> get_admin_area_limits(Guid applicationId, Guid nodeTypeIdOrNodeId)
        {
            return CNParsers.node_types(DBConnector.read(applicationId, GetFullyQualifiedName("GetAdminAreaLimits"),
                applicationId, nodeTypeIdOrNodeId));
        }

        public static bool set_max_acceptable_admin_level(Guid applicationId, 
            Guid nodeTypeId, int? maxAcceptableAdminLevel)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetMaxAcceptableAdminLevel"),
                applicationId, nodeTypeId, maxAcceptableAdminLevel);
        }

        public static bool set_contribution_limits(Guid applicationId, 
            Guid serviceTypeId, List<Guid> limitNodeTypeIds, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetContributionLimits"),
                applicationId, serviceTypeId, ProviderUtil.list_to_string<Guid>(limitNodeTypeIds), ',', currentUserId, DateTime.Now);
        }

        public static List<NodeType> get_contribution_limits(Guid applicationId, Guid nodeTypeIdOrNodeId)
        {
            return CNParsers.node_types(DBConnector.read(applicationId, GetFullyQualifiedName("GetContributionLimits"),
                applicationId, nodeTypeIdOrNodeId));
        }

        public static bool enable_contribution(Guid applicationId, Guid nodeTypeId, bool enable)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("EnableContribution"), applicationId, nodeTypeId, enable);
        }

        private static bool _no_content_service(Guid applicationId, Guid nodeTypeIdOrNodeId, bool? value)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("NoContentService"),
                applicationId, nodeTypeIdOrNodeId, value);
        }

        public static bool no_content_service(Guid applicationId, Guid nodeTypeIdOrNodeId, bool value)
        {
            return _no_content_service(applicationId, nodeTypeIdOrNodeId, value);
        }

        public static bool no_content_service(Guid applicationId, Guid nodeTypeIdOrNodeId)
        {
            return _no_content_service(applicationId, nodeTypeIdOrNodeId, value: null);
        }

        private static bool _is_knowledge(Guid applicationId, Guid nodeTypeIdOrNodeId, bool? isKnowledge)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("IsKnowledge"), 
                applicationId, nodeTypeIdOrNodeId, isKnowledge);
        }

        public static bool is_knowledge(Guid applicationId, Guid nodeTypeIdOrNodeId, bool isKnowledge)
        {
            return _is_knowledge(applicationId, nodeTypeIdOrNodeId, isKnowledge);
        }

        public static bool is_knowledge(Guid applicationId, Guid nodeTypeIdOrNodeId)
        {
            return _is_knowledge(applicationId, nodeTypeIdOrNodeId, isKnowledge: null);
        }

        private static bool _is_document(Guid applicationId, Guid nodeTypeIdOrNodeId, bool? isDocument)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("IsKnowledge"),
                applicationId, nodeTypeIdOrNodeId, isDocument);
        }

        public static bool is_document(Guid applicationId, Guid nodeTypeIdOrNodeId, bool isDocument)
        {
            return _is_document(applicationId, nodeTypeIdOrNodeId, isDocument);
        }

        public static bool is_document(Guid applicationId, Guid nodeTypeIdOrNodeId)
        {
            return _is_document(applicationId, nodeTypeIdOrNodeId, isDocument: null);
        }

        private static bool _enable_previous_version_select(Guid applicationId, Guid nodeTypeIdOrNodeId, bool? value)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("EnablePreviousVersionSelect"),
                applicationId, nodeTypeIdOrNodeId, value);
        }

        public static bool enable_previous_version_select(Guid applicationId, Guid nodeTypeIdOrNodeId, bool value)
        {
            return _enable_previous_version_select(applicationId, nodeTypeIdOrNodeId, value);
        }

        public static bool enable_previous_version_select(Guid applicationId, Guid nodeTypeIdOrNodeId)
        {
            return _enable_previous_version_select(applicationId, nodeTypeIdOrNodeId, value: null);
        }

        private static bool _is_tree(Guid applicationId, ref List<Guid> treeIds, List<Guid> nodeTypeOrNodeIds, bool? isTree)
        {
            if (isTree.HasValue)
                return DBConnector.succeed(applicationId, GetFullyQualifiedName("IsTree"),
                    applicationId, ProviderUtil.list_to_string<Guid>(nodeTypeOrNodeIds), ',', isTree);
            else {
                treeIds = DBConnector.get_guid_list(applicationId, GetFullyQualifiedName("IsTree"),
                    applicationId, ProviderUtil.list_to_string<Guid>(nodeTypeOrNodeIds), ',', isTree);
                return true;
            }
        }

        public static List<Guid> is_tree(Guid applicationId, List<Guid> nodeTypeOrNodeIds)
        {
            List<Guid> retList = new List<Guid>();
            _is_tree(applicationId, ref retList, nodeTypeOrNodeIds, isTree: null);
            return retList;
        }

        public static bool is_tree(Guid applicationId, Guid nodeTypeIdOrNodeId)
        {
            return is_tree(applicationId, new List<Guid>() { nodeTypeIdOrNodeId }).Count > 0;
        }

        public static bool is_tree(Guid applicationId, List<Guid> nodeTypeOrNodeIds, bool isTree)
        {
            List<Guid> retList = new List<Guid>();
            return _is_tree(applicationId, ref retList, nodeTypeOrNodeIds, isTree);
        }

        public static bool is_tree(Guid applicationId, Guid nodeTypeOrNodeId, bool isTree)
        {
            return is_tree(applicationId, new List<Guid>() { nodeTypeOrNodeId }, isTree);
        }

        private static bool _has_unique_membership(Guid applicationId, ref List<Guid> groupIds, List<Guid> nodeTypeOrNodeIds, bool? value)
        {
            if (value.HasValue)
                return DBConnector.succeed(applicationId, GetFullyQualifiedName("HasUniqueMembership"),
                    applicationId, ProviderUtil.list_to_string<Guid>(nodeTypeOrNodeIds), ',', value);
            else
            {
                groupIds = DBConnector.get_guid_list(applicationId, GetFullyQualifiedName("HasUniqueMembership"),
                    applicationId, ProviderUtil.list_to_string<Guid>(nodeTypeOrNodeIds), ',', value);
                return true;
            }
        }

        public static List<Guid> has_unique_membership(Guid applicationId, List<Guid> nodeTypeOrNodeIds)
        {
            List<Guid> retList = new List<Guid>();
            _has_unique_membership(applicationId, ref retList, nodeTypeOrNodeIds, value: null);
            return retList;
        }

        public static bool has_unique_membership(Guid applicationId, Guid nodeTypeIdOrNodeId)
        {
            return has_unique_membership(applicationId, new List<Guid>() { nodeTypeIdOrNodeId }).Count > 0;
        }

        public static bool has_unique_membership(Guid applicationId, List<Guid> nodeTypeOrNodeIds, bool value)
        {
            List<Guid> retList = new List<Guid>();
            return _has_unique_membership(applicationId, ref retList, nodeTypeOrNodeIds, value);
        }

        public static bool has_unique_membership(Guid applicationId, Guid nodeTypeOrNodeId, bool value)
        {
            return has_unique_membership(applicationId, new List<Guid>() { nodeTypeOrNodeId }, value);
        }

        private static bool _has_unique_admin_member(Guid applicationId, 
            ref List<Guid> groupIds, List<Guid> nodeTypeOrNodeIds, bool? value)
        {
            if (value.HasValue)
                return DBConnector.succeed(applicationId, GetFullyQualifiedName("HasUniqueAdminMember"),
                    applicationId, ProviderUtil.list_to_string<Guid>(nodeTypeOrNodeIds), ',', value);
            else
            {
                groupIds = DBConnector.get_guid_list(applicationId, GetFullyQualifiedName("HasUniqueAdminMember"),
                    applicationId, ProviderUtil.list_to_string<Guid>(nodeTypeOrNodeIds), ',', value);
                return true;
            }
        }

        public static List<Guid> has_unique_admin_member(Guid applicationId, List<Guid> nodeTypeOrNodeIds)
        {
            List<Guid> retList = new List<Guid>();
            _has_unique_admin_member(applicationId, ref retList, nodeTypeOrNodeIds, value: null);
            return retList;
        }

        public static bool has_unique_admin_member(Guid applicationId, Guid nodeTypeIdOrNodeId)
        {
            return has_unique_admin_member(applicationId, new List<Guid>() { nodeTypeIdOrNodeId }).Count > 0;
        }

        public static bool has_unique_admin_member(Guid applicationId, List<Guid> nodeTypeOrNodeIds, bool value)
        {
            List<Guid> retList = new List<Guid>();
            return _has_unique_admin_member(applicationId, ref retList, nodeTypeOrNodeIds, value);
        }

        public static bool has_unique_admin_member(Guid applicationId, Guid nodeTypeOrNodeId, bool value)
        {
            return has_unique_admin_member(applicationId, new List<Guid>() { nodeTypeOrNodeId }, value);
        }

        private static bool _abstract_and_keywords_disabled(Guid applicationId,
            ref List<Guid> retIds, List<Guid> nodeTypeOrNodeIds, bool? value)
        {
            if (value.HasValue)
                return DBConnector.succeed(applicationId, GetFullyQualifiedName("AbstractAndKeywordsDisabled"),
                    applicationId, ProviderUtil.list_to_string<Guid>(nodeTypeOrNodeIds), ',', value);
            else
            {
                retIds = DBConnector.get_guid_list(applicationId, GetFullyQualifiedName("AbstractAndKeywordsDisabled"),
                    applicationId, ProviderUtil.list_to_string<Guid>(nodeTypeOrNodeIds), ',', value);
                return true;
            }
        }

        public static List<Guid> abstract_and_keywords_disabled(Guid applicationId, List<Guid> nodeTypeOrNodeIds)
        {
            List<Guid> retList = new List<Guid>();
            _abstract_and_keywords_disabled(applicationId, ref retList, nodeTypeOrNodeIds, value: null);
            return retList;
        }

        public static bool abstract_and_keywords_disabled(Guid applicationId, Guid nodeTypeIdOrNodeId)
        {
            return abstract_and_keywords_disabled(applicationId, new List<Guid>() { nodeTypeIdOrNodeId }).Count > 0;
        }

        public static bool abstract_and_keywords_disabled(Guid applicationId, List<Guid> nodeTypeOrNodeIds, bool value)
        {
            List<Guid> retList = new List<Guid>();
            return _abstract_and_keywords_disabled(applicationId, ref retList, nodeTypeOrNodeIds, value);
        }

        public static bool abstract_and_keywords_disabled(Guid applicationId, Guid nodeTypeOrNodeId, bool value)
        {
            return abstract_and_keywords_disabled(applicationId, new List<Guid>() { nodeTypeOrNodeId }, value);
        }

        private static bool _file_upload_disabled(Guid applicationId,
            ref List<Guid> retIds, List<Guid> nodeTypeOrNodeIds, bool? value)
        {
            if (value.HasValue)
                return DBConnector.succeed(applicationId, GetFullyQualifiedName("FileUploadDisabled"),
                    applicationId, ProviderUtil.list_to_string<Guid>(nodeTypeOrNodeIds), ',', value);
            else
            {
                retIds = DBConnector.get_guid_list(applicationId, GetFullyQualifiedName("FileUploadDisabled"),
                    applicationId, ProviderUtil.list_to_string<Guid>(nodeTypeOrNodeIds), ',', value);
                return true;
            }
        }

        public static List<Guid> file_upload_disabled(Guid applicationId, List<Guid> nodeTypeOrNodeIds)
        {
            List<Guid> retList = new List<Guid>();
            _file_upload_disabled(applicationId, ref retList, nodeTypeOrNodeIds, value: null);
            return retList;
        }

        public static bool file_upload_disabled(Guid applicationId, Guid nodeTypeIdOrNodeId)
        {
            return file_upload_disabled(applicationId, new List<Guid>() { nodeTypeIdOrNodeId }).Count > 0;
        }

        public static bool file_upload_disabled(Guid applicationId, List<Guid> nodeTypeOrNodeIds, bool value)
        {
            List<Guid> retList = new List<Guid>();
            return _file_upload_disabled(applicationId, ref retList, nodeTypeOrNodeIds, value);
        }

        public static bool file_upload_disabled(Guid applicationId, Guid nodeTypeOrNodeId, bool value)
        {
            return file_upload_disabled(applicationId, new List<Guid>() { nodeTypeOrNodeId }, value);
        }

        private static bool _related_nodes_select_disabled(Guid applicationId,
            ref List<Guid> retIds, List<Guid> nodeTypeOrNodeIds, bool? value)
        {
            if (value.HasValue)
                return DBConnector.succeed(applicationId, GetFullyQualifiedName("RelatedNodesSelectDisabled"),
                    applicationId, ProviderUtil.list_to_string<Guid>(nodeTypeOrNodeIds), ',', value);
            else
            {
                retIds = DBConnector.get_guid_list(applicationId, GetFullyQualifiedName("RelatedNodesSelectDisabled"),
                    applicationId, ProviderUtil.list_to_string<Guid>(nodeTypeOrNodeIds), ',', value);
                return true;
            }
        }

        public static List<Guid> related_nodes_select_disabled(Guid applicationId, List<Guid> nodeTypeOrNodeIds)
        {
            List<Guid> retList = new List<Guid>();
            _related_nodes_select_disabled(applicationId, ref retList, nodeTypeOrNodeIds, value: null);
            return retList;
        }

        public static bool related_nodes_select_disabled(Guid applicationId, Guid nodeTypeIdOrNodeId)
        {
            return related_nodes_select_disabled(applicationId, new List<Guid>() { nodeTypeIdOrNodeId }).Count > 0;
        }

        public static bool related_nodes_select_disabled(Guid applicationId, List<Guid> nodeTypeOrNodeIds, bool value)
        {
            List<Guid> retList = new List<Guid>();
            return _related_nodes_select_disabled(applicationId, ref retList, nodeTypeOrNodeIds, value);
        }

        public static bool related_nodes_select_disabled(Guid applicationId, Guid nodeTypeOrNodeId, bool value)
        {
            return related_nodes_select_disabled(applicationId, new List<Guid>() { nodeTypeOrNodeId }, value);
        }

        public static bool editable_for_admin(Guid applicationId, Guid nodeTypeId, bool editable)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("EditableForAdmin"), applicationId, nodeTypeId, editable);
        }

        public static bool editable_for_creator(Guid applicationId, Guid nodeTypeId, bool editable)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("EditableForCreator"), applicationId, nodeTypeId, editable);
        }

        public static bool editable_for_owners(Guid applicationId, Guid nodeTypeId, bool editable)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("EditableForOwners"), applicationId, nodeTypeId, editable);
        }

        public static bool editable_for_experts(Guid applicationId, Guid nodeTypeId, bool editable)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("EditableForExperts"), applicationId, nodeTypeId, editable);
        }

        public static bool editable_for_members(Guid applicationId, Guid nodeTypeId, bool editable)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("EditableForMembers"), applicationId, nodeTypeId, editable);
        }

        public static bool edit_suggestion(Guid applicationId, Guid nodeTypeId, bool enable)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("EditSuggestion"), applicationId, nodeTypeId, enable);
        }

        public static bool add_free_user(Guid applicationId, Guid nodeTypeId, Guid userId, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("AddFreeUser"),
                applicationId, nodeTypeId, userId, currentUserId, DateTime.Now);
        }

        public static bool remove_free_user(Guid applicationId, Guid nodeTypeId, Guid userId, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ArithmeticDeleteFreeUser"),
                applicationId, nodeTypeId, userId, currentUserId, DateTime.Now);
        }

        public static List<User> get_free_users(Guid applicationId, Guid nodeTypeId)
        {
            List<Guid> userIds = DBConnector.get_guid_list(applicationId, GetFullyQualifiedName("GetFreeUserIDs"),
                applicationId, nodeTypeId);

            return UsersController.get_users(applicationId, userIds);
        }

        public static bool is_free_user(Guid applicationId, Guid nodeTypeIdOrNodeId, Guid userId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("IsFreeUser"), applicationId, nodeTypeIdOrNodeId, userId);
        }

        public static bool add_service_admin(Guid applicationId, Guid nodeTypeId, Guid userId, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("AddServiceAdmin"),
                applicationId, nodeTypeId, userId, currentUserId, DateTime.Now);
        }

        public static bool remove_service_admin(Guid applicationId, Guid nodeTypeId, Guid userId, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ArithmeticDeleteServiceAdmin"),
                applicationId, nodeTypeId, userId, currentUserId, DateTime.Now);
        }

        public static List<User> get_service_admins(Guid applicationId, Guid nodeTypeId)
        {
            List<Guid> userIds = DBConnector.get_guid_list(applicationId, GetFullyQualifiedName("GetServiceAdminIDs"),
                applicationId, nodeTypeId);

            return UsersController.get_users(applicationId, userIds);
        }

        public static List<Guid> is_service_admin(Guid applicationId, List<Guid> nodeTypeIdOrNodeIds, Guid userId)
        {
            return DBConnector.get_guid_list(applicationId, GetFullyQualifiedName("IsServiceAdmin"),
                applicationId, ProviderUtil.list_to_string<Guid>(nodeTypeIdOrNodeIds), ',', userId);
        }

        public static bool is_service_admin(Guid applicationId, Guid nodeTypeIdOrNodeId, Guid userId)
        {
            return is_service_admin(applicationId, new List<Guid>() { nodeTypeIdOrNodeId }, userId).Count > 0;
        }

        public static bool register_new_node(Guid applicationId, Node nodeObject, Guid? workflowId, Guid? formInstanceId,
            Guid? wfDirectorNodeId, Guid? wfDirectorUserId, ref List<Dashboard> dashboards, ref string message)
        {
            if (!nodeObject.CreationDate.HasValue) nodeObject.CreationDate = DateTime.Now;

            DBCompositeType<GuidFloatTableType> creators = new DBCompositeType<GuidFloatTableType>()
                .add(nodeObject.Contributors.Select(n => new GuidFloatTableType(n.User.UserID, n.CollaborationShare)).ToList());

            string strTags = nodeObject.Tags.Count == 0 ? null : ProviderUtil.get_tags_text(nodeObject.Tags);

            return DBConnector.get_dashboards(applicationId, ref message, ref dashboards, GetFullyQualifiedName("RegisterNewNode"),
                applicationId, nodeObject.NodeID, nodeObject.NodeTypeID, nodeObject.AdditionalID_Main,
                nodeObject.AdditionalID, nodeObject.ParentNodeID, nodeObject.DocumentTreeNodeID, nodeObject.PreviousVersionID,
                nodeObject.Name, nodeObject.Description, strTags, nodeObject.Creator.UserID, DateTime.Now, creators, 
                nodeObject.OwnerID, workflowId, nodeObject.AdminAreaID, formInstanceId, wfDirectorNodeId, wfDirectorUserId) > 0;
        }

        public static bool register_new_node(Guid applicationId, Node nodeObject, Guid? workflowId, Guid? formInstanceId,
            Guid? wfDirectorNodeId, Guid? wfDirectorUserId, ref string message)
        {
            List<Dashboard> lst = new List<Dashboard>();
            return register_new_node(applicationId, nodeObject, workflowId, formInstanceId, 
                wfDirectorNodeId, wfDirectorUserId, ref lst, ref message);
        }

        public static bool set_admin_area(Guid applicationId, Guid nodeId, Guid? areaId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetAdminArea"), applicationId, nodeId, areaId);
        }

        public static bool set_contributors(Guid applicationId, Node info, ref string errorMessage)
        {
            if (!info.LastModificationDate.HasValue) info.LastModificationDate = DateTime.Now;

            DBCompositeType<GuidFloatTableType> creators = new DBCompositeType<GuidFloatTableType>()
                .add(info.Contributors.Select(n => new GuidFloatTableType(n.User.UserID, n.CollaborationShare)).ToList());

            return DBConnector.succeed(applicationId, ref errorMessage, GetFullyQualifiedName("SetContributors"),
                applicationId, info.NodeID, creators, info.OwnerID, info.LastModifierUserID, DateTime.Now);
        }

        /* end of Service */
    }
}
