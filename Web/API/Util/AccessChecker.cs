using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RaaiVan.Modules.Users;
using RaaiVan.Modules.CoreNetwork;
using RaaiVan.Modules.NotificationCenter;
using RaaiVan.Modules.WorkFlow;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.Privacy;

namespace RaaiVan.Web.API
{
    public class AccessChecker
    {
        Guid? ApplicationID;
        Guid? CurrentUserID;
        Guid? NodeIDOrNodeTypeID;

        Node Node = null;
        NodeType NodeType = null;
        Service Service = null;
        List<NodeCreator> Contributors = null;

        bool? _IsSystemAdmin = null;
        Dictionary<Guid, bool> _IsServiceAdmin = new Dictionary<Guid, bool>();
        Dictionary<Guid, bool> _IsAreaAdmin = new Dictionary<Guid, bool>();
        Dictionary<Guid, bool> _IsCreator = new Dictionary<Guid, bool>();
        Dictionary<Guid, bool> _IsContributor = new Dictionary<Guid, bool>();
        Dictionary<Guid, bool> _IsExpert = new Dictionary<Guid, bool>();
        Dictionary<Guid, bool> _IsMember = new Dictionary<Guid, bool>();
        Dictionary<Guid, bool> _IsAdminMember = new Dictionary<Guid, bool>();
        Dictionary<Guid, bool> _HasNodeEditAccess = new Dictionary<Guid, bool>();
        Dictionary<Guid, bool> _EditSuggestionEnabled = new Dictionary<Guid, bool>();
        Dictionary<Guid, bool> _IsKnowledge = new Dictionary<Guid, bool>();
        Dictionary<Guid, bool> _HasKnowledgeWorkflowPermission = new Dictionary<Guid, bool>();
        Dictionary<Guid, bool> _HasWorkflowPermission = new Dictionary<Guid, bool>();
        Dictionary<AccessRoleName, bool> _HasAccessRole = new Dictionary<AccessRoleName, bool>();

        public AccessChecker(Guid? applicationId, Guid? currentUserId, Guid? nodeIdOrNodeTypeId = null,
            Node node = null, NodeType nodeType = null, Service service = null, List<NodeCreator> contributors = null)
        {
            ApplicationID = applicationId;
            CurrentUserID = currentUserId;
            NodeIDOrNodeTypeID = nodeIdOrNodeTypeId;
            Node = node;
            NodeType = nodeType;
            Contributors = contributors;

            if (!NodeIDOrNodeTypeID.HasValue && Node != null) NodeIDOrNodeTypeID = Node.NodeID;
            else if (!NodeIDOrNodeTypeID.HasValue && NodeType != null) NodeIDOrNodeTypeID = NodeType.NodeTypeID;
            else if (!NodeIDOrNodeTypeID.HasValue && Service != null && Service.NodeType != null) NodeIDOrNodeTypeID = NodeType.NodeTypeID;
        }

        private Node getNode()
        {
            if (!ApplicationID.HasValue) return null;
            else if (!NodeIDOrNodeTypeID.HasValue && (Node == null || !Node.NodeID.HasValue)) return null;
            else if (Node == null && NodeIDOrNodeTypeID.HasValue)
            {
                Node = CNController.get_node(ApplicationID.Value, NodeIDOrNodeTypeID.Value);
                if (Node == null) Node = new Node();
            }

            return Node == null || !Node.NodeID.HasValue ? null : Node;
        }

        private NodeType getNodeType()
        {
            if (!ApplicationID.HasValue) return null;
            else if (!NodeIDOrNodeTypeID.HasValue && (NodeType == null || !NodeType.NodeTypeID.HasValue)) return null;
            else if (NodeType == null && NodeIDOrNodeTypeID.HasValue)
            {
                NodeType = CNController.get_node_type(ApplicationID.Value, NodeIDOrNodeTypeID.Value);
                if (NodeType == null) CNController.get_node_type_by_node_id(ApplicationID.Value, NodeIDOrNodeTypeID.Value);
                if (NodeType == null) NodeType = new NodeType();
            }

            return NodeType == null || !NodeType.NodeTypeID.HasValue ? null : NodeType;
        }

