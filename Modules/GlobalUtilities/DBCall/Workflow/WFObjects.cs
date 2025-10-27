using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RaaiVan.Modules.Users;
using RaaiVan.Modules.CoreNetwork;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.FormGenerator;

namespace RaaiVan.Modules.WorkFlow
{
    public enum StateResponseTypes
    {
        None,
        SendToOwner,
        RefState,
        SpecificNode,
        ContentAdmin,
        SpecificUser
    }

    public enum StateDataNeedsTypes
    {
        None,
        RefState,
        SpecificNodes
    }

    public enum AudienceTypes
    {
        SendToOwner,
        RefState,
        SpecificNode
    }

    public enum ViewerStatus
    {
        None,
        NotInWorkFlow,
        Director,
        DirectorNodeMember,
        Owner
    }

    public enum WorkFlowAction
    {
        None,
        Publish,
        Unpublish
    }

    public class StateDataNeedInstance
    {
        public Guid? InstanceID;
        public Guid? HistoryID;
        public Guid? OwnerID;
        public Node DirectorNode;
        public bool? Admin;
        public bool? Filled;
        public DateTime? FillingDate;
        public Guid? FormID;
        public Guid? AttachmentID;
        public Guid? CreatorUserID;
        public DateTime? CreationDate;
        public Guid? LastModifierUserID;
        public DateTime? LastModificationDate;
        public List<DocFileInfo> PreAttachedFiles;
        public List<DocFileInfo> Attachments;

        public StateDataNeedInstance()
        {
            DirectorNode = new Node();
            PreAttachedFiles = new List<DocFileInfo>();
            Attachments = new List<DocFileInfo>();
        }

        public string toJson()
        {
            return "{\"InstanceID\":\"" + (!InstanceID.HasValue ? string.Empty : InstanceID.Value.ToString()) +
                "\",\"NodeID\":\"" + (!DirectorNode.NodeID.HasValue ? string.Empty : DirectorNode.NodeID.Value.ToString()) +
                "\",\"NodeName\":\"" + Base64.encode(DirectorNode.Name) +
                "\",\"Filled\":" + (Filled.HasValue && Filled.Value).ToString().ToLower() +
                ",\"FillingDate\":\"" + (FillingDate.HasValue ?
                    GenericDate.get_local_date(FillingDate.Value) : string.Empty) + "\"" + 
                ",\"PreAttachedFiles\":" + DocumentUtilities.get_files_json(PreAttachedFiles) + 
                "}";
        }
    }

    public class StateDataNeed
    {
        public Guid? ID;
        public Guid? WorkFlowID;
        public Guid? StateID;
        public NodeType DirectorNodeType;
        public Guid? FormID;
        public string FormTitle;
        public string Description;
        public bool? MultiSelect;
        public bool? Admin;
        public bool? Necessary;
        public Guid? CreatorUserID;
        public DateTime? CreationDate;
        public Guid? LastModifierUserID;
        public DateTime? LastModificationDate;
        public List<StateDataNeedInstance> Instances;

        public StateDataNeed()
        {
            DirectorNodeType = new NodeType();
            Instances = new List<StateDataNeedInstance>();
        }

