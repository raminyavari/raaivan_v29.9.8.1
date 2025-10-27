using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Collections.Specialized;
using System.Xml;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.CoreNetwork;
using RaaiVan.Modules.Users;

namespace RaaiVan.Modules.Jobs
{
    public class Recommender
    {
        protected enum AnalyzeContextType : int
        {
            Node2Node = 1,
            Node2User = 2,
            User2User = 3
        }

        protected enum AnalyzeEntityType : int
        {
            User = 1,
            Node = 2,
            NodeType = 3
        }

        //All Odd numbers are reserved for reverse relations. the id of reverse relations is (RelationID + 1)
        protected enum AnalyzeRelationType : int
        {
            Friend = 1,
            NodeCreator = 3,
            NodeMember = 5,
            Expert = 7,
            NodeLike = 9,
            ItemVisit = 11,
            IsA = 13,
            WikiChange = 15,
            UsedAsTag = 17,
            TaggedItem = 19,
            NodeRelation = 21,
            Related = 23
        }

        protected static AnalyzeContext _create_analyze_context(AnalyzeContextType contextType)
        {
            List<AnalyzeConfigLimitFilterItem> recommends = new List<AnalyzeConfigLimitFilterItem>();
            List<AnalyzeConfigSelectionCount> selectionCounts = new List<AnalyzeConfigSelectionCount>();
            List<AnalyzeConfigRelation> relations = new List<AnalyzeConfigRelation>();
            AnalyzeConfigTimeDependence timeDependence = AnalyzeConfigTimeDependence.Independent;
            double timeFactor = 0;
            int targetRelationTypeId = 0;
            List<int> targetRelationDependentTypeIDs = new List<int>();

            switch (contextType)
            {
                case AnalyzeContextType.Node2Node:
                    recommends.Add(new AnalyzeConfigLimitFilterItem()
                    {
                        EntityTypeID = (int)AnalyzeEntityType.Node,
                        RecommendedEntityTypeID = (int)AnalyzeEntityType.Node
                    });

                    selectionCounts.Add(new AnalyzeConfigSelectionCount()
                    {
                        EntityTypeID = (int)AnalyzeEntityType.Node,
                        SelectionCount = 100
                    });

                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.Expert, Value = 0.3 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.Expert + 1, Value = 0.3 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.Friend, Value = 0 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.Friend + 1, Value = 0 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.IsA, Value = 0.8 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.IsA + 1, Value = 0.8 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.ItemVisit, Value = 0.15 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.ItemVisit + 1, Value = 0.15 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.NodeCreator, Value = 0.3 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.NodeCreator + 1, Value = 0.3 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.NodeLike, Value = 0.3 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.NodeLike + 1, Value = 0.3 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.NodeMember, Value = 0.3 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.NodeMember + 1, Value = 0.3 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.NodeRelation, Value = 0.7 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.NodeRelation + 1, Value = 0.7 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.Related, Value = 0.6 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.Related + 1, Value = 0.6 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.TaggedItem, Value = 0.8 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.TaggedItem + 1, Value = 0.8 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.UsedAsTag, Value = 0.3 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.UsedAsTag + 1, Value = 0.3 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.WikiChange, Value = 0.3 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.WikiChange + 1, Value = 0.3 });

                    timeDependence = AnalyzeConfigTimeDependence.Independent;
                    timeFactor = 0;
                    break;
                case AnalyzeContextType.Node2User:
                    recommends.Add(new AnalyzeConfigLimitFilterItem()
                    {
                        EntityTypeID = (int)AnalyzeEntityType.Node,
                        RecommendedEntityTypeID = (int)AnalyzeEntityType.User
                    });

