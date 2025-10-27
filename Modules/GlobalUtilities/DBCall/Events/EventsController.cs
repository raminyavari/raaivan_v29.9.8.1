using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RaaiVan.Modules.Knowledge;
using RaaiVan.Modules.CoreNetwork;
using RaaiVan.Modules.GlobalUtilities;

namespace RaaiVan.Modules.Events
{
    public static class EventsController
    {
        private static string GetFullyQualifiedName(string name) => "[dbo]." + "[EVT_" + name + "]"; //'[dbo].' is database owner and 'EVT_' is module qualifier

        public static bool create_event(Guid applicationId, Event info, List<Guid> userIds, List<Guid> groupIds, List<Guid> nodeIds)
        {
            List<Guid> relatedNodeIds = new List<Guid>();

            relatedNodeIds.AddRange(groupIds);
            relatedNodeIds.AddRange(nodeIds);

            return DBConnector.succeed(applicationId, GetFullyQualifiedName("CreateEvent"),
                applicationId, info.EventID, info.EventType, info.OwnerID, info.Title, info.Description, 
                info.BeginDate, info.FinishDate, info.CreatorUserID, info.CreationDate, 
                ProviderUtil.list_to_string<Guid>(relatedNodeIds), ProviderUtil.list_to_string<Guid>(userIds), ',');
        }

        public static bool remove_event(Guid applicationId, Guid eventId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ArithmeticDeleteEvent"), applicationId, eventId);
        }

        public static List<Event> get_events(Guid applicationId, List<Guid> eventIds, bool? full = false)
        {
            DBResultSet results = DBConnector.read(applicationId, GetFullyQualifiedName("GetEventsByIDs"),
                applicationId, ProviderUtil.list_to_string<Guid>(eventIds), ',', full);

            return EVTParsers.events(results, full: full.HasValue && full.Value);
        }

        public static Event get_event(Guid applicationId, Guid eventId, bool? full = false)
        {
            return get_events(applicationId, new List<Guid>() { eventId }, full).FirstOrDefault();
        }

        public static int get_user_finished_events_count(Guid applicationId, Guid userId, bool? done = null)
        {
            return DBConnector.get_int(applicationId, GetFullyQualifiedName("GetUserFinishedEventsCount"),
                applicationId, userId, DateTime.Now, done);
        }

        public static List<Event> get_user_finished_events(Guid applicationId, Guid userId, bool? done = null)
        {
            return EVTParsers.events(DBConnector.read(applicationId, GetFullyQualifiedName("GetUserFinishedEvents"),
                applicationId, userId, DateTime.Now, done));
        }

        public static List<Guid> get_related_user_ids(Guid applicationId, Guid eventId)
        {
            return DBConnector.get_guid_list(applicationId, GetFullyQualifiedName("GetRelatedUserIDs"), applicationId, eventId);
        }

        public static List<RelatedUser> get_related_users(Guid applicationId, Guid eventId)
        {
            DBResultSet results = DBConnector.read(applicationId, GetFullyQualifiedName("GetRelatedUsers"), applicationId, eventId);
            return EVTParsers.related_users(results, userInfo: true);
        }

        public static bool remove_related_user(Guid applicationId, Guid eventId, Guid userId, ref bool calenderDeleted)
        {
            int result = DBConnector.get_int(applicationId, GetFullyQualifiedName("ArithmeticDeleteRelatedUser"),
                applicationId, eventId, userId);

            calenderDeleted = result == 2 ? true : false;

            return result > 0 ? true : false;
        }

        public static bool change_user_status(Guid applicationId, Guid eventId, Guid userId, string status)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ChangeUserStatus"),
                applicationId, eventId, userId, status);
        }

        public static List<Node> get_related_nodes(Guid applicationId, Guid eventId, NodeTypes? nodeType = null)
        {
            string strNodeTypeId = null;
            if (nodeType.HasValue) strNodeTypeId = CNUtilities.get_node_type_additional_id(nodeType.Value).ToString();

            List<Guid> nodeIds = DBConnector.get_guid_list(applicationId, GetFullyQualifiedName("GetRelatedNodeIDs"),
                applicationId, eventId, strNodeTypeId);

            return CNController.get_nodes(applicationId, nodeIds, full: null, currentUserId: null);
        }

        public static List<Event> get_node_related_events(Guid applicationId, 
            Guid nodeId, DateTime? beginDate = null, bool? notFinished = null)
        {
            return EVTParsers.events(DBConnector.read(applicationId, GetFullyQualifiedName("GetNodeRelatedEvents"),
                applicationId, nodeId, beginDate, notFinished));
        }

        public static List<RelatedUser> get_user_related_events(Guid applicationId, Guid userId, 
            DateTime? beginDate = null, bool? notFinished = null, UserStatus? status = null, Guid? nodeId = null)
        {
            DBResultSet results = DBConnector.read(applicationId, GetFullyQualifiedName("GetUserRelatedEvents"),
                applicationId, userId, beginDate, notFinished, status, nodeId);

            return EVTParsers.related_users(results, userInfo: false, eventInfo: true);
        }
    }
}