        public string toJson(Dictionary<Guid, string> names = null)
        {
            if (names == null) names = new Dictionary<Guid, string>();

            return "{\"ID\":\"" + (!ID.HasValue ? string.Empty : ID.ToString()) + "\"" +
                ",\"NodeTypeID\":\"" + (!DirectorNodeType.NodeTypeID.HasValue ? string.Empty :
                    DirectorNodeType.NodeTypeID.Value.ToString()) + "\"" +
                ",\"NodeType\":\"" + (!DirectorNodeType.NodeTypeID.HasValue || !names.ContainsKey(DirectorNodeType.NodeTypeID.Value) ?
                     Base64.encode(DirectorNodeType.Name) : Base64.encode(names[DirectorNodeType.NodeTypeID.Value])) + "\"" +
                (!FormID.HasValue ? string.Empty :
                    ",\"FormID\":\"" + FormID.Value.ToString() + "\"" +
                    ",\"FormTitle\":\"" + Base64.encode(FormTitle) + "\""
                ) +
                ",\"MultiSelect\":" + (MultiSelect.HasValue && MultiSelect.Value).ToString().ToLower() +
                ",\"Admin\":" + (Admin.HasValue && Admin.Value).ToString().ToLower() +
                ",\"Necessary\":" + (Necessary.HasValue && Necessary.Value).ToString().ToLower() +
                ",\"Description\":\"" + Base64.encode(Description) + "\"" +
                ",\"Instances\":[" + string.Join(",", Instances.Select(inst => inst.toJson())) + "]" +
                "}";
        }
    }

    public class State
    {
        public Guid? ID;
        public Guid? StateID;
        public Guid? WorkFlowID;
        public string Title;
        public string Description;
        public string Tag;
        public StateResponseTypes? ResponseType;
        public Guid? RefStateID;
        public Node DirectorNode;
        public bool? DirectorIsAdmin;
        public User DirectorUser;
        public StateDataNeedsTypes? DataNeedsType;
        public Guid? RefDataNeedsStateID;
        public string DataNeedsDescription;
        public bool? DescriptionNeeded;
        public bool? HideOwnerName;
        public bool? EditPermission;
        public bool? FreeDataNeedRequests;
        public List<StateDataNeed> DataNeeds;
        public int? MaxAllowedRejections;
        public string RejectionTitle;
        public Guid? RejectionRefStateID;
        public string RejectionRefStateTitle;
        public Guid? PollID;
        public string PollName;
        public List<StateConnection> Connections;
        public Guid? CreatorUserID;
        public DateTime? CreationDate;
        public Guid? LastModifierUserID;
        public DateTime? LastModificationDate;

        public State()
        {
            DirectorNode = new Node();
            DirectorUser = new User();
            DataNeeds = new List<StateDataNeed>();
        }

