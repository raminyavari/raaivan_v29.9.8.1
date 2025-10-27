using RaaiVan.Modules.GlobalUtilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaaiVan.Modules.CoreNetwork
{
    public static class CNParsers
    {
        public static List<NodeType> node_types(DBResultSet results, ref long totalCount)
        {
            List<NodeType> retList = new List<NodeType>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                NodeType nodeType = new NodeType()
                {
                    NodeTypeID = table.GetGuid(i, "NodeTypeID"),
                    ParentID = table.GetGuid(i, "ParentID"),
                    Name = table.GetString(i, "Name"),
                    NodeTypeAdditionalID = table.GetString(i, "AdditionalID"),
                    AdditionalID = table.GetInt(i, "AdditionalID"),
                    AdditionalIDPattern = table.GetString(i, "AdditionalIDPattern"),
                    Archive = table.GetBool(i, "Archive"),
                    IsService = table.GetBool(i, "IsService")
                };

                nodeType.HasDefaultPattern = string.IsNullOrEmpty(nodeType.AdditionalIDPattern);
                if (nodeType.HasDefaultPattern.Value) nodeType.AdditionalIDPattern = CNUtilities.DefaultAdditionalIDPattern;

                retList.Add(nodeType);
            }

            totalCount = results.get_table(1).GetLong(row: 0, column: 0, defaultValue: 0).Value;

            return retList;
        }

        public static List<NodeType> node_types(DBResultSet results) {
            long totalCount = 0;
            return node_types(results, ref totalCount);
        }

        public static List<RelationType> relation_types(DBResultSet results)
        {
            List<RelationType> relationTypes = new List<RelationType>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                RelationType relationType = new RelationType();

                relationType.RelationTypeID = table.GetGuid(i, "RelationTypeID");
                relationType.Name = table.GetString(i, "Name");
                relationType.AdditionalID = table.GetInt(i, "AdditionalID");

                relationTypes.Add(relationType);
            }

            return relationTypes;
        }

        public static List<Node> nodes(DBResultSet results, ref List<NodesCount> lstCounts, 
            ref long totalCount, bool? full, bool fetchCounts = false)
        {
            List<Node> nodes = new List<Node>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                Node node = new Node();

                node.NodeID = table.GetGuid(i, "NodeID");
                node.NodeTypeID = table.GetGuid(i, "NodeTypeID");
                node.NodeType = table.GetString(i, "NodeType");
                node.TypeAdditionalID = table.GetString(i, "NodeTypeAdditionalID");
                node.NodeTypeAdditionalID = table.GetInt(i, "NodeTypeAdditionalID");
                node.Name = table.GetString(i, "Name");
                node.AdditionalID_Main = table.GetString(i, "AdditionalID_Main");
                node.AdditionalID = table.GetString(i, "AdditionalID");
                node.ParentNodeID = table.GetGuid(i, "ParentNodeID");
                node.Creator.UserID = table.GetGuid(i, "CreatorUserID");
                node.CreationDate = table.GetDate(i, "CreationDate");
                node.AdminAreaID = table.GetGuid(i, "AdminAreaID");
                node.DocumentTreeNodeID = table.GetGuid(i, "DocumentTreeNodeID");
                node.Status = table.GetEnum<Status>(i, "Status", Status.NotSet);
                node.WFState = table.GetString(i, "WFState");
                node.Searchable = table.GetBool(i, "Searchable");
                node.HideCreators = table.GetBool(i, "HideCreators");
                node.Archive = table.GetBool(i, "Archive");
                
                if (full.HasValue && full.Value)
                {
                    node.Creator.UserName = table.GetString(i, "CreatorUserName");
                    node.Creator.FirstName = table.GetString(i, "CreatorFirstName");
                    node.Creator.LastName = table.GetString(i, "CreatorLastName");
                    node.DocumentTreeID = table.GetGuid(i, "DocumentTreeID");
                    node.DocumentTreeName = table.GetString(i, "DocumentTreeName");
                    node.PreviousVersionID = table.GetGuid(i, "PreviousVersionID");
                    node.PreviousVersionName = table.GetString(i, "PreviousVersionName");
                    node.Description = table.GetString(i, "Description");
                    node.PublicDescription = table.GetString(i, "PublicDescription");
                    node.Tags = ProviderUtil.get_tags_list(table.GetString(i, "Tags"));
                    node.LikesCount = table.GetInt(i, "LikesCount");
                    node.LikeStatus = table.GetBool(i, "LikeStatus");
                    node.MembershipStatus = table.GetString(i, "MembershipStatus");
                    node.VisitsCount = table.GetInt(i, "VisitsCount");
                    node.AdminAreaName = table.GetString(i, "AdminAreaName");
                    node.AdminAreaType = table.GetString(i, "AdminAreaType");
                    node.ConfidentialityLevel.ID = table.GetGuid(i, "ConfidentialityLevelID");
                    node.ConfidentialityLevel.LevelID = table.GetInt(i, "ConfidentialityLevelNum");
                    node.ConfidentialityLevel.Title = table.GetString(i, "ConfidentialityLevel");
                    node.OwnerID = table.GetGuid(i, "OwnerID");
                    node.OwnerName = table.GetString(i, "OwnerName");
                    node.PublicationDate = table.GetDate(i, "PublicationDate");
                    node.ExpirationDate = table.GetDate(i, "ExpirationDate");
                    node.Score = table.GetDouble(i, "Score");
                    node.IsFreeUser = table.GetBool(i, "IsFreeUser");
                    node.HasWikiContent = table.GetBool(i, "HasWikiContent");
                    node.HasFormContent = table.GetBool(i, "HasFormContent");

                }

                nodes.Add(node);
            }

            if (results.TablesCount > 1) totalCount = results.get_table(1).GetLong(row: 0, column: 0, defaultValue: 0).Value;

            if (fetchCounts && results.TablesCount > 2) lstCounts = nodes_count(results.get_table(2));

            return nodes;
        }

        public static List<Node> nodes(DBResultSet results, bool? full, ref long totalCount)
        {
            List<NodesCount> lst = new List<NodesCount>();
            return nodes(results, ref lst, ref totalCount, full, fetchCounts: false);
        }

        public static List<Node> nodes(DBResultSet results, bool? full = false) {
            long totalCount = 0;
            return nodes(results, full, ref totalCount);
        }

        public static Dictionary<string, object> node_counts_grouped_by_element(DBResultSet results)
        {
            Dictionary<string, object> ret = new Dictionary<string, object>();
            ArrayList items = new ArrayList();

            RVDataTable table = results.get_table();

            ret["Items"] = items;

            for (int i = 0; i < table.Rows.Count; i++)
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();

                dic["TextValue"] = Base64.encode(table.GetString(i, "TextValue"));
                dic["BitValue"] = table.GetValue(i, "BitValue");
                dic["Type"] = table.GetValue(i, "Type");
                dic["Count"] = table.GetValue(i, "Count");

                items.Add(dic);
            }

            return ret;
        }

        public static List<Node> popular_nodes(DBResultSet results, ref long totalCount)
        {
            List<Node> nodes = new List<Node>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                Node node = new Node();

                node.NodeID = table.GetGuid(i, "NodeID");
                node.NodeTypeID = table.GetGuid(i, "NodeTypeID");
                node.Name = table.GetString(i, "Name");
                node.NodeType = table.GetString(i, "NodeType");
                node.VisitsCount = table.GetInt(i, "VisitsCount");
                node.LikesCount = table.GetInt(i, "LikesCount");

                totalCount = table.GetLong(i, "TotalCount", 0).Value;

                nodes.Add(node);
            }

            return nodes;
        }

        public static List<NodesCount> nodes_count(RVDataTable table)
        {
            bool? hasOrder = null;

            List<NodesCount> lst = new List<NodesCount>();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                NodesCount nodesCount = new NodesCount();

                if (!hasOrder.HasValue || hasOrder.Value)
                {
                    nodesCount.Order = table.GetInt(i, "Order");
                    nodesCount.ReverseOrder = table.GetInt(i, "ReverseOrder");

                    hasOrder = true;
                }

                nodesCount.NodeTypeID = table.GetGuid(i, "NodeTypeID");
                nodesCount.NodeTypeAdditionalID = table.GetString(i, "NodeTypeAdditionalID");
                nodesCount.TypeName = table.GetString(i, "TypeName");
                nodesCount.Count = table.GetInt(i, "NodesCount");

                lst.Add(nodesCount);
            }

            return lst;
        }

        public static List<NodesCount> nodes_count(DBResultSet results) {
            return nodes_count(results.get_table());
        }

        public static List<NodeMember> node_members(DBResultSet results, bool? parseNode, bool? parseUser, ref long totalCount)
        {
            List<NodeMember> nodeMembers = new List<NodeMember>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                NodeMember nodeMember = new NodeMember();

                nodeMember.Node.NodeID = table.GetGuid(i, "NodeID");
                nodeMember.Member.UserID = table.GetGuid(i, "UserID");
                nodeMember.MembershipDate = table.GetDate(i, "MembershipDate");
                nodeMember.IsAdmin = table.GetBool(i, "IsAdmin");
                nodeMember.IsPending = table.GetBool(i, "IsPending");
                nodeMember.Status = table.GetString(i, "Status");
                nodeMember.AcceptionDate = table.GetDate(i, "AcceptionDate");
                nodeMember.Position = table.GetString(i, "Position");

                if (parseNode.HasValue && parseNode.Value)
                {
                    nodeMember.Node.AdditionalID = table.GetString(i, "NodeAdditionalID");
                    nodeMember.Node.Name = table.GetString(i, "NodeName");
                    nodeMember.Node.NodeTypeID = table.GetGuid(i, "NodeTypeID");
                    nodeMember.Node.NodeType = table.GetString(i, "NodeType");
                }

                if (parseUser.HasValue && parseUser.Value)
                {
                    nodeMember.Member.UserName = table.GetString(i, "UserName");
                    nodeMember.Member.FirstName = table.GetString(i, "FirstName");
                    nodeMember.Member.LastName = table.GetString(i, "LastName");
                }

                nodeMembers.Add(nodeMember);
            }

            totalCount = results.get_table(1).GetLong(row: 0, column: 0, defaultValue: 0).Value;

            return nodeMembers;
        }

        public static List<NodeMember> node_members(DBResultSet results, bool? parseNode, bool? parseUser) {
            long totalCount = 0;
            return node_members(results, parseNode, parseUser, ref totalCount);
        }

        public static List<NodeList> lists(DBResultSet results)
        {
            List<NodeList> lists = new List<NodeList>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                NodeList nodeList = new NodeList();

                nodeList.ListID = table.GetGuid(i, "ListID");
                nodeList.Name = table.GetString(i, "ListName");
                nodeList.AdditionalID = table.GetString(i, "AdditionalID");
                nodeList.NodeTypeID = table.GetGuid(i, "NodeTypeID");
                nodeList.NodeType = table.GetString(i, "NodeType");
                nodeList.OwnerID = table.GetGuid(i, "OwnerID");
                nodeList.OwnerType = table.GetString(i, "OwnerType");

                lists.Add(nodeList);
            }

            return lists;
        }

        public static List<HierarchyAdmin> hierarchy_admins(DBResultSet results)
        {
            List<HierarchyAdmin> admins = new List<HierarchyAdmin>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                HierarchyAdmin hierarchyAdmin = new HierarchyAdmin();

                hierarchyAdmin.Node.NodeID = table.GetGuid(i, "NodeID");
                hierarchyAdmin.User.UserID = table.GetGuid(i, "UserID");
                hierarchyAdmin.Level = table.GetInt(i, "Level");

                admins.Add(hierarchyAdmin);
            }

            return admins;
        }

        public static List<Tag> tags(DBResultSet results)
        {
            List<Tag> tags = new List<Tag>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                Tag tag = new Tag();

                tag.TagID = table.GetGuid(i, "TagID");
                tag.Text = table.GetString(i, "Tag");
                tag.Approved = table.GetBool(i, "IsApproved");
                tag.CallsCount = table.GetInt(i, "CallsCount");

                tags.Add(tag);
            }

            return tags;
        }

        public static List<NodeCreator> node_creators(DBResultSet results, bool? full)
        {
            List<NodeCreator> nodeCreators = new List<NodeCreator>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                NodeCreator creator = new NodeCreator();

                creator.NodeID = table.GetGuid(i, "NodeID");
                creator.User.UserID = table.GetGuid(i, "UserID");
                creator.CollaborationShare = table.GetDouble(i, "CollaborationShare");
                creator.Status = table.GetString(i, "Status");

                if (full.HasValue && full.Value)
                {
                    creator.User.UserName = table.GetString(i, "UserName");
                    creator.User.FirstName = table.GetString(i, "FirstName");
                    creator.User.LastName = table.GetString(i, "LastName");
                }

                nodeCreators.Add(creator);
            }

            return nodeCreators;
        }

        public static List<Guid> fan_user_ids(DBResultSet results, ref long totalCount)
        {
            List<Guid> retList = new List<Guid>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                Guid? id = table.GetGuid(i, "UserID");
                if (id.HasValue) retList.Add(id.Value);

                totalCount = table.GetLong(row: i, column: "TotalCount", defaultValue: 0).Value;
            }

            return retList;
        }

        public static List<Expert> experts(DBResultSet results, ref long totalCount)
        {
            List<Expert> retList = new List<Expert>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                totalCount = table.GetLong(i, "TotalCount", defaultValue: 0).Value;

                Expert expert = new Expert();

                expert.Node.NodeID = table.GetGuid(i, "NodeID");
                expert.Node.AdditionalID = table.GetString(i, "NodeAdditionalID");
                expert.Node.Name = table.GetString(i, "NodeName");
                expert.Node.NodeTypeID = table.GetGuid(i, "NodeTypeID");
                expert.Node.NodeType = table.GetString(i, "NodeType");
                expert.User.UserID = table.GetGuid(i, "ExpertUserID");
                expert.User.UserName = table.GetString(i, "ExpertUserName");
                expert.User.FirstName = table.GetString(i, "ExpertFirstName");
                expert.User.LastName = table.GetString(i, "ExpertLastName");

                retList.Add(expert);
            }

            return retList;
        }

        public static List<Expert> experts(DBResultSet results) {
            long totalCount = 0;
            return experts(results, ref totalCount); 
        }

        public static List<Expert> expertise_suggestions(DBResultSet results)
        {
            List<Expert> retList = new List<Expert>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                Expert expert = new Expert();

                expert.Node.NodeID = table.GetGuid(i, "NodeID");
                expert.Node.Name = table.GetString(i, "NodeName");
                expert.Node.NodeType = table.GetString(i, "NodeType");
                expert.User.UserID = table.GetGuid(i, "ExpertUserID");
                expert.User.UserName = table.GetString(i, "ExpertUserName");
                expert.User.FirstName = table.GetString(i, "ExpertFirstName");
                expert.User.LastName = table.GetString(i, "ExpertLastName");

                retList.Add(expert);
            }

            return retList;
        }

        public static List<NodeInfo> node_info(DBResultSet results)
        {
            List<NodeInfo> retList = new List<NodeInfo>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                NodeInfo info = new NodeInfo();

                info.NodeID = table.GetGuid(i, "NodeID");
                info.NodeTypeID = table.GetGuid(i, "NodeTypeID");
                info.Description = table.GetString(i, "Description");
                info.Creator.UserID = table.GetGuid(i, "CreatorUserID");
                info.Creator.UserName = table.GetString(i, "CreatorUserName");
                info.Creator.FirstName = table.GetString(i, "CreatorFirstName");
                info.Creator.LastName = table.GetString(i, "CreatorLastName");
                info.ContributorsCount = table.GetInt(i, "ContributorsCount");
                info.LikesCount = table.GetInt(i, "LikesCount");
                info.VisitsCount = table.GetInt(i, "VisitsCount");
                info.ExpertsCount = table.GetInt(i, "ExpertsCount");
                info.MembersCount = table.GetInt(i, "MembersCount");
                info.ChildsCount = table.GetInt(i, "ChildsCount");
                info.RelatedNodesCount = table.GetInt(i, "RelatedNodesCount");
                info.LikeStatus = table.GetBool(i, "LikeStatus");
                info.Tags = ProviderUtil.get_tags_list(table.GetString(i, "Tags"));
                
                retList.Add(info);
            }

            return retList;
        }

        public static List<Extension> extensions(DBResultSet results)
        {
            List<Extension> retList = new List<Extension>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                Extension extension = new Extension();

                extension.OwnerID = table.GetGuid(i, "OwnerID");
                extension.Title = table.GetString(i, "Title");
                extension.Disabled = table.GetBool(i, "Disabled");
                extension.ExtensionType = table.GetEnum<ExtensionType>(i, "Extension", defaultValue: ExtensionType.NotSet);

                extension.Initialized = true;

                if (extension.ExtensionType != ExtensionType.NotSet) retList.Add(extension);
            }

            return retList;
        }

        public static List<Service> services(DBResultSet results)
        {
            List<Service> retList = new List<Service>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                Service service = new Service();

                service.NodeType.NodeTypeID = table.GetGuid(i, "NodeTypeID");
                service.NodeType.Name = table.GetString(i, "NodeType");
                service.Title = table.GetString(i, "ServiceTitle");
                service.Description = table.GetString(i, "ServiceDescription");
                service.EnableContribution = table.GetBool(i, "EnableContribution");
                service.NoContent = table.GetBool(i, "NoContent");
                service.IsKnowledge = table.GetBool(i, "IsKnowledge");
                service.IsDocument = table.GetBool(i, "IsDocument");
                service.EnablePreviousVersionSelect = table.GetBool(i, "EnablePreviousVersionSelect");
                service.IsTree = table.GetBool(i, "IsTree");
                service.UniqueMembership = table.GetBool(i, "UniqueMembership");
                service.UniqueAdminMember = table.GetBool(i, "UniqueAdminMember");
                service.DisableAbstractAndKeywords = table.GetBool(i, "DisableAbstractAndKeywords");
                service.DisableFileUpload = table.GetBool(i, "DisableFileUpload");
                service.DisableRelatedNodesSelect = table.GetBool(i, "DisableRelatedNodesSelect");
                service.AdminNode.NodeID = table.GetGuid(i, "AdminNodeID");
                service.MaxAcceptableAdminLevel = table.GetInt(i, "MaxAcceptableAdminLevel");
                service.LimitAttachedFilesTo = ListMaker.get_string_items(table.GetString(i, "LimitAttachedFilesTo"), ',');
                service.MaxAttachedFileSize = table.GetInt(i, "MaxAttachedFileSize");
                service.MaxAttachedFilesCount = table.GetInt(i, "MaxAttachedFilesCount");
                service.EditableForAdmin = table.GetBool(i, "EditableForAdmin");
                service.EditableForCreator = table.GetBool(i, "EditableForCreator");
                service.EditableForContributors = table.GetBool(i, "EditableForOwners");
                service.EditableForExperts = table.GetBool(i, "EditableForExperts");
                service.EditableForMembers = table.GetBool(i, "EditableForMembers");
                service.EditSuggestion = table.GetBool(i, "EditSuggestion");
                service.AdminType = table.GetEnum<ServiceAdminType>(i, "AdminType", defaultValue: ServiceAdminType.NotSet);

                retList.Add(service);
            }

            return retList;
        }

        public static void user2node_status(DBResultSet results, ref Guid? nodeTypeId,
            ref Guid? areaId, ref bool isCreator, ref bool isContributor, ref bool isExpert,
            ref bool isMember, ref bool isAdminMember, ref bool isServiceAdmin)
        {
            RVDataTable table = results.get_table();

            nodeTypeId = table.GetGuid(0, "NodeTypeID");
            areaId = table.GetGuid(0, "AreaID");
            isCreator = table.GetBool(0, "IsCreator", defaultValue: false).Value;
            isContributor = table.GetBool(0, "IsContributor", defaultValue: false).Value;
            isExpert = table.GetBool(0, "IsExpert", defaultValue: false).Value;
            isMember = table.GetBool(0, "IsMember", defaultValue: false).Value;
            isAdminMember = table.GetBool(0, "IsAdminMember", defaultValue: false).Value;
            isServiceAdmin = table.GetBool(0, "IsServiceAdmin", defaultValue: false).Value;
        }

        public static List<ExploreItem> explore_items(DBResultSet results, ref long totalCount)
        {
            List<ExploreItem> retList = new List<ExploreItem>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                totalCount = table.GetLong(row: i, column: "TotalCount", defaultValue: 0).Value;

                ExploreItem itm = new ExploreItem();

                itm.BaseID = table.GetGuid(i, "BaseID");
                itm.BaseTypeID = table.GetGuid(i, "BaseTypeID");
                itm.BaseName = table.GetString(i, "BaseName");
                itm.BaseType = table.GetString(i, "BaseType");
                itm.RelatedID = table.GetGuid(i, "RelatedID");
                itm.RelatedTypeID = table.GetGuid(i, "RelatedTypeID");
                itm.RelatedName = table.GetString(i, "RelatedName");
                itm.RelatedType = table.GetString(i, "RelatedType");
                itm.RelatedCreationDate = table.GetDate(i, "RelatedCreationDate");
                itm.IsTag = table.GetBool(i, "IsTag");
                itm.IsRelation = table.GetBool(i, "IsRelation");
                itm.IsRegistrationArea = table.GetBool(i, "IsRegistrationArea");

                retList.Add(itm);
            }

            return retList;
        }

        public static List<SimilarNode> similar_nodes(DBResultSet results)
        {
            List<SimilarNode> retList = new List<SimilarNode>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                SimilarNode itm = new SimilarNode();

                itm.Suggested.NodeID = table.GetGuid(i, "NodeID");
                itm.Rank = table.GetDouble(i, "Rank");
                itm.Tags = table.GetBool(i, "Tags");
                itm.Favorites = table.GetBool(i, "Favorites");
                itm.Relations = table.GetBool(i, "Relations");
                itm.Experts = table.GetBool(i, "Experts");

                retList.Add(itm);
            }

            return retList;
        }

        public static List<KnowledgableUser> knowledgable_users(DBResultSet results)
        {
            List<KnowledgableUser> retList = new List<KnowledgableUser>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                KnowledgableUser itm = new CoreNetwork.KnowledgableUser();

                itm.User.UserID = table.GetGuid(i, "UserID");
                itm.Rank = table.GetDouble(i, "Rank");
                itm.Expert = table.GetBool(i, "Expert");
                itm.Contributor = table.GetBool(i, "Contributor");
                itm.WikiEditor = table.GetBool(i, "WikiEditor");
                itm.Member = table.GetBool(i, "Member");
                itm.ExpertOfRelatedNode = table.GetBool(i, "ExpertOfRelatedNode");
                itm.ContributorOfRelatedNode = table.GetBool(i, "ContributorOfRelatedNode");
                itm.MemberOfRelatedNode = table.GetBool(i, "MemberOfRelatedNode");

                retList.Add(itm);
            }

            return retList;
        }

        public static Dictionary<string, Guid> node_ids(DBResultSet results)
        {
            Dictionary<string, Guid> ret = new Dictionary<string, Guid>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                string key = table.GetString(i, "AdditionalID");
                Guid? value = table.GetGuid(i, "NodeID");

                if (!string.IsNullOrEmpty(key) && value.HasValue) ret[key] = value.Value;
            }

            return ret;
        }
    }
}
