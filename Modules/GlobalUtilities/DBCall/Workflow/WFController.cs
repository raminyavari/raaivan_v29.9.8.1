using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RaaiVan.Modules.CoreNetwork;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.GlobalUtilities.DBCompositeTypes;

namespace RaaiVan.Modules.WorkFlow
{
    public class WFController
    {
        private static ModuleIdentifier GetModuleQualifier()
        {
            return ModuleIdentifier.WF;
        }

        private static string GetFullyQualifiedName(string name)
        {
            return "[dbo]." + "[" + GetModuleQualifier().ToString() + "_" + name + "]"; //'[dbo].' is database owner
        }

        public static bool create_state(Guid applicationId, State info)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("CreateState"),
                applicationId, info.StateID, info.Title, info.CreatorUserID, DateTime.Now);
        }

        public static bool modify_state(Guid applicationId, State info)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ModifyState"),
                applicationId, info.StateID, info.Title, info.LastModifierUserID, DateTime.Now);
        }

        public static bool remove_state(Guid applicationId, Guid stateId, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ArithmeticDeleteState"),
                applicationId, stateId, currentUserId, DateTime.Now);
        }

        public static List<State> get_states(Guid applicationId, Guid? workflowId = null)
        {
            return WFParsers.states(DBConnector.read(applicationId, GetFullyQualifiedName("GetStates"), applicationId, workflowId));
        }

        public static List<State> get_states(Guid applicationId, List<Guid> stateIds)
        {
            return WFParsers.states(DBConnector.read(applicationId, GetFullyQualifiedName("GetStatesByIDs"),
                applicationId, ProviderUtil.list_to_string<Guid>(stateIds), ','));
        }

        public static State get_state(Guid applicationId, Guid stateId)
        {
            return get_states(applicationId, new List<Guid>() { stateId }).FirstOrDefault();
        }

        public static bool create_workflow(Guid applicationId, WorkFlow info)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("CreateWorkFlow"),
                applicationId, info.WorkFlowID, info.Name, info.Description, info.CreatorUserID, DateTime.Now);
        }

        public static bool modify_workflow(Guid applicationId, WorkFlow info)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ModifyWorkFlow"),
                applicationId, info.WorkFlowID, info.Name, info.Description, info.LastModifierUserID, DateTime.Now);
        }

        public static bool remove_workflow(Guid applicationId, Guid workflowId, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ArithmeticDeleteWorkFlow"),
                applicationId, workflowId, currentUserId, DateTime.Now);
        }

        public static List<WorkFlow> get_workflows(Guid applicationId)
        {
            return WFParsers.workflows(DBConnector.read(applicationId, GetFullyQualifiedName("GetWorkFlows"), applicationId));
        }

        public static List<WorkFlow> get_workflows(Guid applicationId, List<Guid> workflowIds)
        {
            return WFParsers.workflows(DBConnector.read(applicationId, GetFullyQualifiedName("GetWorkFlowsByIDs"),
                applicationId, ProviderUtil.list_to_string<Guid>(workflowIds), ','));
        }

        public static WorkFlow get_workflow(Guid applicationId, Guid workflowId)
        {
            return get_workflows(applicationId, new List<Guid>() { workflowId }).FirstOrDefault();
        }

        public static bool add_workflow_state(Guid applicationId, Guid? id, Guid workflowId, Guid stateId, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("AddWorkFlowState"),
                applicationId, id, workflowId, stateId, currentUserId, DateTime.Now);
        }

        public static bool remove_workflow_state(Guid applicationId, Guid workflowId, Guid stateId, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ArithmeticDeleteWorkFlowState"),
                applicationId, workflowId, stateId, currentUserId, DateTime.Now);
        }

        public static bool set_workflow_state_description(Guid applicationId, State info)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetWorkFlowStateDescription"),
                applicationId, info.WorkFlowID, info.StateID, info.Description, info.LastModifierUserID, DateTime.Now);
        }

        public static bool set_workflow_state_tag(Guid applicationId, Guid workflowId, Guid stateId, string tag, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetWorkFlowStateTag"),
                applicationId, workflowId, stateId, tag, currentUserId, DateTime.Now);
        }

        public static bool remove_workflow_state_tag(Guid applicationId, Guid workflowId, Guid stateId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("RemoveWorkFlowStateTag"),
                applicationId, workflowId, stateId);
        }

        public static List<Tag> get_workflow_tags(Guid applicationId, Guid workflowId)
        {
            return WFParsers.tags(DBConnector.read(applicationId, GetFullyQualifiedName("GetWorkFlowTags"), 
                applicationId, workflowId));
        }

        public static bool save_workflow_actions(Guid applicationId,
            Guid connectionId, List<WorkFlowAction> actions, Guid currentUserId)
        {
            if (actions == null) actions = new List<WorkFlowAction>();

            DBCompositeType<StringTableType> actionsParam = StringTableType.getCompositeType(
                actions.Where(a => a != WorkFlowAction.None).Select(a => a.ToString()).ToList());

            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SaveWorkFlowActions"),
                applicationId, connectionId, actionsParam, currentUserId, DateTime.Now);
        }

        private static List<StateConnection> _get_workflow_actions(Guid applicationId, Guid? workflowId, Guid? connectionId)
        {
            return WFParsers.state_actions(DBConnector.read(applicationId, GetFullyQualifiedName("GetWorkFlowActions"),
                    applicationId, workflowId, connectionId));
        }

        public static List<StateConnection> get_workflow_actions(Guid applicationId, Guid workflowId)
        {
            return _get_workflow_actions(applicationId, workflowId, connectionId: null);
        }

        public static List<WorkFlowAction> get_connection_actions(Guid applicationId, Guid connectionId)
        {
            StateConnection sc = _get_workflow_actions(applicationId, workflowId: null, connectionId: connectionId).FirstOrDefault();
            return sc == null || sc.Actions == null ? new List<WorkFlowAction>() : sc.Actions;
        }

        public static List<WorkFlowAction> get_next_state_actions(Guid applicationId, Guid historyId, Guid nextStateId)
        {
            StateConnection sc = WFParsers.state_actions(DBConnector.read(applicationId, 
                GetFullyQualifiedName("GetNextStateActions"), applicationId, historyId, nextStateId)).FirstOrDefault();
            return sc == null || sc.Actions == null ? new List<WorkFlowAction>() : sc.Actions;
        }

        public static bool set_state_director(Guid applicationId, State info)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetStateDirector"),
                applicationId, info.WorkFlowID, info.StateID, info.ResponseType, info.RefStateID, 
                info.DirectorNode.NodeID, info.DirectorIsAdmin, info.DirectorUser.UserID, info.CreatorUserID, DateTime.Now);
        }

        public static bool set_state_poll(Guid applicationId, Guid workflowId, Guid stateId, Guid? pollId, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetStatePoll"),
                applicationId, workflowId, stateId, pollId, currentUserId, DateTime.Now);
        }

        public static bool set_state_data_needs_type(Guid applicationId, Guid workflowId, Guid stateId, 
            StateDataNeedsTypes? dataNeedsType, Guid refStateId, Guid currentUserId)
        {
            string strDataNeedsType = null;
            if (dataNeedsType.HasValue) strDataNeedsType = dataNeedsType.Value.ToString();

            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetStateDataNeedsType"),
                applicationId, workflowId, stateId, strDataNeedsType, refStateId, currentUserId, DateTime.Now);
        }

        public static bool set_state_data_needs_description(Guid applicationId, 
            Guid workflowId, Guid stateId, string description, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetStateDataNeedsDescription"),
                applicationId, workflowId, stateId, description, currentUserId, DateTime.Now);
        }

        public static bool set_state_description_needed(Guid applicationId, 
            Guid workflowId, Guid stateId, bool descriptionNeeded, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetStateDescriptionNeeded"),
                applicationId, workflowId, stateId, descriptionNeeded, currentUserId, DateTime.Now);
        }

        public static bool set_state_hide_owner_name(Guid applicationId, 
            Guid workflowId, Guid stateId, bool hideOwnerName, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetStateHideOwnerName"),
                applicationId, workflowId, stateId, hideOwnerName, currentUserId, DateTime.Now);
        }

        public static bool set_state_edit_permisison(Guid applicationId, 
            Guid workflowId, Guid stateId, bool editPermission, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetStateEditPermission"),
                applicationId, workflowId, stateId, editPermission, currentUserId, DateTime.Now);
        }

        public static bool set_free_data_need_requests(Guid applicationId, 
            Guid workflowId, Guid stateId, bool freeDataNeedRequests, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetFreeDataNeedRequests"),
                applicationId, workflowId, stateId, freeDataNeedRequests, currentUserId, DateTime.Now);
        }

        public static bool set_state_data_need(Guid applicationId, StateDataNeed info, Guid? previousNodeTypeId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetStateDataNeed"),
                applicationId, info.ID, info.WorkFlowID, info.StateID, info.DirectorNodeType.NodeTypeID, previousNodeTypeId,
                info.FormID, info.Description, info.MultiSelect, info.Admin, info.Necessary, info.CreatorUserID, DateTime.Now);
        }

        public static bool remove_state_data_need(Guid applicationId, StateDataNeed info)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ArithmeticDeleteStateDataNeed"),
                applicationId, info.WorkFlowID, info.StateID, info.DirectorNodeType.NodeTypeID, info.LastModifierUserID, DateTime.Now);
        }

        public static bool set_rejection_settings(Guid applicationId, State info)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetRejectionSettings"),
                applicationId, info.WorkFlowID, info.StateID, info.MaxAllowedRejections, info.RejectionTitle,
                info.RejectionRefStateID, info.LastModifierUserID, DateTime.Now);
        }

        public static bool set_max_allowed_rejections(Guid applicationId, State info)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetMaxAllowedRejections"),
                applicationId, info.WorkFlowID, info.StateID, info.MaxAllowedRejections, info.LastModifierUserID, DateTime.Now);
        }

        public static int get_rejections_count(Guid applicationId, Guid ownerId, Guid workflowId, Guid stateId)
        {
            return DBConnector.get_int(applicationId, GetFullyQualifiedName("GetRejectionsCount"),
                applicationId, ownerId, workflowId, stateId);
        }

        public static Guid? add_state_connection(Guid applicationId, StateConnection info)
        {
            return DBConnector.get_guid(applicationId, GetFullyQualifiedName("AddStateConnection"),
                applicationId, info.WorkFlowID, info.InState.StateID, info.OutState.StateID, info.CreatorUserID, DateTime.Now);
        }

        public static bool sort_state_connections(Guid applicationId, List<Guid> ids)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SortStateConnections"),
                applicationId, ProviderUtil.list_to_string<Guid>(ids), ',');
        }

        public static bool move_state_connection(Guid applicationId, StateConnection Info, bool moveDown)
        {
            if (!Info.CreationDate.HasValue) Info.CreationDate = DateTime.Now;

            return DBConnector.succeed(applicationId, GetFullyQualifiedName("MoveStateConnection"),
                applicationId, Info.WorkFlowID, Info.InState.StateID, Info.OutState.StateID, moveDown);
        }

        public static bool remove_state_connection(Guid applicationId, StateConnection info)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ArithmeticDeleteStateConnection"),
                applicationId, info.WorkFlowID, info.InState.StateID, info.OutState.StateID, info.LastModifierUserID, DateTime.Now);
        }

        public static bool set_state_connection_label(Guid applicationId, StateConnection info)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetStateConnectionLabel"),
                applicationId, info.WorkFlowID, info.InState.StateID, info.OutState.StateID, 
                info.Label, info.LastModifierUserID, DateTime.Now);
        }

        public static bool set_state_connection_attachment_status(Guid applicationId, StateConnection info)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetStateConnectionAttachmentStatus"),
                applicationId, info.WorkFlowID, info.InState.StateID, info.OutState.StateID,
                info.AttachmentRequired, info.AttachmentTitle, info.LastModifierUserID, DateTime.Now);
        }

        public static bool set_state_connection_director(Guid applicationId, StateConnection info)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetStateConnectionDirector"),
                applicationId, info.WorkFlowID, info.InState.StateID, info.OutState.StateID, info.NodeRequired, 
                info.DirectorNodeType.NodeTypeID, info.NodeTypeDescription, info.LastModifierUserID, DateTime.Now);
        }

        public static bool set_state_connection_form(Guid applicationId, StateConnectionForm info)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetStateConnectionForm"),
                applicationId, info.WorkFlowID, info.InStateID, info.OutStateID, info.Form.FormID, 
                info.Description, info.Necessary, info.CreatorUserID, DateTime.Now);
        }

        public static bool remove_state_connection_form(Guid applicationId, StateConnectionForm info)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ArithmeticDeleteStateConnectionForm"),
                applicationId, info.WorkFlowID, info.InStateID, info.OutStateID, info.Form.FormID, info.LastModifierUserID, DateTime.Now);
        }

        public static bool add_auto_message(Guid applicationId, AutoMessage info)
        {
            string strAudienceType = null;
            if (info.AudienceType.HasValue) strAudienceType = info.AudienceType.Value.ToString();

            return DBConnector.succeed(applicationId, GetFullyQualifiedName("AddAutoMessage"),
                applicationId, info.AutoMessageID, info.OwnerID, info.BodyText, strAudienceType, info.RefState.StateID, 
                info.Node.NodeID, info.Admin, info.CreatorUserID, DateTime.Now);
        }

        public static bool modify_auto_message(Guid applicationId, AutoMessage info)
        {
            string strAudienceType = null;
            if (info.AudienceType.HasValue) strAudienceType = info.AudienceType.Value.ToString();

            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ModifyAutoMessage"),
                applicationId, info.AutoMessageID, info.BodyText, strAudienceType, info.RefState.StateID, 
                info.Node.NodeID, info.Admin, info.LastModifierUserID, DateTime.Now);
        }

        public static bool remove_auto_message(Guid applicationId, Guid automessageId, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ArithmeticDeleteAutoMessage"),
                applicationId, automessageId, currentUserId, DateTime.Now);
        }

        public static List<AutoMessage> get_owner_auto_messages(Guid applicationId, List<Guid> ownerIds)
        {
            return WFParsers.auto_messages(DBConnector.read(applicationId, GetFullyQualifiedName("GetOwnerAutoMessages"),
                applicationId, ProviderUtil.list_to_string<Guid>(ownerIds), ','));
        }

        public static List<AutoMessage> get_owner_auto_messages(Guid applicationId, Guid ownerId)
        {
            return get_owner_auto_messages(applicationId, new List<Guid>() { ownerId });
        }

        public static List<AutoMessage> get_workflow_auto_messages(Guid applicationId, Guid workflowId)
        {
            return WFParsers.auto_messages(DBConnector.read(applicationId, GetFullyQualifiedName("GetWorkFlowAutoMessages"),
                applicationId, workflowId));
        }

        public static List<AutoMessage> get_connection_auto_messages(Guid applicationId, 
            Guid workflowId, Guid inStateId, Guid outStateId)
        {
            return WFParsers.auto_messages(DBConnector.read(applicationId, GetFullyQualifiedName("GetConnectionAutoMessages"),
                applicationId, workflowId, inStateId, outStateId));
        }

        private static List<State> _get_workflow_states(Guid applicationId, Guid workflowId, List<Guid> stateIds, bool? all)
        {
            string strStateIds = null;
            if (!all.HasValue || !all.Value) strStateIds = ProviderUtil.list_to_string<Guid>(stateIds);

            return WFParsers.workflow_states(DBConnector.read(applicationId, GetFullyQualifiedName("GetWorkFlowStates"),
                applicationId, workflowId, strStateIds, ','));
        }

        public static List<State> get_workflow_states(Guid applicationId, Guid workflowId)
        {
            return _get_workflow_states(applicationId, workflowId, stateIds: new List<Guid>(), all: true);
        }

        public static List<State> get_workflow_states(Guid applicationId, Guid workflowId, List<Guid> stateIds)
        {
            return _get_workflow_states(applicationId, workflowId, stateIds, all: false);
        }

        public static State get_workflow_state(Guid applicationId, Guid workflowId, Guid stateId)
        {
            return get_workflow_states(applicationId, workflowId, new List<Guid>() { stateId }).FirstOrDefault();
        }

        public static State get_first_workflow_state(Guid applicationId, Guid workflowId)
        {
            return WFParsers.workflow_states(DBConnector.read(applicationId, GetFullyQualifiedName("GetFirstWorkFlowState"),
                applicationId, workflowId)).FirstOrDefault();
        }

        private static List<StateDataNeed> _get_state_data_needs(Guid applicationId, Guid workflowId, List<Guid> stateIds, bool? all)
        {
            string strStateIds = null;
            if (!all.HasValue || !all.Value) strStateIds = ProviderUtil.list_to_string<Guid>(stateIds);

            return WFParsers.state_data_needs(DBConnector.read(applicationId, GetFullyQualifiedName("GetStateDataNeeds"),
                applicationId, workflowId, strStateIds, ','));
        }

        public static List<StateDataNeed> get_state_data_needs(Guid applicationId, Guid workflowId)
        {
            return _get_state_data_needs(applicationId, workflowId, stateIds: new List<Guid>(), all: true);
        }

        public static List<StateDataNeed> get_state_data_needs(Guid applicationId, Guid workflowId, List<Guid> stateIds)
        {
            return _get_state_data_needs(applicationId, workflowId, stateIds, all: false);
        }

        public static List<StateDataNeed> get_state_data_needs(Guid applicationId, Guid workflowId, Guid stateId)
        {
            return get_state_data_needs(applicationId, workflowId, new List<Guid>() { stateId });
        }

        public static StateDataNeed get_state_data_need(Guid applicationId, Guid workflowId, Guid stateId, Guid nodeTypeId)
        {
            return WFParsers.state_data_needs(DBConnector.read(applicationId, GetFullyQualifiedName("GetStateDataNeed"),
                applicationId, workflowId, stateId, nodeTypeId)).FirstOrDefault();
        }

        public static List<StateDataNeed> get_current_state_data_needs(Guid applicationId, Guid workflowId, Guid stateId)
        {
            return WFParsers.state_data_needs(DBConnector.read(applicationId, GetFullyQualifiedName("GetCurrentStateDataNeeds"),
                applicationId, workflowId, stateId));
        }

        public static bool create_state_data_need_instance(Guid applicationId, StateDataNeedInstance instance, 
            ref List<Dashboard> dashboards, ref string errorMessage)
        {
            return DBConnector.get_dashboards(applicationId, ref errorMessage, ref dashboards,
                GetFullyQualifiedName("CreateStateDataNeedInstance"),
                applicationId, instance.InstanceID, instance.HistoryID, instance.DirectorNode.NodeID,
                instance.Admin, instance.FormID, instance.CreatorUserID, DateTime.Now) > 0;
        }

        public static bool set_state_data_need_instance_as_filled(Guid applicationId, Guid instanceId, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetStateDataNeedInstanceAsFilled"),
                applicationId, instanceId, currentUserId, DateTime.Now);
        }

        public static bool set_state_data_need_instance_as_not_filled(Guid applicationId, Guid instanceId, 
            Guid currentUserId, ref List<Dashboard> dashboards, ref string errorText)
        {
            return DBConnector.get_dashboards(applicationId, ref errorText, ref dashboards,
                GetFullyQualifiedName("SetStateDataNeedInstanceAsNotFilled"), applicationId, instanceId, currentUserId, DateTime.Now) > 0;
        }

        public static bool remove_state_data_need_instance(Guid applicationId, Guid instanceId, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ArithmeticDeleteStateDataNeedInstance"),
                applicationId, instanceId, currentUserId, DateTime.Now);
        }

        public static List<StateDataNeedInstance> get_state_data_need_instances(Guid applicationId, List<Guid> historyIds)
        {
            return WFParsers.state_data_need_instances(DBConnector.read(applicationId,
                GetFullyQualifiedName("GetStateDataNeedInstances"),
                applicationId, ProviderUtil.list_to_string<Guid>(historyIds), ','));
        }

        public static List<StateDataNeedInstance> get_state_data_need_instances(Guid applicationId, Guid historyId)
        {
            return get_state_data_need_instances(applicationId, new List<Guid>() { historyId });
        }

        public static StateDataNeedInstance get_state_data_need_instance(Guid applicationId, Guid instanceId)
        {
            return WFParsers.state_data_need_instances(DBConnector.read(applicationId,
                GetFullyQualifiedName("GetStateDataNeedInstance"), applicationId, instanceId)).FirstOrDefault();
        }

        private static List<StateConnection> _get_workflow_connections(Guid applicationId, 
            Guid workflowId, List<Guid> inStateIds, bool? all)
        {
            string strStateIds = null;
            if (!all.HasValue || !all.Value) strStateIds = ProviderUtil.list_to_string<Guid>(inStateIds);

            return WFParsers.state_connections(DBConnector.read(applicationId, GetFullyQualifiedName("GetWorkFlowConnections"),
                applicationId, workflowId, strStateIds, ','));
        }

        public static List<StateConnection> get_workflow_connections(Guid applicationId, Guid workflowId)
        {
            return _get_workflow_connections(applicationId, workflowId, inStateIds: new List<Guid>(), all: true);
        }

        public static List<StateConnection> get_workflow_connections(Guid applicationId, Guid workflowId, List<Guid> inStateIds)
        {
            return _get_workflow_connections(applicationId, workflowId, inStateIds, all: false);
        }

        public static List<StateConnection> get_workflow_connections(Guid applicationId, Guid workflowId, Guid inStateId)
        {
            return get_workflow_connections(applicationId, workflowId, new List<Guid>() { inStateId });
        }

        private static List<StateConnectionForm> _get_workflow_connection_forms(Guid applicationId, 
            Guid workflowId, List<Guid> inStateIds, bool? all)
        {
            string strStateIds = null;
            if (!all.HasValue || !all.Value) strStateIds = ProviderUtil.list_to_string<Guid>(inStateIds);

            return WFParsers.connection_forms(DBConnector.read(applicationId,
                GetFullyQualifiedName("GetWorkFlowConnectionForms"), applicationId, workflowId, strStateIds, ','));
        }

        public static List<StateConnectionForm> get_workflow_connection_forms(Guid applicationId, Guid workflowId)
        {
            return _get_workflow_connection_forms(applicationId, workflowId, inStateIds: new List<Guid>(), all: true);
        }

        public static List<StateConnectionForm> get_workflow_connection_forms(Guid applicationId, Guid workflowId, List<Guid> inStateIds)
        {
            return _get_workflow_connection_forms(applicationId, workflowId, inStateIds, all: false);
        }

        public static List<StateConnectionForm> get_workflow_connection_forms(Guid applicationId, Guid workflowId, Guid inStateId)
        {
            return get_workflow_connection_forms(applicationId, workflowId, new List<Guid>() { inStateId });
        }

        public static List<History> get_owner_history(Guid applicationId, Guid ownerId)
        {
            return WFParsers.history(DBConnector.read(applicationId, GetFullyQualifiedName("GetHistory"), applicationId, ownerId));
        }

        public static History get_last_history(Guid applicationId, Guid ownerId, Guid? stateId = null, bool done = false)
        {
            return WFParsers.history(DBConnector.read(applicationId, GetFullyQualifiedName("GetLastHistory"),
                applicationId, ownerId, stateId, done)).FirstOrDefault();
        }

        public static Guid? get_last_selected_state_id(Guid applicationId, Guid ownerId, Guid? inStateId = null)
        {
            return DBConnector.get_guid(applicationId, GetFullyQualifiedName("GetLastSelectedStateID"), 
                applicationId, ownerId, inStateId);
        }

        public static List<History> get_history(Guid applicationId, List<Guid> historyIds)
        {
            return WFParsers.history(DBConnector.read(applicationId, GetFullyQualifiedName("GetHistoryByIDs"),
                applicationId, ProviderUtil.list_to_string<Guid>(historyIds), ','));
        }

        public static History get_history(Guid applicationId, Guid historyId)
        {
            return get_history(applicationId, new List<Guid>() { historyId }).FirstOrDefault();
        }

        public static Guid get_history_owner_id(Guid applicationId, Guid historyId)
        {
            Guid? id = DBConnector.get_guid(applicationId, GetFullyQualifiedName("GetHistoryOwnerID"), applicationId, historyId);

            if (id.HasValue) return id.Value;
            else return Guid.Empty;
        }

        public static Guid create_history_form_instance(Guid applicationId, 
            Guid historyId, Guid outStateId, Guid formId, Guid currentUserId)
        {
            Guid? id = DBConnector.get_guid(applicationId, GetFullyQualifiedName("CreateHistoryFormInstance"),
                applicationId, historyId, outStateId, formId, currentUserId, DateTime.Now);

            if (id.HasValue) return id.Value;
            else return Guid.Empty;
        }

        public static List<HistoryFormInstance> get_history_form_instances(Guid applicationId, List<Guid> historyIds, bool? selected)
        {
            return WFParsers.history_form_instances(DBConnector.read(applicationId, GetFullyQualifiedName("GetHistoryFormInstances"),
                applicationId, ProviderUtil.list_to_string<Guid>(historyIds), ',', selected));
        }

        public static List<HistoryFormInstance> get_history_form_instances(Guid applicationId, Guid historyId, bool? selected)
        {
            return get_history_form_instances(applicationId, new List<Guid>() { historyId }, selected);
        }

        public static bool send_to_next_state(Guid applicationId,
            History info, bool reject, ref string errorMessage, ref List<Dashboard> dashboards)
        {
            if (info == null) info = new History();

            return DBConnector.get_dashboards(applicationId, ref errorMessage, ref dashboards,
                GetFullyQualifiedName("SendToNextState"), applicationId, info.HistoryID, info.State.StateID, 
                info.DirectorNode.NodeID, info.DirectorUserID, info.Description, reject, info.Sender.UserID, 
                DateTime.Now, DocFileInfoTableType.getCompositeType(info.AttachedFiles)) > 0;
        }
        
        public static bool terminate_workFlow(Guid applicationId, 
            Guid historyId, string description, Guid currentUserId, ref string errorMessage)
        {
            return DBConnector.succeed(applicationId, ref errorMessage, GetFullyQualifiedName("TerminateWorkFlow"),
                applicationId, historyId, description, currentUserId, DateTime.Now);
        }

        public static ViewerStatus get_viewer_status(Guid applicationId, Guid userId, Guid ownerId)
        {
            string status = DBConnector.get_string(applicationId, GetFullyQualifiedName("GetViewerStatus"), 
                applicationId, userId, ownerId);

            return PublicMethods.parse_enum<ViewerStatus>(status, defaultValue: ViewerStatus.None);
        }

        public static bool restart_workFlow(Guid applicationId, Guid ownerId, Guid? directorNodeId,
            Guid? directorUserId, Guid currentUserId, ref List<Dashboard> dashboards, ref string errorMessage)
        {
            return DBConnector.get_dashboards(applicationId, ref errorMessage, ref dashboards,
                GetFullyQualifiedName("RestartWorkFlow"),
                applicationId, ownerId, directorNodeId, directorUserId, currentUserId, DateTime.Now) > 0;
        }

        public static bool has_workflow(Guid applicationId, Guid ownerId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("HasWorkFlow"), applicationId, ownerId);
        }

        public static bool is_terminated(Guid applicationId, Guid ownerId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("IsTerminated"), applicationId, ownerId);
        }

        public static List<KeyValuePair<string, int>> get_service_abstract(Guid applicationId, 
            Guid nodeTypeId, Guid? workflowId, Guid? userId, string nullTagLabel)
        {
            return WFParsers.service_abstract(DBConnector.read(applicationId, GetFullyQualifiedName("GetServiceAbstract"),
                applicationId, workflowId, nodeTypeId, userId, nullTagLabel));
        }

        public static List<Guid> get_service_user_ids(Guid applicationId, Guid? nodeTypeId, Guid? workflowId)
        {
            return DBConnector.get_guid_list(applicationId, GetFullyQualifiedName("GetServiceUserIDs"),
                applicationId, workflowId, nodeTypeId);
        }

        public static int get_workflow_items_count(Guid applicationId, Guid workflowId)
        {
            return DBConnector.get_int(applicationId, GetFullyQualifiedName("GetWorkFlowItemsCount"), applicationId, workflowId);
        }

        public static int get_workflow_state_items_count(Guid applicationId, Guid workflowId, Guid stateId)
        {
            return DBConnector.get_int(applicationId, GetFullyQualifiedName("GetWorkFlowStateItemsCount"),
                applicationId, workflowId, stateId);
        }

        public static List<NodesCount> get_user_workflow_items_count(Guid applicationId, Guid userId, 
            DateTime? lowerCreationDateLimit = null, DateTime? upperCreationDateLimit = null)
        {
            return WFParsers.items_count(DBConnector.read(applicationId, GetFullyQualifiedName("GetUserWorkFlowItemsCount"),
                applicationId, userId, lowerCreationDateLimit, upperCreationDateLimit));
        }

        public static bool add_owner_workflow(Guid applicationId, Guid nodeTypeId, Guid workflowId, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("AddOwnerWorkFlow"),
                applicationId, nodeTypeId, workflowId, currentUserId, DateTime.Now);
        }

        public static bool remove_owner_workflow(Guid applicationId, Guid nodeTypeId, Guid workflowId, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ArithmeticDeleteOwnerWorkFlow"),
                applicationId, nodeTypeId, workflowId, currentUserId, DateTime.Now);
        }

        public static List<WorkFlow> get_owner_workflows(Guid applicationId, Guid nodeTypeId)
        {
            return WFParsers.workflows(DBConnector.read(applicationId, GetFullyQualifiedName("GetOwnerWorkFlows"),
                applicationId, nodeTypeId));
        }

        public static Guid? get_owner_workflow_primary_key(Guid applicationId, Guid nodeTypeId, Guid workflowId)
        {
            Guid? id = DBConnector.get_guid(applicationId, GetFullyQualifiedName("GetOwnerWorkFlowPrimaryKey"),
                applicationId, nodeTypeId, workflowId);

            if (id.HasValue) return id.Value;
            else return Guid.Empty;
        }

        public static List<NodeType> get_workflow_owners(Guid applicationId)
        {
            List<Guid> ids = DBConnector.get_guid_list(applicationId, GetFullyQualifiedName("GetWorkFlowOwnerIDs"), applicationId);
            return CNController.get_node_types(applicationId, ids);
        }

        public static Guid? get_form_instance_workflow_owner_id(Guid applicationId, Guid formInstanceId)
        {
            return DBConnector.get_guid(applicationId, GetFullyQualifiedName("GetFormInstanceWorkFlowOwnerID"),
                applicationId, formInstanceId);
        }
    }
}