        public string toJson(Dictionary<Guid, string> names)
        {
            if (names == null) names = new Dictionary<Guid, string>();

            return "{\"ID\":\"" + (!ID.HasValue ? string.Empty : ID.ToString()) + "\"" +
                ",\"StateID\":\"" + (!StateID.HasValue ? string.Empty : StateID.Value.ToString()) + "\"" +
                ",\"Title\":\"" + (Base64.encode(string.IsNullOrEmpty(Title) && names.ContainsKey(StateID.Value) ?
                    names[StateID.Value] : Title)) + "\"" +
                ",\"Description\":\"" + Base64.encode(Description) + "\"" +
                ",\"Tag\":\"" + Base64.encode(Tag) + "\"" +
                ",\"DataNeedsType\":\"" + (DataNeedsType.HasValue ? DataNeedsType.ToString() : string.Empty) + "\"" +
                ",\"RefDataNeedsStateID\":\"" + (RefDataNeedsStateID.HasValue ?
                    RefDataNeedsStateID.ToString() : string.Empty) + "\"" +
                ",\"RefDataNeedsStateTitle\":\"" + (RefDataNeedsStateID.HasValue && names.ContainsKey(RefDataNeedsStateID.Value) ?
                    Base64.encode(names[RefDataNeedsStateID.Value]) : string.Empty) + "\"" +
                ",\"DataNeedsDescription\":\"" + Base64.encode(DataNeedsDescription) + "\"" +
                ",\"DescriptionNeeded\":" + (DescriptionNeeded.HasValue ?
                    DescriptionNeeded.Value.ToString().ToLower() : "true") +
                ",\"HideOwnerName\":" + (HideOwnerName.HasValue && HideOwnerName.Value).ToString().ToLower() +
                ",\"EditPermission\":" + (EditPermission.HasValue && EditPermission.Value).ToString().ToLower() +
                ",\"FreeDataNeedRequests\":" + (FreeDataNeedRequests.HasValue && FreeDataNeedRequests.Value).ToString().ToLower() +
                ",\"Director\":" + "{\"ResponseType\":\"" + (ResponseType.HasValue ? ResponseType.ToString() : string.Empty) + "\"" +
                    ",\"RefStateID\":\"" + (RefStateID.HasValue ? RefStateID.Value.ToString() : string.Empty) + "\"" +
                    ",\"RefStateTitle\":\"" + (RefStateID.HasValue && names.ContainsKey(RefStateID.Value) ?
                        Base64.encode(names[RefStateID.Value]) : string.Empty) + "\"" +
                    ",\"NodeName\":\"" + Base64.encode(DirectorNode.Name) + "\"" +
                    ",\"NodeID\":\"" + (DirectorNode.NodeID.HasValue ? DirectorNode.NodeID.Value.ToString() : string.Empty) + "\"" +
                    ",\"NodeTypeID\":\"" + (DirectorNode.NodeTypeID.HasValue ?
                        DirectorNode.NodeTypeID.Value.ToString() : string.Empty) + "\"" +
                    ",\"NodeType\":\"" + Base64.encode(DirectorNode.NodeType) + "\"" +
                    ",\"Admin\":" + (DirectorIsAdmin.HasValue && DirectorIsAdmin.Value).ToString().ToLower() +
                    ",\"FullName\":\"" + Base64.encode(DirectorUser.FullName) + "\"" +
                    ",\"UserID\":\"" + (DirectorUser.UserID.HasValue ? DirectorUser.UserID.Value.ToString() : string.Empty) + "\"" +
                    "}" +
                ",\"MaxAllowedRejections\":" + (MaxAllowedRejections.HasValue ? MaxAllowedRejections.ToString() : "null") +
                ",\"RejectionTitle\":\"" + Base64.encode(RejectionTitle) + "\"" +
                ",\"RejectionRefStateID\":\"" + (RejectionRefStateID.HasValue ?
                    RejectionRefStateID.ToString() : string.Empty) + "\"" +
                ",\"RejectionRefStateTitle\":\"" + Base64.encode(RejectionRefStateTitle) + "\"" +
                (!PollID.HasValue ? string.Empty : 
                    ",\"Poll\":{\"PollID\":\"" + PollID.ToString() + "\"" +
                    ",\"Name\":\"" + Base64.encode(PollName) + "\"" + "}") +
                ",\"DataNeeds\":[" + string.Join(",", DataNeeds.Select(dn => dn.toJson(names))) + "]" +
                ",\"Connections\":[" + string.Join(",", Connections.Select(conn => conn.toJson(names))) + "]" +
                "}";
        }
    }

    public class StateConnectionForm
    {
        public Guid? WorkFlowID;
        public Guid? InStateID;
        public Guid? OutStateID;
        public FormType Form;
        public string Description;
        public bool? Necessary;
        public Guid? CreatorUserID;
        public DateTime? CreationDate;
        public Guid? LastModifierUserID;
        public DateTime? LastModificationDate;

        public StateConnectionForm()
        {
            Form = new FormType();
        }

        public string toJson()
        {
            return "{\"FormID\":\"" + (Form.FormID.HasValue ? Form.FormID.Value.ToString() : string.Empty) +
                "\",\"Title\":\"" + Base64.encode(Form.Title) + "\"" + 
                ",\"Description\":\"" + Base64.encode(Form.Description) +
                "\",\"Necessary\":" + (Necessary.HasValue && Necessary.Value).ToString().ToLower() + 
                "}";
        }
    }

    public class AutoMessage
    {
        public Guid? AutoMessageID;
        public Guid? OwnerID;
        public Guid? WorkFlowID;
        public Guid? InStateID;
        public Guid? OutStateID;
        public string BodyText;
        public AudienceTypes? AudienceType;
        public State RefState;
        public Node Node;
        public bool? Admin;
        public Guid? CreatorUserID;
        public DateTime? CreationDate;
        public Guid? LastModifierUserID;
        public DateTime? LastModificationDate;