        private Service getService()
        {
            if (!ApplicationID.HasValue) return null;
            else if (!NodeIDOrNodeTypeID.HasValue &&
                (Service == null || Service.NodeType == null || !Service.NodeType.NodeTypeID.HasValue)) return null;
            else if (Service == null && NodeIDOrNodeTypeID.HasValue)
            {
                Service = CNController.get_service(ApplicationID.Value, NodeIDOrNodeTypeID.Value);
                if (Service == null) Service = new Service();
            }

            return Service == null || Service.NodeType == null || !Service.NodeType.NodeTypeID.HasValue ? null : Service;
        }

        public bool isSystemAdmin()
        {
            if (!_IsSystemAdmin.HasValue) _IsSystemAdmin = ApplicationID.HasValue && CurrentUserID.HasValue &&
                    PublicMethods.is_system_admin(ApplicationID, CurrentUserID.Value);

            return _IsSystemAdmin.Value;
        }

        public bool isServiceAdmin(Guid? nodeTypeIdOrNodeId = null)
        {
            if (nodeTypeIdOrNodeId == null) nodeTypeIdOrNodeId = NodeIDOrNodeTypeID;

            if (!ApplicationID.HasValue || !CurrentUserID.HasValue || !nodeTypeIdOrNodeId.HasValue) return false;

            if (!_IsServiceAdmin.ContainsKey(nodeTypeIdOrNodeId.Value))
            {
                _IsServiceAdmin[nodeTypeIdOrNodeId.Value] =
                    CNController.is_service_admin(ApplicationID.Value, nodeTypeIdOrNodeId.Value, CurrentUserID.Value);
            }

            return _IsServiceAdmin[nodeTypeIdOrNodeId.Value];
        }

        public bool isAreaAdmin() { return calculateUser2NodeStatus(_IsAreaAdmin); }

        public bool isCreator() { return calculateUser2NodeStatus(_IsCreator); }

        public bool isContributor() { return calculateUser2NodeStatus(_IsContributor); }

        public bool isExpert() { return calculateUser2NodeStatus(_IsExpert); }

        public bool isMember() { return calculateUser2NodeStatus(_IsMember); }

        public bool isAdminMember() { return calculateUser2NodeStatus(_IsAdminMember); }

        public bool hasNodeEditAccess() { return calculateUser2NodeStatus(_HasNodeEditAccess); }

        public bool editSuggestionEnabled() { return calculateUser2NodeStatus(_EditSuggestionEnabled); }

        public bool isKnowledge(Guid? nodeTypeIdOrNodeId = null)
        {
            if (nodeTypeIdOrNodeId == null) nodeTypeIdOrNodeId = NodeIDOrNodeTypeID;

            if (!ApplicationID.HasValue || !nodeTypeIdOrNodeId.HasValue || 
                !Modules.RaaiVanConfig.Modules.Knowledge(ApplicationID)) return false;

            if (!_IsKnowledge.ContainsKey(nodeTypeIdOrNodeId.Value))
                _IsKnowledge[nodeTypeIdOrNodeId.Value] = CNController.is_knowledge(ApplicationID.Value, nodeTypeIdOrNodeId.Value);

            return _IsKnowledge[nodeTypeIdOrNodeId.Value];
        }

        public bool hasKnowledgeWorkflowPermission()
        {
            if (!ApplicationID.HasValue || !CurrentUserID.HasValue ||
                !NodeIDOrNodeTypeID.HasValue || getNode() == null || !isKnowledge(NodeIDOrNodeTypeID)) return false;

            if (_HasKnowledgeWorkflowPermission.ContainsKey(NodeIDOrNodeTypeID.Value))
                return _HasKnowledgeWorkflowPermission[NodeIDOrNodeTypeID.Value];

            bool isDirector = isSystemAdmin() || isServiceAdmin(NodeIDOrNodeTypeID.Value) || isAreaAdmin() || isCreator();

            List<Status> inWorkflowStatuses = new List<Status>()
            {
                Status.SentToAdmin,
                Status.SentToEvaluators
            };

            if (isDirector && inWorkflowStatuses.Any(s => s == getNode().Status))
                return (_HasKnowledgeWorkflowPermission[NodeIDOrNodeTypeID.Value] = true);

            List<Status> beforeWorkflowStatuses = new List<Status>()
            {
                Status.NotSet,
                Status.Personal,
                Status.SentBackForRevision,
                Status.Rejected
            };

            List<Status> afterWorkflowStatuses = new List<Status>()
            {
                Status.Accepted,
                Status.Rejected
            };

            bool beforeWorkflow = beforeWorkflowStatuses.Any(s => s == getNode().Status) && isDirector;
            bool afterWorkflow = afterWorkflowStatuses.Any(s => s == getNode().Status) && isDirector;

            bool inWorkFlow = !beforeWorkflow && !afterWorkflow && isDirector &&
                NotificationController.dashboard_exists(ApplicationID.Value,
                userId: null, NodeIDOrNodeTypeID.Value, DashboardType.Knowledge, subType: null, seen: null, done: false);

            _HasKnowledgeWorkflowPermission[NodeIDOrNodeTypeID.Value] = beforeWorkflow || afterWorkflow || inWorkFlow ||
                NotificationController.dashboard_exists(ApplicationID.Value,
                CurrentUserID.Value, NodeIDOrNodeTypeID.Value, DashboardType.Knowledge, null, null, false) ||
                (getNode().Status == Status.SentToEvaluators && NotificationController.dashboard_exists(ApplicationID.Value,
                CurrentUserID.Value, NodeIDOrNodeTypeID.Value, DashboardType.Knowledge, DashboardSubType.Evaluator, seen: null, done: null));

            return _HasKnowledgeWorkflowPermission[NodeIDOrNodeTypeID.Value];
        }