                    selectionCounts.Add(new AnalyzeConfigSelectionCount()
                    {
                        EntityTypeID = (int)AnalyzeEntityType.User,
                        SelectionCount = 100
                    });

                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.Expert, Value = 0.6 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.Expert + 1, Value = 0.6 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.Friend, Value = 0.2 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.Friend + 1, Value = 0.2 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.IsA, Value = 0.2 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.IsA + 1, Value = 0.2 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.ItemVisit, Value = 0.8 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.ItemVisit + 1, Value = 0.8 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.NodeCreator, Value = 0.7 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.NodeCreator + 1, Value = 0.7 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.NodeLike, Value = 0.5 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.NodeLike + 1, Value = 0.5 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.NodeMember, Value = 0.6 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.NodeMember + 1, Value = 0.6 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.NodeRelation, Value = 0.2 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.NodeRelation + 1, Value = 0.2 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.Related, Value = 0.2 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.Related + 1, Value = 0.2 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.TaggedItem, Value = 0.2 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.TaggedItem + 1, Value = 0.2 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.UsedAsTag, Value = 0.7 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.UsedAsTag + 1, Value = 0.7 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.WikiChange, Value = 0.7 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.WikiChange + 1, Value = 0.7 });

                    timeDependence = AnalyzeConfigTimeDependence.HighlyDependent;
                    timeFactor = 2;
                    break;
                case AnalyzeContextType.User2User:
                    recommends.Add(new AnalyzeConfigLimitFilterItem()
                    {
                        EntityTypeID = (int)AnalyzeEntityType.User,
                        RecommendedEntityTypeID = (int)AnalyzeEntityType.User
                    });

                    selectionCounts.Add(new AnalyzeConfigSelectionCount()
                    {
                        EntityTypeID = (int)AnalyzeEntityType.User,
                        SelectionCount = 100
                    });

                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.Expert, Value = 0.5 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.Expert + 1, Value = 0.5 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.Friend, Value = 0.8 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.Friend + 1, Value = 0.8 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.IsA, Value = 0 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.IsA + 1, Value = 0 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.ItemVisit, Value = 0.4 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.ItemVisit + 1, Value = 0.4 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.NodeCreator, Value = 0.2 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.NodeCreator + 1, Value = 0.2 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.NodeLike, Value = 0.4 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.NodeLike + 1, Value = 0.4 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.NodeMember, Value = 0.4 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.NodeMember + 1, Value = 0.4 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.NodeRelation, Value = 0 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.NodeRelation + 1, Value = 0 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.Related, Value = 0 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.Related + 1, Value = 0 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.TaggedItem, Value = 0 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.TaggedItem + 1, Value = 0 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.UsedAsTag, Value = 0.4 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.UsedAsTag + 1, Value = 0.4 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.WikiChange, Value = 0.5 });
                    relations.Add(new AnalyzeConfigRelation() { TypeID = (int)AnalyzeRelationType.WikiChange + 1, Value = 0.5 });

                    timeDependence = AnalyzeConfigTimeDependence.LowlyDependent;
                    timeFactor = 1;

                    targetRelationTypeId = (int)AnalyzeRelationType.Friend;
                    targetRelationDependentTypeIDs.Add((int)AnalyzeRelationType.ItemVisit);
                    targetRelationDependentTypeIDs.Add((int)AnalyzeRelationType.ItemVisit + 1);
                    targetRelationDependentTypeIDs.Add((int)AnalyzeRelationType.TaggedItem);
                    targetRelationDependentTypeIDs.Add((int)AnalyzeRelationType.TaggedItem + 1);
                    targetRelationDependentTypeIDs.Add((int)AnalyzeRelationType.UsedAsTag);
                    targetRelationDependentTypeIDs.Add((int)AnalyzeRelationType.UsedAsTag + 1);
                    break;
            }

            return new AnalyzeContext()
            {
                ID = (int)contextType,
                Relations = relations,
                SelectionCounts = selectionCounts,
                ConsiderRecommendsAsFilter = false,
                Recommends = recommends,
                TargetRelationTypeID = targetRelationTypeId,
                TargetRelationDependentTypeIDs = targetRelationDependentTypeIDs,
                TimeDependence = timeDependence,
                AnalyzeRelationTimeFactor = timeFactor
            };
        }
        
        public static void send_data_to_analyzer(Guid applicationId)
        {
            if (!RaaiVanSettings.Recommender.Enabled(applicationId)) return;

            string variableName = "LastAnalyzedID";

            long lastAnalyzedId = 0;

            if (!long.TryParse(GlobalController.get_variable(applicationId, variableName), out lastAnalyzedId))
                lastAnalyzedId = 0;
            
            XmlDocument xmlRes = PublicMethods.json2xml("{\"root\":" +
                get_deleted_states(applicationId, 250, lastAnalyzedId + 1) + "}");

            XmlNodeList items = xmlRes.GetElementsByTagName("Items");

            long lastId = 0;
            if (xmlRes["root"] == null || xmlRes["root"]["LastID"] == null ||
                !long.TryParse(xmlRes["root"]["LastID"].InnerText, out lastId)) return;

            List<AnalyzeEntity> entities = new List<AnalyzeEntity>(), deletedEntities = new List<AnalyzeEntity>();
            List<AnalyzeRelation> relations = new List<AnalyzeRelation>(), deletedRelations = new List<AnalyzeRelation>();

            foreach (XmlNode itm in items)
            {
                string id = itm["ID"] == null ? string.Empty : itm["ID"].InnerText;
                string type = itm["Type"] == null ? string.Empty : itm["Type"].InnerText;
                bool deleted = itm["Deleted"] == null ? false : itm["Deleted"].InnerText.ToLower() == "true";
                bool isRelation = itm["IsRelation"] == null ? false : itm["IsRelation"].InnerText.ToLower() == "true";
                string relSourceId = itm["RelSourceID"] == null ? string.Empty : itm["RelSourceID"].InnerText;
                string relDestinationId = itm["RelDestinationID"] == null ? string.Empty : itm["RelDestinationID"].InnerText;
                string relSourceType = itm["RelSourceType"] == null ? string.Empty : itm["RelSourceType"].InnerText;
                string relDestinationType = itm["RelDestinationType"] == null ? string.Empty : itm["RelDestinationType"].InnerText;
                bool bidirectional = itm["Bidirectional"] == null ? false : itm["Bidirectional"].InnerText.ToLower() == "true";
                bool hasReverse = bidirectional || itm["HasReverse"] == null ? false : itm["HasReverse"].InnerText.ToLower() == "true";

                DateTime _tm = new DateTime();
                DateTime? time = null;
                if (DateTime.TryParse(itm["Time"] == null ? string.Empty : itm["Time"].InnerText, out _tm)) time = _tm;

                AnalyzeEntityType entityType = AnalyzeEntityType.Node;
                AnalyzeRelationType relationType = AnalyzeRelationType.Related;

                int entityTypeId = Enum.TryParse<AnalyzeEntityType>(type, out entityType) ?
                    (int)entityType : (int)AnalyzeEntityType.Node;
                int relationTypeId = Enum.TryParse<AnalyzeRelationType>(type, out relationType) ?
                    (int)relationType : (int)AnalyzeRelationType.Related;

                int typeId = isRelation ? relationTypeId : entityTypeId;
                int relSourceTypeId = 0;
                int relDestinationTypeId = 0;

                if (isRelation)
                {
                    relSourceTypeId = Enum.TryParse<AnalyzeEntityType>(relSourceType, out entityType) ?
                        (int)entityType : (int)AnalyzeEntityType.Node;
                    relDestinationTypeId = Enum.TryParse<AnalyzeEntityType>(relDestinationType, out entityType) ?
                        (int)entityType : (int)AnalyzeEntityType.Node;

                    List<AnalyzeRelation> lst = new List<AnalyzeRelation>() {
                        new AnalyzeRelation() {
                            ID = id,
                            TypeID = typeId,
                            SourceID = relSourceId,
                            SourceTypeID = relSourceTypeId,
                            DestinationID = relDestinationId,
                            DestinationTypeID = relDestinationTypeId,
                            Bidirectional = bidirectional,
                            Time = time
                        }
                    };

                    if (hasReverse)
                    {
                        lst.Add(new AnalyzeRelation()
                        {
                            ID = new string(id.ToCharArray().Reverse().ToArray()),
                            TypeID = typeId + 1,
                            SourceID = relDestinationId,
                            SourceTypeID = relDestinationTypeId,
                            DestinationID = relSourceId,
                            DestinationTypeID = relSourceTypeId
                        });
                    }

                    if (deleted) deletedRelations.AddRange(lst);
                    else relations.AddRange(lst);
                }
                else
                {
                    AnalyzeEntity entity = new AnalyzeEntity()
                    {
                        ID = id,
                        TypeID = typeId,
                        Time = time
                    };

                    if (deleted) deletedEntities.Add(entity);
                    else entities.Add(entity);
                }
            } //end of 'foreach (XmlNode itm in items)'

            if (entities.Count == 0 && deletedEntities.Count == 0 &&
                relations.Count == 0 && deletedRelations.Count == 0) return;

            RaaiVanAnalyzerClientAPI clientAPI = new RaaiVanAnalyzerClientAPI(RaaiVanSettings.Recommender.Username(applicationId),
                RaaiVanSettings.Recommender.Password(applicationId), RaaiVanSettings.Recommender.URL(applicationId));

            //Analyze Config
            List<AnalyzeContext> configContexts = new List<AnalyzeContext>();

            configContexts.Add(_create_analyze_context(AnalyzeContextType.Node2Node));
            configContexts.Add(_create_analyze_context(AnalyzeContextType.Node2User));
            configContexts.Add(_create_analyze_context(AnalyzeContextType.User2User));

            string result = clientAPI.AnalyzeConfiguration(configContexts);
            //end of Analyze Config

            if (entities.Count > 0) result = clientAPI.InsertEntities(entities);
            if (deletedEntities.Count > 0) result = clientAPI.DeleteEntities(deletedEntities);
            if (relations.Count > 0) result = clientAPI.InsertRelations(relations);
            if (deletedRelations.Count > 0) result = clientAPI.DeleteRelations(deletedRelations);

            Guid? userId = UsersController.get_user_id(applicationId, "admin");
            if (userId.HasValue) GlobalController.set_variable(applicationId, variableName, lastId.ToString(), userId.Value);
        }

        protected static string _get_deleted_state_json(string id, string type, DateTime? time, bool? deleted, 
            bool? isRelation, Guid? relSourceId, string relSourceType, Guid? relDestinationId, string relDestinationType,
            bool bidirectional, bool hasReverse)
        {
            if (!isRelation.HasValue || !relSourceId.HasValue || !relDestinationId.HasValue) isRelation = false;
            if (!time.HasValue) time = new DateTime(2011, 6, 15);

            string json = "{\"ID\":\"" + id + "\"" +
                ",\"Type\":\"" + type + "\"" +
                ",\"Time\":\"" + (!time.HasValue ? string.Empty : time.ToString()) + "\"" +
                ",\"Deleted\":" + (deleted.HasValue && deleted.Value).ToString().ToLower() +
                ",\"IsRelation\":" + isRelation.Value.ToString().ToLower() +
                ",\"RelSourceID\":\"" + (isRelation.Value ? relSourceId.ToString() : string.Empty) + "\"" +
                ",\"RelDestinationID\":\"" + (isRelation.Value ? relDestinationId.ToString() : string.Empty) + "\"" +
                ",\"RelSourceType\":\"" + (isRelation.Value ? relSourceType : string.Empty) + "\"" +
                ",\"RelDestinationType\":\"" + (isRelation.Value ? relDestinationType : string.Empty) + "\"" +
                ",\"Bidirectional\":" + (isRelation.Value ? bidirectional : false).ToString().ToLower() +
                ",\"HasReverse\":" + (isRelation.Value ? hasReverse : false).ToString().ToLower() +
                "}";

            return json;
        }

        protected static string get_deleted_states(Guid applicationId, int? count, long? lowerBoundary)
        {
            List<DeletedState> deletedStates =
                GlobalController.get_deleted_states(applicationId, count, lowerBoundary);

            string response = "{\"FirstID\":" + (deletedStates.Count > 0 ? deletedStates.First().ID.Value : 0).ToString() +
                ",\"LastID\":" + (deletedStates.Count > 0 ? deletedStates.Last().ID.Value : 0).ToString() +
                ",\"Items\":[";

            List<Guid> nodeIds = new List<Guid>();

            bool isFirst = true;
            foreach (DeletedState ds in deletedStates)
            {
                if (ds.ObjectType == "EmailAddress") continue;

                if (ds.ObjectType == "Node") nodeIds.Add(ds.ObjectID.Value);

                bool isRelation = ds.RelSourceID.HasValue && ds.RelDestinationID.HasValue;

                response += (isFirst ? string.Empty : ",") +
                    _get_deleted_state_json(ds.ObjectID.Value.ToString(), ds.ObjectType, ds.Date, ds.Deleted, isRelation,
                    ds.RelSourceID, ds.RelSourceType, ds.RelDestinationID, ds.RelDestinationType,
                    ds.Bidirectional.HasValue && ds.Bidirectional.Value, ds.HasReverse.HasValue && ds.HasReverse.Value);

                if (ds.ObjectType == "TaggedItem" && isRelation && ds.RelCreatorID.HasValue)
                {
                    _get_deleted_state_json(Guid.NewGuid().ToString(), "UsedAsTag", ds.Date, false, true,
                    ds.RelCreatorID, "User", ds.RelDestinationID, ds.RelDestinationType, false, true);
                }

                isFirst = false;
            }

            if (nodeIds.Count > 0)
            {
                List<Node> nodes = CNController.get_nodes(applicationId, nodeIds, full: null, currentUserId: null);

                foreach (Node nd in nodes)
                {
                    response += (isFirst ? string.Empty : ",") +
                        _get_deleted_state_json(Guid.NewGuid().ToString(), "IsA", nd.CreationDate, false, true,
                        nd.NodeID.Value, "Node", nd.NodeTypeID.Value, "NodeType", false, true);

                    isFirst = false;
                }
            } //end of 'if(nodeIds.Count > 0)'

            return response + "]}";
        }
    }
}