        public AutoMessage()
        {
            RefState = new State();
            Node = new Node();
        }

        public string toJson()
        {
            return "{\"AutoMessageID\":\"" + (!AutoMessageID.HasValue ? string.Empty : AutoMessageID.Value.ToString()) + "\"" +
                ",\"OwnerID\":\"" + (!OwnerID.HasValue ? string.Empty : OwnerID.Value.ToString()) + "\"" +
                ",\"BodyText\":\"" + Base64.encode(BodyText) + "\"" +
                ",\"AudienceType\":\"" + AudienceType + "\"" +
                ",\"RefStateID\":\"" + (RefState.StateID.HasValue ? RefState.StateID.Value.ToString() : string.Empty) + "\"" +
                ",\"RefStateTitle\":\"" + Base64.encode(RefState.Title) + "\"" +
                ",\"NodeID\":\"" + (Node.NodeID.HasValue ? Node.NodeID.Value.ToString() : string.Empty) + "\"" +
                ",\"NodeName\":\"" + Base64.encode(Node.Name) + "\"" +
                ",\"NodeTypeID\":\"" + (Node.NodeTypeID.HasValue ? Node.NodeTypeID.Value.ToString() : string.Empty) + "\"" +
                ",\"NodeType\":\"" + Base64.encode(Node.NodeType) + "\"" +
                ",\"Admin\":" + (Admin.HasValue && Admin.Value).ToString().ToLower() +
                "}";
        }
    }

    public class StateConnection
    {
        public Guid? ID;
        public Guid? WorkFlowID;
        public State InState;
        public State OutState;
        public int? SequenceNumber;
        public string Label;
        public bool? AttachmentRequired;
        public string AttachmentTitle;
        public bool? NodeRequired;
        public NodeType DirectorNodeType;
        public string NodeTypeDescription;

        public string StateTitle;
        public Guid? NodeID;
        public string NodeTitle;

        public Guid? CreatorUserID;
        public DateTime? CreationDate;
        public Guid? LastModifierUserID;
        public DateTime? LastModificationDate;
        public List<DocFileInfo> AttachedFiles;
        public List<StateConnectionForm> Forms;
        public List<AutoMessage> AutoMessages;
        public List<HistoryFormInstance> HistoryFormInstances;
        public List<WorkFlowAction> Actions;

        public StateConnection()
        {
            InState = new State();
            OutState = new State();
            DirectorNodeType = new NodeType();
            AttachedFiles = new List<DocFileInfo>();
            Forms = new List<StateConnectionForm>();
            AutoMessages = new List<AutoMessage>();
            HistoryFormInstances = new List<HistoryFormInstance>();
            Actions = new List<WorkFlowAction>();
        }