        public bool hasWorkflowPermission(ref bool hideContributors, ref bool editPermission)
        {
            if (!ApplicationID.HasValue || !CurrentUserID.HasValue || !NodeIDOrNodeTypeID.HasValue || getNode() == null ||
                !Modules.RaaiVanConfig.Modules.WorkFlow(ApplicationID.Value)) return false;

            if (_HasWorkflowPermission.ContainsKey(NodeIDOrNodeTypeID.Value))
                return _HasWorkflowPermission[NodeIDOrNodeTypeID.Value];

            bool isTerminated = WFController.is_terminated(ApplicationID.Value, NodeIDOrNodeTypeID.Value);

            ViewerStatus viewerStatus = WFController.get_viewer_status(ApplicationID.Value, CurrentUserID.Value, NodeIDOrNodeTypeID.Value);

            if (!(isSystemAdmin() || isServiceAdmin(NodeIDOrNodeTypeID.Value)) && viewerStatus != ViewerStatus.Owner && isTerminated)
                viewerStatus = ViewerStatus.None;

            if (viewerStatus != ViewerStatus.NotInWorkFlow && (isSystemAdmin() || isServiceAdmin(NodeIDOrNodeTypeID.Value)))
                viewerStatus = ViewerStatus.Director;

            if (viewerStatus != ViewerStatus.Owner && !isTerminated)
            {
                History history = WFController.get_last_history(ApplicationID.Value, NodeIDOrNodeTypeID.Value);

                State state = history == null ? null : WFController.get_workflow_state(ApplicationID.Value,
                    history.WorkFlowID.Value, history.State.StateID.Value);

                hideContributors = state != null && state.HideOwnerName.HasValue && state.HideOwnerName.Value;

                editPermission = viewerStatus == ViewerStatus.Director && state != null &&
                    state.EditPermission.HasValue && state.EditPermission.Value;
            }

            _HasWorkflowPermission[NodeIDOrNodeTypeID.Value] = viewerStatus != ViewerStatus.NotInWorkFlow && viewerStatus != ViewerStatus.None;

            return _HasWorkflowPermission[NodeIDOrNodeTypeID.Value];
        }

