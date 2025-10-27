using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaaiVan.Modules.CoreNetwork;
using RaaiVan.Modules.FormGenerator;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.Users;

namespace RaaiVan.Modules.WorkFlow
{
    public static class WFParsers
    {
        public static List<State> states(DBResultSet results)
        {
            List<State> retList = new List<State>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                retList.Add(new State()
                {
                    StateID = table.GetGuid(i, "StateID"),
                    Title = table.GetString(i, "Title")
                });
            }

            return retList;
        }

        public static List<WorkFlow> workflows(DBResultSet results)
        {
            List<WorkFlow> retList = new List<WorkFlow>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                retList.Add(new WorkFlow()
                {
                    WorkFlowID = table.GetGuid(i, "WorkFlowID"),
                    Name = table.GetString(i, "Name"),
                    Description = table.GetString(i, "Description")
                });
            }

            return retList;
        }

        public static List<State> workflow_states(DBResultSet results)
        {
            List<State> retList = new List<State>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                retList.Add(new State()
                {
                    ID = table.GetGuid(i, "ID"),
                    StateID = table.GetGuid(i, "StateID"),
                    WorkFlowID = table.GetGuid(i, "WorkFlowID"),
                    Description = table.GetString(i, "Description"),
                    Tag = table.GetString(i, "Tag"),
                    DataNeedsType = table.GetEnum<StateDataNeedsTypes>(i, "DataNeedsType"),
                    RefDataNeedsStateID = table.GetGuid(i, "RefDataNeedsStateID"),
                    DataNeedsDescription = table.GetString(i, "DataNeedsDescription"),
                    DescriptionNeeded = table.GetBool(i, "DescriptionNeeded"),
                    HideOwnerName = table.GetBool(i, "HideOwnerName"),
                    EditPermission = table.GetBool(i, "EditPermission"),
                    FreeDataNeedRequests = table.GetBool(i, "FreeDataNeedRequests"),
                    ResponseType = table.GetEnum<StateResponseTypes>(i, "ResponseType"),
                    RefStateID = table.GetGuid(i, "RefStateID"),
                    DirectorNode = new Node()
                    {
                        NodeID = table.GetGuid(i, "NodeID"),
                        Name = table.GetString(i, "NodeName"),
                        NodeTypeID = table.GetGuid(i, "NodeTypeID"),
                        NodeType = table.GetString(i, "NodeType")
                    },
                    DirectorIsAdmin = table.GetBool(i, "Admin"),
                    DirectorUser = new User()
                    {
                        UserID = table.GetGuid(i, "UserID"),
                        UserName = table.GetString(i, "UserName"),
                        FirstName = table.GetString(i, "FirstName"),
                        LastName = table.GetString(i, "LastName")
                    },
                    MaxAllowedRejections = table.GetInt(i, "MaxAllowedRejections"),
                    RejectionTitle = table.GetString(i, "RejectionTitle"),
                    RejectionRefStateID = table.GetGuid(i, "RejectionRefStateID"),
                    RejectionRefStateTitle = table.GetString(i, "RejectionRefStateTitle"),
                    PollID = table.GetGuid(i, "PollID"),
                    PollName = table.GetString(i, "PollName")
                });
            }

            return retList;
        }

        public static List<StateDataNeed> state_data_needs(DBResultSet results)
        {
            List<StateDataNeed> retList = new List<StateDataNeed>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                retList.Add(new StateDataNeed()
                {
                    ID = table.GetGuid(i, "ID"),
                    StateID = table.GetGuid(i, "StateID"),
                    WorkFlowID = table.GetGuid(i, "WorkFlowID"),
                    DirectorNodeType = new NodeType()
                    {
                        NodeTypeID = table.GetGuid(i, "NodeTypeID"),
                        Name = table.GetString(i, "NodeType")
                    },
                    FormID = table.GetGuid(i, "FormID"),
                    FormTitle = table.GetString(i, "FormTitle"),
                    Description = table.GetString(i, "Description"),
                    MultiSelect = table.GetBool(i, "MultiSelect"),
                    Admin = table.GetBool(i, "Admin"),
                    Necessary = table.GetBool(i, "Necessary")
                });
            }

            return retList;
        }

        public static List<StateDataNeedInstance> state_data_need_instances(DBResultSet results)
        {
            List<StateDataNeedInstance> retList = new List<StateDataNeedInstance>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                retList.Add(new StateDataNeedInstance()
                {
                    InstanceID = table.GetGuid(i, "InstanceID"),
                    HistoryID = table.GetGuid(i, "HistoryID"),
                    DirectorNode = new Node()
                    {
                        NodeID = table.GetGuid(i, "NodeID"),
                        Name = table.GetString(i, "NodeName"),
                        NodeTypeID = table.GetGuid(i, "NodeTypeID")
                    },
                    Filled = table.GetBool(i, "Filled"),
                    FillingDate = table.GetDate(i, "FillingDate"),
                    AttachmentID = table.GetGuid(i, "AttachmentID")
                });
            }

            return retList;
        }

        public static List<StateConnection> state_connections(DBResultSet results)
        {
            List<StateConnection> retList = new List<StateConnection>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                retList.Add(new StateConnection()
                {
                    ID = table.GetGuid(i, "ID"),
                    WorkFlowID = table.GetGuid(i, "WorkFlowID"),
                    InState = new State()
                    {
                        StateID = table.GetGuid(i, "InStateID")
                    },
                    OutState = new State()
                    {
                        StateID = table.GetGuid(i, "OutStateID")
                    },
                    SequenceNumber = table.GetInt(i, "SequenceNumber"),
                    Label = table.GetString(i, "ConnectionLabel"),
                    AttachmentRequired = table.GetBool(i, "AttachmentRequired"),
                    AttachmentTitle = table.GetString(i, "AttachmentTitle"),
                    NodeRequired = table.GetBool(i, "NodeRequired"),
                    DirectorNodeType = new NodeType()
                    {
                        NodeTypeID = table.GetGuid(i, "NodeTypeID"),
                        Name = table.GetString(i, "NodeType")
                    },
                    NodeTypeDescription = table.GetString(i, "NodeTypeDescription")
                });
            }

            return retList;
        }

        public static List<StateConnectionForm> connection_forms(DBResultSet results)
        {
            List<StateConnectionForm> retList = new List<StateConnectionForm>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                retList.Add(new StateConnectionForm()
                {
                    WorkFlowID = table.GetGuid(i, "WorkFlowID"),
                    InStateID = table.GetGuid(i, "InStateID"),
                    OutStateID = table.GetGuid(i, "OutStateID"),
                    Form = new FormType()
                    {
                        FormID = table.GetGuid(i, "FormID"),
                        Title = table.GetString(i, "FormTitle")
                    },
                    Description = table.GetString(i, "Description"),
                    Necessary = table.GetBool(i, "Necessary")
                });
            }

            return retList;
        }

        public static List<StateConnection> state_actions(DBResultSet results)
        {
            List<StateConnection> retList = new List<StateConnection>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                Guid? id = table.GetGuid(i, "ConnectionID");
                WorkFlowAction action = table.GetEnum<WorkFlowAction>(i, "Action", defaultValue: WorkFlowAction.None);

                if (!id.HasValue || action == WorkFlowAction.None) continue;

                StateConnection stateConnection = retList.Where(x => x.ID == id).FirstOrDefault();

                if (stateConnection == null) {
                    stateConnection = new StateConnection() { ID = id };
                    retList.Add(stateConnection);
                }

                stateConnection.Actions.Add(action);
            }

            return retList;
        }

        public static List<AutoMessage> auto_messages(DBResultSet results)
        {
            List<AutoMessage> retList = new List<AutoMessage>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                retList.Add(new AutoMessage()
                {
                    AutoMessageID = table.GetGuid(i, "AutoMessageID"),
                    OwnerID = table.GetGuid(i, "OwnerID"),
                    BodyText = table.GetString(i, "BodyText"),
                    AudienceType = table.GetEnum<AudienceTypes>(i, "AudienceType"),
                    RefState = new State()
                    {
                        StateID = table.GetGuid(i, "RefStateID"),
                        Title = table.GetString(i, "RefStateTitle")
                    },
                    Node = new Node()
                    {
                        NodeID = table.GetGuid(i, "NodeID"),
                        Name = table.GetString(i, "NodeName"),
                        NodeTypeID = table.GetGuid(i, "NodeTypeID"),
                        NodeType = table.GetString(i, "NodeType")
                    },
                    Admin = table.GetBool(i, "Admin")
                });
            }

            return retList;
        }

        public static List<History> history(DBResultSet results)
        {
            List<History> retList = new List<History>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                retList.Add(new History()
                {
                    HistoryID = table.GetGuid(i, "HistoryID"),
                    PreviousHistoryID = table.GetGuid(i, "PreviousHistoryID"),
                    OwnerID = table.GetGuid(i, "OwnerID"),
                    WorkFlowID = table.GetGuid(i, "WorkFlowID"),
                    DirectorNode = new Node()
                    {
                        NodeID = table.GetGuid(i, "DirectorNodeID"),
                        Name = table.GetString(i, "DirectorNodeName"),
                        NodeType = table.GetString(i, "DirectorNodeType")
                    },
                    DirectorUserID = table.GetGuid(i, "DirectorUserID"),
                    State = new State()
                    {
                        StateID = table.GetGuid(i, "StateID"),
                        Title = table.GetString(i, "StateTitle")
                    },
                    SelectedOutStateID = table.GetGuid(i, "SelectedOutStateID"),
                    Description = table.GetString(i, "Description"),
                    Sender = new User()
                    {
                        UserID = table.GetGuid(i, "SenderUserID"),
                        UserName = table.GetString(i, "SenderUserName"),
                        FirstName = table.GetString(i, "SenderFirstName"),
                        LastName = table.GetString(i, "SenderLastName")
                    },
                    SendDate = table.GetDate(i, "SendDate"),
                    PollID = table.GetGuid(i, "PollID"),
                    PollName = table.GetString(i, "PollName")
                });
            }

            return retList;
        }

        public static List<HistoryFormInstance> history_form_instances(DBResultSet results)
        {
            List<HistoryFormInstance> retList = new List<HistoryFormInstance>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                retList.Add(new HistoryFormInstance()
                {
                    HistoryID = table.GetGuid(i, "HistoryID"),
                    OutStateID = table.GetGuid(i, "OutStateID"),
                    FormsID = table.GetGuid(i, "FormsID")
                });
            }

            return retList;
        }

        public static List<NodesCount> items_count(DBResultSet results)
        {
            List<NodesCount> retList = new List<NodesCount>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                retList.Add(new NodesCount()
                {
                    NodeTypeID = table.GetGuid(i, "NodeTypeID"),
                    TypeName = table.GetString(i, "NodeType"),
                    Count = table.GetInt(i, "Count")
                });
            }

            return retList;
        }

        public static List<Tag> tags(DBResultSet results)
        {
            List<Tag> retList = new List<Tag>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                retList.Add(new Tag()
                {
                    TagID = table.GetGuid(i, "TagID"),
                    Text = table.GetString(i, "Tag")
                });
            }

            return retList;
        }

        public static List<KeyValuePair<string, int>> service_abstract(DBResultSet results)
        {
            List<KeyValuePair<string, int>> retList = new List<KeyValuePair<string, int>>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                string tag = table.GetString(i, "Tag");
                int count = table.GetInt(i, "Count", defaultValue: 0).Value;

                retList.Add(new KeyValuePair<string, int>(tag, count));
            }

            return retList;
        }
    }
}
