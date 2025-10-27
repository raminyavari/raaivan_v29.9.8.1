using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RaaiVan.Modules.Users;
using System.Globalization;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.Privacy;

namespace RaaiVan.Modules.CoreNetwork
{
    public enum NodeTypes
    {
        KnowledgeDomain,
        Project,
        Process,
        Community,
        Knowledge,
        Department,
        Expertise,
        Tag
    }
    
    public enum NodeMemberStatuses
    {
        NotSet,
        Accepted,
        Pending
    }
    
    public enum ServiceAdminType
    {
        NotSet,
        AreaAdmin,
        ComplexAdmin,
        SpecificNode,
        Registerer
    }

    [Serializable]
    public enum ExtensionType
    {
        NotSet,
        Wiki,
        Form,
        Documents,
        Posts,
        Experts,
        Members,
        Group,
        Events,
        Browser
    }

    public enum Status
    {
        NotSet,
        Personal,
        SentToAdmin,
        SentBackForRevision,
        SentToEvaluators,
        Rejected,
        Accepted
    }

    public enum AdminLevel
    {
        System,
        Service,
        Node,
        Creator
    }

    public static class CNUtilities
    {
        private static string _DefaultAdditionalIDPattern;
        public static string DefaultAdditionalIDPattern
        {
            get
            {
                if (_DefaultAdditionalIDPattern == null)
                {
                    string ptt = string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["DefaultAdditionalIDPattern"]) ?
                        string.Empty : System.Configuration.ConfigurationManager.AppSettings["DefaultAdditionalIDPattern"];
                    _DefaultAdditionalIDPattern = string.IsNullOrEmpty(ptt) || !Expressions.is_match(ptt, Expressions.Patterns.AdditionalID) ?
                        "~[[PYear]]0~[[NCount]]0~[[NCountSPY]]" : ptt;
                }
                return _DefaultAdditionalIDPattern;
            }
        }
        
        public static bool validate_additional_id_pattern(string pattern)
        {
            return string.IsNullOrEmpty(pattern) ||
                (pattern.Length <= 250 && Expressions.is_match(pattern, Expressions.Patterns.AdditionalID));
        }

        public static int get_node_type_additional_id(NodeTypes nodeType)
        {
            switch (nodeType)
            {
                case NodeTypes.KnowledgeDomain:
                    return 1;
                case NodeTypes.Project:
                    return 2;
                case NodeTypes.Process:
                    return 3;
                case NodeTypes.Community:
                    return 4;
                case NodeTypes.Knowledge:
                    return 5;
                case NodeTypes.Department:
                    return 6;
                case NodeTypes.Expertise:
                    return 7;
                case NodeTypes.Tag:
                    return 8;
                default:
                    return 0;
            }
        }
        