        private bool calculateUser2NodeStatus(Dictionary<Guid, bool> dic)
        {
            if (!NodeIDOrNodeTypeID.HasValue) return false;
            else if (dic != null && dic.ContainsKey(NodeIDOrNodeTypeID.Value)) return dic[NodeIDOrNodeTypeID.Value];

            bool isCreator = false, isContributor = false, isExpert = false, isMember = false, isAdminMember = false,
                isServiceAdmin = false, isAreaAdmin = false, editable = false, editSuggestion = false, noOptionSet = false;

            if (ApplicationID.HasValue && CurrentUserID.HasValue && NodeIDOrNodeTypeID.HasValue)
            {
                CNController.get_user2node_status(ApplicationID.Value, CurrentUserID.Value, NodeIDOrNodeTypeID.Value,
                    ref isCreator, ref isContributor, ref isExpert, ref isMember, ref isAdminMember, ref isServiceAdmin,
                    ref isAreaAdmin, ref editable, ref noOptionSet, ref editSuggestion, Service, Contributors);
            }

            _IsServiceAdmin[NodeIDOrNodeTypeID.Value] = isServiceAdmin;
            _IsAreaAdmin[NodeIDOrNodeTypeID.Value] = isAreaAdmin;
            _IsCreator[NodeIDOrNodeTypeID.Value] = isCreator;
            _IsContributor[NodeIDOrNodeTypeID.Value] = isContributor;
            _IsExpert[NodeIDOrNodeTypeID.Value] = isExpert;
            _IsMember[NodeIDOrNodeTypeID.Value] = isMember;
            _IsAdminMember[NodeIDOrNodeTypeID.Value] = isAdminMember;
            _HasNodeEditAccess[NodeIDOrNodeTypeID.Value] = editable;
            _EditSuggestionEnabled[NodeIDOrNodeTypeID.Value] = editSuggestion;

            return dic != null && dic.ContainsKey(NodeIDOrNodeTypeID.Value) && dic[NodeIDOrNodeTypeID.Value];
        }

        public List<AccessRoleName> checkAccessRole(List<AccessRoleName> roleNames)
        {
            if (!CurrentUserID.HasValue || roleNames == null) return new List<AccessRoleName>();

            roleNames = roleNames.Where(r => r != AccessRoleName.None).Distinct().ToList();

            List<AccessRoleName> existing = roleNames.Where(r => _HasAccessRole.ContainsKey(r)).ToList();
            List<AccessRoleName> notExist = roleNames.Where(r => !_HasAccessRole.ContainsKey(r)).ToList();

            if (notExist.Count > 0)
            {
                List<AccessRoleName> result = AuthorizationManager.has_right(roleNames, CurrentUserID);

                notExist.ForEach(r => _HasAccessRole[r] = result != null && result.Any(x => x == r));
            }

            return roleNames.Where(r => _HasAccessRole.ContainsKey(r) && _HasAccessRole[r]).ToList();
        }

        public bool checkAccessRole(AccessRoleName roleName)
        {
            return checkAccessRole(new List<AccessRoleName>() { roleName }).Count > 0;
        }

        private bool checkAdminLevel(AdminLevel level)
        {
            bool isAdmin = isSystemAdmin();
            if (level == AdminLevel.System) return isAdmin;
            if (!isAdmin) isAdmin = isServiceAdmin(NodeIDOrNodeTypeID);
            if (level == AdminLevel.Service) return isAdmin;
            if (!isAdmin) isAdmin = isAreaAdmin() || isAdminMember();
            if (level == AdminLevel.Node) return isAdmin;
            if (!isAdmin) isAdmin = isCreator() || isContributor();

            return isAdmin;
        }

        public bool checkNodeEditAccess(AdminLevel level, bool? checkWorkFlowEditPermission = false,
            AccessRoleName roleName = AccessRoleName.None, PrivacyObjectType privacyObjectType = PrivacyObjectType.None, 
            List<PermissionType> permissions = null)
        {
            if (permissions != null) permissions = permissions.Where(p => p != PermissionType.None).ToList();

            if (checkAccessRole(roleName)) return true;

            bool isAdmin =  checkAdminLevel(level);

            if (!isAdmin && checkWorkFlowEditPermission.HasValue && checkWorkFlowEditPermission.Value)
            {
                bool hideContributors = false;
                hasWorkflowPermission(ref hideContributors, ref isAdmin);
            }

            if (!isAdmin && ApplicationID.HasValue && NodeIDOrNodeTypeID.HasValue && permissions != null &&
                permissions.Count > 0 /* && privacyObjectType != PrivacyObjectType.None */) //even none permission matters!
                isAdmin = PrivacyController.check_access(applicationId: ApplicationID.Value, userId: CurrentUserID,
                    objectId: NodeIDOrNodeTypeID.Value, objectType: privacyObjectType, permissions: permissions).Count > 0;

            return isAdmin;
        }

        public bool checkNodeEditAccess(AdminLevel level, bool? checkWorkFlowEditPermission = false,
            AccessRoleName roleName = AccessRoleName.None, PrivacyObjectType privacyObjectType = PrivacyObjectType.None,
            PermissionType permission = PermissionType.None)
        {
            return checkNodeEditAccess(level, checkWorkFlowEditPermission, roleName,
                privacyObjectType, new List<PermissionType>() { permission });
        }
    }
}