        public string toJson(Dictionary<Guid, string> names)
        {
            if (names == null) names = new Dictionary<Guid, string>();

            return "{\"ID\":\"" + (!ID.HasValue ? string.Empty : ID.ToString()) + "\"" +
                ",\"OutStateID\":\"" + (!OutState.StateID.HasValue ? string.Empty : OutState.StateID.Value.ToString()) + "\"" +
                ",\"OutStateTitle\":\"" + (!OutState.StateID.HasValue || !names.ContainsKey(OutState.StateID.Value) ? string.Empty : 
                    Base64.encode(names[OutState.StateID.Value])) + "\"" +
                ",\"ConnectionLabel\":\"" + Base64.encode(Label) + "\"" +
                ",\"AttachmentRequired\":" + (AttachmentRequired.HasValue && AttachmentRequired.Value).ToString().ToLower() +
                ",\"AttachmentTitle\":\"" + Base64.encode(AttachmentTitle) + "\"" +
                ",\"NodeRequired\":" + (NodeRequired.HasValue && NodeRequired.Value).ToString().ToLower() +
                ",\"NodeTypeID\":\"" + (!DirectorNodeType.NodeTypeID.HasValue ? string.Empty :
                    DirectorNodeType.NodeTypeID.Value.ToString()) + "\"" +
                ",\"NodeTypeName\":\"" + (!DirectorNodeType.NodeTypeID.HasValue || !names.ContainsKey(DirectorNodeType.NodeTypeID.Value) ?
                    string.Empty : Base64.encode(names[DirectorNodeType.NodeTypeID.Value])) + "\"" +
                ",\"NodeTypeDescription\":\"" + Base64.encode(NodeTypeDescription) + "\"" +
                (Actions == null || Actions.Count == 0 ? string.Empty :
                    ",\"Actions\":[" + string.Join(",",
                        Actions.Where(ac => ac != WorkFlowAction.None).Select(ac => "\"" + ac.ToString() + "\"")) + "]") +
                ",\"AttachedFiles\":[" + string.Join(",", AttachedFiles.Select(u => u.toJson())) + "]" +
                ",\"Forms\":[" + string.Join(",", Forms.Select(u => u.toJson())) + "]" +
                ",\"Audience\":[" + string.Join(",", AutoMessages.Select(u => u.toJson())) + "]" +
                "}";
        }

        public string toJson_history_mode(Guid applicationId)
        {
            return "{\"DirectorNodeTypeID\":\"" + (DirectorNodeType.NodeTypeID.HasValue ?
                    DirectorNodeType.NodeTypeID.Value.ToString() : string.Empty) + "\"" +
                ",\"DirectorNodeType\":\"" + Base64.encode(DirectorNodeType.Name) + "\"" +
                ",\"NodeTypeDescription\":\"" + Base64.encode(NodeTypeDescription) + "\"" +
                ",\"OutStateID\":\"" + (OutState.StateID.HasValue ? OutState.StateID.Value.ToString() : string.Empty) + "\"" +
                ",\"Label\":\"" + Base64.encode(Label) + "\"" +
                ",\"NodeRequired\":" + (NodeRequired.HasValue && NodeRequired.Value).ToString().ToLower() +
                ",\"AttachmentRequired\":" + (AttachmentRequired.HasValue && AttachmentRequired.Value).ToString().ToLower() +
                ",\"AttachmentTitle\":\"" + Base64.encode(AttachmentTitle) + "\"" +
                ",\"TemplateFiles\":[" + string.Join(",", AttachedFiles.Select(u => u.toJson(icon: true))) + "]" +
                ",\"Forms\":[" + string.Join(",",
                    HistoryFormInstances.Select(x => string.Join(",", x.Forms.Select(f => f.toJson(applicationId, creator: true))))) + "]" +
                "}";
        }
    }

    public class WorkFlow
    {
        public Guid? WorkFlowID;
        public string Name;
        public string Description;
        public List<State> States;
        public Guid? CreatorUserID;
        public DateTime? CreationDate;
        public Guid? LastModifierUserID;
        public DateTime? LastModificationDate;

        public WorkFlow()
        {
            States = new List<State>();
        }

        public string toJson(Dictionary<Guid, string> names = null) {
            if (names == null) names = new Dictionary<Guid, string>();

            return "{\"WorkFlowID\":\"" + (!WorkFlowID.HasValue ? string.Empty : WorkFlowID.ToString()) + "\"" +
                ",\"Name\":\"" + Base64.encode(Name) + "\"" +
                ",\"Description\":\"" + Base64.encode(Description) + "\"" +
                (States == null || States.Count == 0 ? string.Empty :
                    ",\"States\":[" + string.Join(",", States.Select(st => st.toJson(names))) + "]") +
                "}";
        }
    }

    public class HistoryFormInstance
    {
        public Guid? HistoryID;
        public Guid? OutStateID;
        public Guid? FormsID;
        public List<FormType> Forms;