        private static string _generate_new_additional_id(Guid applicationId, Guid? nodeTypeId, 
            string additionalIdPattern, bool getPattern, DateTime? time, ref Dictionary<string, string> dic)
        {
            if (nodeTypeId == Guid.Empty) nodeTypeId = null;

            if (getPattern)
            {
                NodeType nt = nodeTypeId.HasValue ? CNController.get_node_type(applicationId, nodeTypeId.Value) : null;
                additionalIdPattern = nt == null || string.IsNullOrEmpty(nt.AdditionalIDPattern) ?
                    DefaultAdditionalIDPattern : nt.AdditionalIDPattern;
            }

            List<string> tags = Expressions.get_existing_tags(additionalIdPattern, Expressions.Patterns.AutoTag);
            if (tags == null || tags.Count == 0)
                return string.IsNullOrEmpty(additionalIdPattern) ? null : additionalIdPattern;

            dic = new Dictionary<string, string>();

            DateTime now = time.HasValue ? time.Value : DateTime.Now;
            PersianCalendar pcal = new PersianCalendar();
            
            DateTime? lowerPersianDateLimit = PublicMethods.persian_to_gregorian_date(pcal.GetYear(now), 1, 1, 0, 0, 0);
            DateTime? upperPersianDateLimit = null;
            DateTime lowerGregorianDateLimit = new DateTime(now.Year, 1, 1, 0, 0, 0);
            DateTime upperGregorianDateLimit = lowerGregorianDateLimit.AddYears(1).AddMilliseconds(-1);

            if (lowerPersianDateLimit.HasValue)
                upperPersianDateLimit = lowerPersianDateLimit.Value.AddYears(1).AddMilliseconds(-1);

            foreach (string _str in tags)
            {
                int num = 0;
                int ind = _str.Length;
                while (ind > 0 && int.TryParse(_str[ind - 1].ToString(), out num)) ind -= 1;
                num = ind < _str.Length ? int.Parse(_str.Substring(ind)) : 0;
                string _tg = _str.Substring(0, ind).ToLower();

                switch (_tg)
                {
                    case "pyear":
                        dic[_str] = pcal.GetYear(now).ToString();
                        break;
                    case "pyears":
                        dic[_str] = pcal.GetYear(now).ToString().Substring(2);
                        break;
                    case "pmonth":
                        dic[_str] = pcal.GetMonth(now).ToString();
                        break;
                    case "pday":
                        dic[_str] = pcal.GetDayOfMonth(now).ToString();
                        break;
                    case "gyear":
                        dic[_str] = now.Year.ToString();
                        break;
                    case "gyears":
                        dic[_str] = now.Year.ToString().Substring(2);
                        break;
                    case "gmonth":
                        dic[_str] = now.Month.ToString();
                        break;
                    case "gday":
                        dic[_str] = now.Day.ToString();
                        break;
                    case "rnd":
                        dic[_str] = PublicMethods.get_random_number(num > 0 ? num : 3).ToString();
                        break;
                    case "ncount":
                        dic[_str] = PublicMethods.fit_number_to_size(
                            CNController.get_nodes_count(applicationId, archive: null).Sum(u => u.Count.Value) + 1, num);
                        break;
                    case "ncountpy":
                        dic[_str] = PublicMethods.fit_number_to_size(CNController.get_nodes_count(applicationId,
                            lowerPersianDateLimit, upperPersianDateLimit, archive: null).Sum(u => u.Count.Value) + 1, num);
                        break;
                    case "ncountgy":
                        dic[_str] = PublicMethods.fit_number_to_size(CNController.get_nodes_count(applicationId,
                            lowerGregorianDateLimit, upperGregorianDateLimit, archive: null)
                            .Sum(u => u.Count.Value) + 1, num);
                        break;
                    case "ncounts":
                        NodesCount ntc = !nodeTypeId.HasValue ? null :
                            CNController.get_nodes_count(applicationId, nodeTypeId.Value, archive: null);
                        dic[_str] = PublicMethods.fit_number_to_size((ntc == null ? 0 : ntc.Count.Value) + 1, num);
                        break;
                    case "ncountspy":
                        NodesCount ntcp = !nodeTypeId.HasValue ? null : CNController.get_nodes_count(applicationId,
                            nodeTypeId.Value, lowerPersianDateLimit, upperPersianDateLimit, archive: null);
                        dic[_str] = PublicMethods.fit_number_to_size((ntcp == null ? 0 : ntcp.Count.Value) + 1, num);
                        break;
                    case "ncountsgy":
                        NodesCount ntcg = !nodeTypeId.HasValue ? null : CNController.get_nodes_count(applicationId,
                            nodeTypeId.Value, lowerGregorianDateLimit, upperGregorianDateLimit, archive: null);
                        dic[_str] = PublicMethods.fit_number_to_size((ntcg == null ? 0 : ntcg.Count.Value) + 1, num);
                        break;
                        /*
                    case "depid":
                        NodeMember dep = CNController.get_member_nodes(applicationId, 
                            currentUserId, NodeTypes.Department).FirstOrDefault();
                        dic[_str] = dep == null || string.IsNullOrEmpty(dep.Node.AdditionalID) ? "0" : dep.Node.AdditionalID;
                        break;
                        */
                }
            } //end of 'foreach (string _str in tags)'

            return Expressions.replace(additionalIdPattern, ref dic, Expressions.Patterns.AutoTag, null);
        }