        public HistoryFormInstance()
        {
            Forms = new List<FormType>();
        }
    }

    public class History
    {
        public Guid? HistoryID;
        public Guid? PreviousHistoryID;
        public Guid? OwnerID;
        public Node DirectorNode;
        public Guid? DirectorUserID;
        public Guid? WorkFlowID;
        public State State;
        public Guid? SelectedOutStateID;
        public string Description;
        public User Sender;
        public DateTime? SendDate;
        public Guid? PollID;
        public string PollName;
        public List<HistoryFormInstance> FormInstances;
        public List<DocFileInfo> AttachedFiles;

        public History()
        {
            DirectorNode = new Node();
            State = new State();
            FormInstances = new List<HistoryFormInstance>();
            AttachedFiles = new List<DocFileInfo>();
            Sender = new User();
        }

        public string toJson(Guid applicationId)
        {
            return "{\"HistoryID\":\"" + (!HistoryID.HasValue ? string.Empty : HistoryID.Value.ToString()) + "\"" +
                ",\"PreviousHistoryID\":\"" + (PreviousHistoryID.HasValue ? PreviousHistoryID.ToString() : string.Empty) + "\"" +
                ",\"OwnerID\":\"" + (!OwnerID.HasValue ? string.Empty : OwnerID.Value.ToString()) + "\"" +
                ",\"WorkFlowID\":\"" + (!WorkFlowID.HasValue ? string.Empty : WorkFlowID.Value.ToString()) + "\"" +
                ",\"StateID\":\"" + (!State.StateID.HasValue ? string.Empty : State.StateID.Value.ToString()) + "\"" +
                ",\"StateTitle\":\"" + Base64.encode(State.Title) + "\"" +
                ",\"Description\":\"" + Base64.encode(Description) + "\"" +
                ",\"SendDate\":\"" + (SendDate.HasValue ?
                    GenericDate.get_local_date(SendDate.Value, true) : string.Empty) + "\"" +
                ",\"PollID\":\"" + (PollID.HasValue ? PollID.ToString() : string.Empty) + "\"" +
                ",\"PollName\":\"" + (PollID.HasValue ? Base64.encode(PollName) : string.Empty) + "\"" +
                (!PollID.HasValue ? string.Empty : 
                    ",\"PollID\":\"" + PollID.ToString() + "\"") +
                ",\"Director\":{\"UserID\":\"" + (Sender.UserID.HasValue ?
                        Sender.UserID.Value.ToString() : string.Empty) + "\"" +
                    ",\"FullName\":\"" + Base64.encode(Sender.FullName) + "\"" +
                    ",\"ProfileImageURL\":\"" + (Sender.UserID.HasValue ?
                        DocumentUtilities.get_personal_image_address(applicationId, Sender.UserID.Value) : string.Empty) + "\"" +
                    ",\"NodeID\":\"" + (DirectorNode.NodeID.HasValue ?
                        DirectorNode.NodeID.Value.ToString() : string.Empty) + "\"" +
                    ",\"NodeName\":\"" + Base64.encode(DirectorNode.Name) + "\"" +
                    ",\"NodeType\":\"" + Base64.encode(DirectorNode.NodeType) + "\"" + "}" +
                ",\"AttachedFiles\":[" + string.Join(",",
                    AttachedFiles.Select(u => u.toJson())) + "]" +
                ",\"Forms\":[" + string.Join(",",
                    FormInstances.Select(x => string.Join(",", x.Forms.Select(f => f.toJson(applicationId, creator: true))))) + "]" +
                "}";
        }
    }

    public class WFDashboard
    {
        public Guid? NodeID;
        public string NodeName;
        public string NodeType;
        public Guid? InstanceID;
        public DateTime? CreationDate;
        public string StateTitle;
    }

    public class WorkFlowNode
    {
        public State State;
        public Node Node;

        public WorkFlowNode()
        {
            State = new State();
            Node = new Node();
        }
    }
}