        public static string generate_new_additional_id(Guid applicationId, DateTime? time = null)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            return _generate_new_additional_id(applicationId, null, null, true, time, ref dic);
        }

        public static string generate_new_additional_id(Guid applicationId, Guid nodeTypeId, DateTime? time = null)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            return _generate_new_additional_id(applicationId, nodeTypeId, null, true, time, ref dic);
        }

        public static string generate_new_additional_id(Guid applicationId, string additionalIdPattern, DateTime? time = null)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            return _generate_new_additional_id(applicationId, null, additionalIdPattern, false, time, ref dic);
        }

        public static string generate_new_additional_id(Guid applicationId,
            Guid nodeTypeId, string additionalIdPattern, DateTime? time = null)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            return _generate_new_additional_id(applicationId, nodeTypeId, additionalIdPattern, false, time, ref dic);
        }

        public static string generate_new_additional_id(Guid applicationId, 
            Guid nodeTypeId, string additionalIdPattern, ref Dictionary<string, string> dic, DateTime? time = null)
        {
            return _generate_new_additional_id(applicationId, nodeTypeId, additionalIdPattern, false, time, ref dic);
        }

        public static List<ExtensionType> get_extension_types()
        {
            List<ExtensionType> types = new List<ExtensionType>();

            Array arr = Enum.GetValues(typeof(ExtensionType));
            for (int i = 0, lnt = arr.Length; i < lnt; ++i)
                if ((ExtensionType)arr.GetValue(i) != ExtensionType.NotSet) types.Add((ExtensionType)arr.GetValue(i));

            return types;
        }

        public static List<Extension> extend_extensions(Guid applicationId, List<Extension> extensions, bool ignoreDefault = false)
        {
            if (!ignoreDefault)
            {
                foreach (string ext in RaaiVanSettings.CoreNetwork.DefaultCNExtensions(applicationId))
                {
                    ExtensionType extType = new ExtensionType();
                    if (!Enum.TryParse<ExtensionType>(ext, out extType) || extType == ExtensionType.NotSet ||
                        extensions.Any(u => u.ExtensionType == extType)) continue;

                    extensions.Add(new Extension() { ExtensionType = extType, Disabled = false, Initialized = false });
                }
            }

            List<ExtensionType> extensionTypes = CNUtilities.get_extension_types();
            foreach (ExtensionType ext in extensionTypes)
            {
                if (extensions.Any(u => u.ExtensionType == ext)) continue;
                extensions.Add(new Extension() { ExtensionType = ext, Disabled = true, Initialized = false });
            }

            if (!RaaiVanConfig.Modules.SocialNetwork(applicationId) && extensions.Any(u => u.ExtensionType == ExtensionType.Posts))
                extensions.Remove(extensions.Where(x => x.ExtensionType == ExtensionType.Posts).FirstOrDefault());

            if (!RaaiVanConfig.Modules.FormGenerator(applicationId) && extensions.Any(u => u.ExtensionType == ExtensionType.Form))
                extensions.Remove(extensions.Where(x => x.ExtensionType == ExtensionType.Form).FirstOrDefault());

            if (!RaaiVanConfig.Modules.Documents(applicationId) && extensions.Any(u => u.ExtensionType == ExtensionType.Documents))
                extensions.Remove(extensions.Where(x => x.ExtensionType == ExtensionType.Documents).FirstOrDefault());

            if (!RaaiVanConfig.Modules.Explorer(applicationId) && extensions.Any(u => u.ExtensionType == ExtensionType.Browser))
                extensions.Remove(extensions.Where(x => x.ExtensionType == ExtensionType.Browser).FirstOrDefault());

            return extensions;
        }
    }

    public class NodeType
    {
        public Guid? NodeTypeID;
        public Guid? TemplateTypeID;
        public int? AdditionalID;
        public Guid? ParentID;
        public string NodeTypeAdditionalID;
        public string Name;
        public string Description;
        public string AdditionalIDPattern;
        public bool? HasDefaultPattern;
        public string AvatarName;
        public Guid? CreatorUserID;
        public DateTime? CreationDate;
        public Guid? LastModifierUserID;
        public DateTime? LastModificationDate;
        public bool? Hidden;
        public bool? Archive;
        public bool? HasChild;
        public string IconURL;
        public string HighQualityIconURL;
        public bool? IsService;
        public List<NodeType> Sub;

        public NodeType() {
            Sub = new List<NodeType>();
        }

        public bool IsCategory
        {
            get {
                return (!IsService.HasValue || !IsService.Value) && RaaiVanSettings.SAASBasedMultiTenancy;
            }
        }

        public string toJson(Guid? applicationId = null, bool iconUrl = false)
        {
            bool hasDefaultPattern = !HasDefaultPattern.HasValue ? true : HasDefaultPattern.Value;

            if (applicationId.HasValue && iconUrl && NodeTypeID.HasValue)
            {
                if(string.IsNullOrEmpty(IconURL))
                    IconURL = DocumentUtilities.get_icon_url(applicationId.Value, NodeTypeID.Value);
                if(string.IsNullOrEmpty(HighQualityIconURL))
                    HighQualityIconURL = DocumentUtilities.get_icon_url(applicationId.Value, NodeTypeID.Value, null, true);
            }

            return "{\"NodeTypeID\":\"" + (!NodeTypeID.HasValue ? string.Empty : NodeTypeID.ToString()) + "\"" +
                ",\"AdditionalID\":\"" + Base64.encode(NodeTypeAdditionalID) + "\"" +
                ",\"TypeName\":\"" + Base64.encode(Name) + "\"" +
                (string.IsNullOrEmpty(Description) ? string.Empty :
                    ",\"Description\":\"" + Base64.encode(Description) + "\"") +
                (!ParentID.HasValue ? string.Empty : 
                    ",\"ParentID\":\"" + ParentID.ToString() + "\"") +
                (string.IsNullOrEmpty(AdditionalIDPattern) ? string.Empty : 
                    ",\"AdditionalIDPattern\":\"" + AdditionalIDPattern + "\"") +
                (!AdditionalID.HasValue ? string.Empty : 
                    ",\"IsDefault\":" + AdditionalID.HasValue.ToString().ToLower()) +
                (!Hidden.HasValue || !Hidden.Value ? string.Empty :
                    ",\"Hidden\":" + Hidden.Value.ToString().ToLower()) +
                (!Archive.HasValue || !Archive.Value ? string.Empty : 
                    ",\"IsArchive\":" + Archive.Value.ToString().ToLower()) +
                (!hasDefaultPattern ? string.Empty : 
                    ",\"HasDefaultPattern\":" + hasDefaultPattern.ToString().ToLower()) +
                (string.IsNullOrEmpty(AvatarName) ? string.Empty : ",\"AvatarName\":" + AvatarName) +
                (string.IsNullOrEmpty(IconURL) ? string.Empty : 
                    ",\"IconURL\":\"" + IconURL + "\"") +
                (string.IsNullOrEmpty(HighQualityIconURL) ? string.Empty : 
                    ",\"HighQualityIconURL\":\"" + HighQualityIconURL + "\"") +
                (!HasChild.HasValue || !HasChild.Value ? string.Empty :
                    ",\"HasChild\":" + HasChild.Value.ToString().ToLower()) +
                (!IsService.HasValue || !IsService.Value ? string.Empty :
                    ",\"IsService\":" + IsService.Value.ToString().ToLower()) +
                (!IsCategory ? string.Empty : 
                    ",\"IsCategory\":" + IsCategory.ToString().ToLower()) +
                (Sub == null || Sub.Count == 0 ? string.Empty :
                    ",\"Sub\":[" + string.Join(",", Sub.Select(s => s.toJson(applicationId, iconUrl))) + "]") +
                "}";
        }

        private static List<NodeType> _children(List<NodeType> all, Guid? parentId = null)
        {
            List<NodeType> children = all == null ? new List<NodeType>() : 
                all.Where(a => (!parentId.HasValue && !a.ParentID.HasValue) || parentId == a.ParentID).ToList();

            children.ForEach(c => c.Sub = _children(all, c.NodeTypeID));

            return children;
        }

        public static List<NodeType> toTree(List<NodeType> all) {
            return _children(all, null);
        }
    }

    public class Service
    {
        public NodeType NodeType;
        public string Title;
        public string Description;
        public ServiceAdminType AdminType;
        public Node AdminNode;
        public int? MaxAcceptableAdminLevel;
        public List<string> LimitAttachedFilesTo;
        public int? MaxAttachedFileSize;
        public int? MaxAttachedFilesCount;
        public bool? EnableContribution;
        public bool? NoContent;
        public bool? IsKnowledge;
        public bool? IsDocument;
        public bool? EnablePreviousVersionSelect;
        public bool? IsTree;
        public bool? UniqueMembership;
        public bool? UniqueAdminMember;
        public bool? DisableAbstractAndKeywords;
        public bool? DisableFileUpload;
        public bool? DisableRelatedNodesSelect;
        public bool? EditableForAdmin;
        public bool? EditableForCreator;
        public bool? EditableForContributors;
        public bool? EditableForExperts;
        public bool? EditableForMembers;
        public bool? EditSuggestion;

        public Service()
        {
            NodeType = new NodeType();
            AdminType = new ServiceAdminType();
            AdminNode = new Node();
            LimitAttachedFilesTo = new List<string>();
        }

        public string toJson(Guid applicationId, bool isAdmin = false)
        {
            string strSuccessMessage = string.Empty;
            string strAdminNode = "null";
            string strAdminLimits = "null";
            string strContributionLimits = "null";
            string strFreeUsers = "[]";
            string strServiceAdmins = "[]";

            if (isAdmin && NodeType.NodeTypeID.HasValue)
            {
                strSuccessMessage = CNController.get_service_success_message(applicationId,
                    NodeType.NodeTypeID.Value);

                Node adminNode = null;
                if (AdminNode.NodeID.HasValue &&
                    (adminNode = CNController.get_node(applicationId, AdminNode.NodeID.Value)) != null)
                    strAdminNode = adminNode.toJson(applicationId, iconUrl: false, simple: true);

                List<NodeType> adminLimits = CNController.get_admin_area_limits(applicationId, NodeType.NodeTypeID.Value);

                strAdminLimits = "[" + string.Join(",", adminLimits.Select(
                    u => "{\"NodeTypeID\":\"" + u.NodeTypeID.ToString() + "\"" +
                            ",\"Name\":\"" + Base64.encode(u.Name) + "\"" +
                         "}")) + "]";

                List<NodeType> contributionLimits = CNController.get_contribution_limits(applicationId, NodeType.NodeTypeID.Value);

                strContributionLimits = "[" + string.Join(",", contributionLimits.Select(
                    u => "{\"NodeTypeID\":\"" + u.NodeTypeID.ToString() + "\"" +
                            ",\"Name\":\"" + Base64.encode(u.Name) + "\"" +
                         "}")) + "]";

                List<User> freeUsers = CNController.get_free_users(applicationId, NodeType.NodeTypeID.Value);
                List<User> serviceAdmins = CNController.get_service_admins(applicationId, NodeType.NodeTypeID.Value);

                strFreeUsers = "[" + string.Join(",", freeUsers.Select(u => u.toJson(applicationId, true))) + "]";
                strServiceAdmins = "[" + string.Join(",", serviceAdmins.Select(u => u.toJson(applicationId, true))) + "]";
            }

            return "{\"NodeTypeID\":\"" + NodeType.NodeTypeID.ToString() + "\"" +
                ",\"TypeName\":\"" + Base64.encode(NodeType.Name) + "\"" +
                ",\"Title\":\"" + Base64.encode(Title) + "\"" +
                ",\"Description\":\"" + Base64.encode(Description) + "\"" +
                ",\"SuccessMessage\":\"" + Base64.encode(strSuccessMessage) + "\"" +
                ",\"AdminType\":\"" + (AdminType == ServiceAdminType.NotSet ? string.Empty : AdminType.ToString()) + "\"" +
                ",\"AdminNode\":" + strAdminNode +
                ",\"AdminLimits\":" + strAdminLimits +
                ",\"MaxAcceptableAdminLevel\":\"" +
                    (MaxAcceptableAdminLevel.HasValue ? MaxAcceptableAdminLevel.ToString() : string.Empty) + "\"" +
                ",\"LimitAttachedFilesTo\":[" + ProviderUtil.list_to_string<string>(LimitAttachedFilesTo.Select(u =>
                    "\"" + Base64.encode(u) + "\"").ToList()) + "]" +
                ",\"MaxAttachedFileSize\":\"" +
                    (MaxAttachedFileSize.HasValue ? MaxAttachedFileSize.ToString() : string.Empty) + "\"" +
                ",\"MaxAttachedFilesCount\":\"" +
                    (MaxAttachedFilesCount.HasValue ? MaxAttachedFilesCount.ToString() : string.Empty) + "\"" +
                ",\"ContributionLimits\":" + strContributionLimits +
                ",\"EnableContribution\":" + (EnableContribution.HasValue ? EnableContribution : false).ToString().ToLower() +
                ",\"NoContent\":" + (NoContent.HasValue && NoContent.Value).ToString().ToLower() +
                ",\"IsKnowledge\":" + (IsKnowledge.HasValue && IsKnowledge.Value).ToString().ToLower() +
                ",\"IsDocument\":" + (IsDocument.HasValue && IsDocument.Value).ToString().ToLower() +
                ",\"EnablePreviousVersionSelect\":" + (EnablePreviousVersionSelect.HasValue && 
                    EnablePreviousVersionSelect.Value).ToString().ToLower() +
                ",\"IsTree\":" + (IsTree.HasValue && IsTree.Value).ToString().ToLower() +
                ",\"UniqueMembership\":" + (UniqueMembership.HasValue && UniqueMembership.Value).ToString().ToLower() +
                ",\"UniqueAdminMember\":" + (UniqueAdminMember.HasValue && UniqueAdminMember.Value).ToString().ToLower() +
                ",\"DisableAbstractAndKeywords\":" + (DisableAbstractAndKeywords.HasValue && DisableAbstractAndKeywords.Value).ToString().ToLower() +
                ",\"DisableFileUpload\":" + (DisableFileUpload.HasValue && DisableFileUpload.Value).ToString().ToLower() +
                ",\"DisableRelatedNodesSelect\":" + (DisableRelatedNodesSelect.HasValue && DisableRelatedNodesSelect.Value).ToString().ToLower() +
                ",\"EditableForAdmin\":" + (EditableForAdmin.HasValue && EditableForAdmin.Value).ToString().ToLower() +
                ",\"EditableForCreator\":" + (EditableForCreator.HasValue && EditableForCreator.Value).ToString().ToLower() +
                ",\"EditableForOwners\":" + (EditableForContributors.HasValue && EditableForContributors.Value).ToString().ToLower() +
                ",\"EditableForExperts\":" + (EditableForExperts.HasValue && EditableForExperts.Value).ToString().ToLower() +
                ",\"EditableForMembers\":" + (EditableForMembers.HasValue && EditableForMembers.Value).ToString().ToLower() +
                ",\"EditSuggestion\":" + (EditSuggestion.HasValue && EditSuggestion.Value).ToString().ToLower() +
                ",\"FreeUsers\":" + strFreeUsers +
                ",\"ServiceAdmins\":" + strServiceAdmins +
                ",\"IsAdmin\":" + isAdmin.ToString().ToLower() +
                ",\"IconURL\":\"" + DocumentUtilities.get_icon_url(applicationId,
                    NodeType.NodeTypeID.Value, DefaultIconTypes.None) + "\"" +
                "}";
        }
    }

    public class RelationType
    {
        public Guid? RelationTypeID;
        public int? AdditionalID;
        public string Name;
        public string Description;
        public Guid? CreatorUserID;
        public DateTime? CreationDate;
        public Guid? LastModifierUserID;
        public DateTime? LastModificationDate;

        public string toJson()
        {
            return "{\"RelationTypeID\":\"" + (!RelationTypeID.HasValue ? string.Empty : RelationTypeID.ToString()) + "\"" + 
                ",\"TypeName\":\"" + Base64.encode(Name) + "\"" + 
                ",\"IsDefault\":" + AdditionalID.HasValue.ToString().ToLower() + 
                "}";
        }
    }

    public class Node
    {
        public Guid? NodeID;
        public string AdditionalID_Main;
        public string AdditionalID;
        public Guid? NodeTypeID;
        public int? NodeTypeAdditionalID;
        public string TypeAdditionalID;
        public Guid? DocumentTreeNodeID;
        public Guid? DocumentTreeID;
        public string DocumentTreeName;
        public Guid? PreviousVersionID;
        public string PreviousVersionName;
        public string NodeType;
        public string Name;
        public string Description;
        public string PublicDescription;
        public string AvatarName;
        public User Creator;
        public DateTime? CreationDate;
        public Guid? LastModifierUserID;
        public DateTime? LastModificationDate;
        public string DepartmentGroup;
        public Guid? ParentNodeID;
        public string ParentName;
        public int? LikesCount;
        public bool? LikeStatus;
        public string MembershipStatus;
        public int? VisitsCount;
        public List<string> Tags;
        public List<NodeCreator> Contributors;
        public Guid? OwnerID;
        public string OwnerName;
        public Guid? AdminAreaID;
        public string AdminAreaName;
        public string AdminAreaType;
        public ConfidentialityLevel ConfidentialityLevel;
        public bool? Searchable;
        public bool? HideCreators;
        public DateTime? PublicationDate;
        public DateTime? ExpirationDate;
        public Status Status;
        public string WFState;
        public double? Score;
        public bool? IsFreeUser;
        public bool? Archive;
        public bool? HasWikiContent;
        public bool? HasFormContent;
        public bool? HasChild;
        
        public Node()
        {
            Tags = new List<string>();
            Creator = new User();
            Contributors = new List<NodeCreator>();
            ConfidentialityLevel = new ConfidentialityLevel();
            Status = Status.NotSet;
        }

        public bool isPersonal(Guid currentUserId)
        {
            return Creator.UserID == currentUserId && (Status == Status.NotSet ||
                Status == Status.SentBackForRevision || Status == Status.Personal || Status == Status.Rejected);
        }

        public string url(Guid applicationId)
        {
            return !NodeID.HasValue ? string.Empty :
                PublicConsts.NodePage.Replace("~", RaaiVanSettings.RaaiVanURL(applicationId)) + "/" + NodeID.ToString();
        }

        public string toJson(Guid? applicationId = null, bool iconUrl = false, bool simple = false)
        {
            bool showCreator = Creator != null && Creator.UserID.HasValue && !string.IsNullOrEmpty(Creator.UserName) &&
                !(HideCreators.HasValue && HideCreators.Value) && (Status == Status.NotSet || Status == Status.Accepted);

            return "{\"NodeID\":\"" + (!NodeID.HasValue ? string.Empty : NodeID.Value.ToString()) + "\"" +
                ",\"AdditionalID\":\"" + Base64.encode(AdditionalID) + "\"" +
                ",\"NodeTypeID\":\"" + (!NodeTypeID.HasValue ? string.Empty : NodeTypeID.Value.ToString()) + "\"" +
                ",\"Name\":\"" + Base64.encode(Name) + "\"" +
                ",\"NodeType\":\"" + Base64.encode(NodeType) + "\"" +
                (string.IsNullOrEmpty(AvatarName) ? string.Empty : ",\"AvatarName\":" + AvatarName) +
                (simple ? string.Empty : 
                    ",\"CreationDate\":\"" + (CreationDate.HasValue ? GenericDate.get_local_date(CreationDate.Value) : string.Empty) + "\"" +
                    ",\"Status\":\"" + (Status == Status.NotSet ? string.Empty : Status.ToString()) + "\"" +
                    ",\"WFState\":\"" + Base64.encode(WFState) + "\"" +
                    ",\"HideCreators\":" + (HideCreators.HasValue && HideCreators.Value).ToString().ToLower() +
                    (VisitsCount.HasValue ? ",\"VisitsCount\":" + VisitsCount.Value.ToString() : string.Empty) +
                    (LikesCount.HasValue ? ",\"LikesCount\":" + LikesCount.Value.ToString() : string.Empty) +
                    ",\"HasChild\":" + (!HasChild.HasValue ? "null" : HasChild.Value.ToString().ToLower()) +
                    (!iconUrl || !applicationId.HasValue || !NodeID.HasValue ? string.Empty :
                        ",\"IconURL\":\"" + DocumentUtilities.get_icon_url(applicationId.Value,
                            NodeID.Value, DefaultIconTypes.Node, NodeTypeID) + "\"") +
                    (!showCreator ? string.Empty :  ",\"Creator\":" + Creator.toJson(applicationId, true)) +
                    ",\"Archived\":" + (Archive.HasValue && Archive.Value).ToString().ToLower()
                ) +
                "}";
        }
    }

    public class Relation
    {
        private Node _Source;
        private Node _Destination;
        private RelationType _RelationType;
        private bool? _Bidirectional;

        public Relation()
        {
            _Source = new Node();
            _Destination = new Node();
            _RelationType = new RelationType();
        }

        public Node Source
        {
            get { return _Source; }
            set { _Source = value; }
        }

        public Node Destination
        {
            get { return _Destination; }
            set { _Destination = value; }
        }

        public RelationType RelationType
        {
            get { return _RelationType; }
            set { _RelationType = value; }
        }

        public bool? Bidirectional
        {
            get { return _Bidirectional; }
            set { _Bidirectional = value; }
        }
    }

    public class NodeMember
    {
        public Node Node;
        public User Member;
        public DateTime? MembershipDate;
        public bool? IsAdmin;
        public bool? IsPending;
        public string Status;
        public DateTime? AcceptionDate;
        public string Position;

        public NodeMember()
        {
            Node = new Node();
            Member = new User();
        }

        public string toJson(Guid applicationId) {
            return "{\"UserID\":\"" + Member.UserID.ToString() + "\"" +
                ",\"FirstName\":\"" + Base64.encode(Member.FirstName) + "\"" +
                ",\"LastName\":\"" + Base64.encode(Member.LastName) + "\"" +
                ",\"UserName\":\"" + Base64.encode(Member.UserName) + "\"" +
                ",\"IsAdmin\":" + (IsAdmin.HasValue && IsAdmin.Value).ToString().ToLower() +
                ",\"IsPending\":" + (IsPending.HasValue && IsPending.Value).ToString().ToLower() +
                ",\"Status\":\"" + (string.IsNullOrEmpty(Status) ?
                    string.Empty : Status.ToString()) + "\"" +
                ",\"ImageURL\":\"" + DocumentUtilities.get_personal_image_address(applicationId, Member.UserID.Value) + "\"" +
                "}";
        }
    }

    public class NodesCount
    {
        public int? Order;
        public int? ReverseOrder;

        public Guid? NodeTypeID;
        public string NodeTypeAdditionalID;
        public string TypeName;
        public int? Count;

        public string toJson() {
            return "{\"NodeTypeID\":\"" + (NodeTypeID.HasValue ? NodeTypeID.ToString() : string.Empty) + "\"" +
                ",\"TypeName\":\"" + Base64.encode(TypeName) + "\"" + 
                ",\"Count\":" + (Count.HasValue ? Count.Value : 0).ToString() +
                "}";
        }
    }

    public class NodeList
    {
        public Guid? ListID;
        public string AdditionalID;
        public Guid? NodeTypeID;
        public string NodeType;
        public string Name;
        public string Description;
        public Guid? CreatorUserID;
        public DateTime? CreationDate;
        public Guid? LastModifierUserID;
        public DateTime? LastModificationDate;
        public Guid? ParentListID;
        public string ParentName;
        public Guid? OwnerID;
        public string OwnerType;
    }

    public class HierarchyAdmin
    {
        public Node Node;
        public User User;
        public int? Level;


        public HierarchyAdmin()
        {
            Node = new Node();
            User = new User();
        }
    }

    public class Tag
    {
        public Guid? TagID;
        public string Text;
        public bool? Approved;
        public int? CallsCount;
        public Guid? CreatorUserID;
        public DateTime? CreationDate;
    }

    public class NodeCreator
    {
        public Guid? NodeID;
        public User User;
        public double? CollaborationShare;
        public string Status;
        public Guid? CreatorUserID;
        public DateTime? CreationDate;
        public Guid? LastModifierUserID;
        public DateTime? LastModificationDate;

        public NodeCreator()
        {
            User = new User();
        }
    }

    public class Expert
    {
        public Node Node;
        public User User;
        public bool? Approved;
        public bool? SocialApproved;
        public int? ReferralsCount;
        public double? ConfirmsPercentage;

        public Expert()
        {
            Node = new Node();
            User = new User();
        }

        public string toJson(Guid applicationId) {
            return "{\"UserID\":\"" + User.UserID.ToString() + "\"" +
                ",\"FirstName\":\"" + Base64.encode(User.FirstName) + "\"" +
                ",\"LastName\":\"" + Base64.encode(User.LastName) + "\"" +
                ",\"UserName\":\"" + Base64.encode(User.UserName) + "\"" +
                ",\"ImageURL\":\"" + DocumentUtilities.get_personal_image_address(applicationId, User.UserID.Value) + "\"" +
                "}";
        }
    }

    [Serializable]
    public class Extension
    {
        public Guid? OwnerID;
        public ExtensionType ExtensionType;
        public string Title;
        public bool? Disabled;
        public bool? Initialized;


        public Extension()
        {
            ExtensionType = new ExtensionType();
        }

        public string toJson() {
            return "{\"Extension\":\"" + ExtensionType.ToString() + "\"" +
                ",\"Title\":\"" + Base64.encode(Title) + "\"" +
                ",\"Disabled\":" + (Disabled.HasValue ? Disabled : true).ToString().ToLower() +
                ",\"Initialized\":" + (Initialized.HasValue ? Initialized : false).ToString().ToLower() +
                "}";
        }
    }

    public class NodeInfo
    {
        public NodeInfo()
        {
            Tags = new List<string>();
            Creator = new User();
        }

        public Guid? NodeID;
        public Guid? NodeTypeID;
        public List<string> Tags;
        public string Description;
        public User Creator;
        public int? ContributorsCount;
        public int? LikesCount;
        public int? VisitsCount;
        public int? ExpertsCount;
        public int? MembersCount;
        public int? ChildsCount;
        public int? RelatedNodesCount;
        public bool? LikeStatus;
    }

    public class ExploreItem
    {
        public Guid? BaseID;
        public Guid? BaseTypeID;
        public string BaseName;
        public string BaseType;
        public Guid? RelatedID;
        public Guid? RelatedTypeID;
        public string RelatedName;
        public string RelatedType;
        public DateTime? RelatedCreationDate;
        public bool? IsTag;
        public bool? IsRelation;
        public bool? IsRegistrationArea;
    }

    public class SimilarNode
    {
        public Node Suggested;
        public double? Rank;
        public bool? Tags;
        public bool? Favorites;
        public bool? Relations;
        public bool? Experts;

        public SimilarNode() {
            Suggested = new CoreNetwork.Node();
        }
    }

    public class KnowledgableUser
    {
        public User User;
        public double? Rank;
        public bool? Expert;
        public bool? Contributor;
        public bool? WikiEditor;
        public bool? Member;
        public bool? ExpertOfRelatedNode;
        public bool? ContributorOfRelatedNode;
        public bool? MemberOfRelatedNode;

        public KnowledgableUser()
        {
            User = new User();
        }
    }
}
