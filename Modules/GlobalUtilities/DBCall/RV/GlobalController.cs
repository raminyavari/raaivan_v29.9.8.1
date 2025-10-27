using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading;
using RaaiVan.Modules.GlobalUtilities.DBCompositeTypes;

namespace RaaiVan.Modules.GlobalUtilities
{
    public class GlobalController
    {
        private static string GetFullyQualifiedName(string name)
        {
            return "[dbo]." + "[RV_" + name + "]"; //'[dbo].' is database owner and 'CN_' is module qualifier
        }

        public static string get_system_version()
        {
            return DBConnector.get_string(null, GetFullyQualifiedName("GetSystemVersion"));
        }

        public static bool set_applications(List<KeyValuePair<Guid, string>> applications)
        {
            if (applications == null) applications = new List<KeyValuePair<Guid, string>>();

            DBCompositeType<GuidStringTableType> appsParam = new DBCompositeType<GuidStringTableType>()
                .add(applications.Select(app => new GuidStringTableType(app.Key, app.Value)).ToList());

            return DBConnector.succeed(null, GetFullyQualifiedName("SetApplications"), appsParam);
        }

        public static List<Application> get_applications(List<Guid> applicationIds)
        {
            if (applicationIds == null || applicationIds.Count == 0) return new List<Application>();

            return RVParsers.applications(DBConnector.read(null, GetFullyQualifiedName("GetApplicationsByIDs"),
                string.Join(",", applicationIds.Select(id => id.ToString())), ','));
        }

        public static List<Application> get_applications(int? count, int? lowerBoundary, ref int totalCount)
        {
            DBResultSet results = DBConnector.read(null, GetFullyQualifiedName("GetApplications"), count, lowerBoundary);
            return RVParsers.applications(results, ref totalCount);
        }

        public static List<Application> get_applications()
        {
            int totalCount = 0;
            return get_applications(count: 1000000, lowerBoundary: null, totalCount: ref totalCount);
        }

        public static List<Application> get_user_applications(Guid userId, bool isCreator = false, bool? archive = false)
        {
            return RVParsers.applications(DBConnector.read(null, GetFullyQualifiedName("GetUserApplications"),
                userId, isCreator, archive));
        }

        public static Application get_user_application(Guid userId, Guid applicationId)
        {
            return get_user_applications(userId).Where(a => a.ApplicationID == applicationId).FirstOrDefault();
        }

        public static bool add_or_modify_application(Guid applicationId, 
            string name, string title, string description, Guid? currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("AddOrModifyApplication"),
                applicationId, name, title, description, currentUserId, DateTime.Now);
        }

        public static bool set_application_size(Guid applicationId, string size)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetApplicationSize"), applicationId, size);
        }

        public static bool set_application_field_of_expertise(Guid applicationId, Guid fieldId, string fieldName)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetApplicationFieldOfExpertise"), 
                applicationId, fieldId, fieldName);
        }

        public static bool remove_application(Guid applicationId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("RemoveApplication"), applicationId);
        }

        public static bool recycle_application(Guid applicationId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("RecycleApplication"), applicationId);
        }

        public static bool add_user_to_application(Guid applicationId, Guid userId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("AddUserToApplication"), applicationId, userId, DateTime.Now);
        }

        public static bool remove_user_from_application(Guid applicationId, Guid userId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("RemoveUserFromApplication"), applicationId, userId);
        }

        public static bool set_variable(Guid? applicationId, string name, string value, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetVariable"),
                applicationId, name, value, currentUserId, DateTime.Now);
        }

        public static string get_variable(Guid? applicationId, string name)
        {
            return DBConnector.get_string(applicationId, GetFullyQualifiedName("GetVariable"), applicationId, name);
        }

        private static long? _set_owner_variable(Guid applicationId, long? id, Guid? ownerId,
            string name, string value, Guid currentUserId)
        {
            long? result = DBConnector.get_long(applicationId, GetFullyQualifiedName("SetOwnerVariable"),
                applicationId, id, ownerId, name, value, currentUserId, DateTime.Now);

            return result.HasValue && result.Value <= 0 ? null : result;
        }

        public static long? set_owner_variable(Guid applicationId, long id, string name, string value, Guid currentUserId)
        {
            Guid? ownerId = null;
            return _set_owner_variable(applicationId, id, ownerId, name, value, currentUserId);
        }

        public static long? set_owner_variable(Guid applicationId, Guid ownerId, string name, string value, Guid currentUserId)
        {
            long? id = null;
            return _set_owner_variable(applicationId, id, ownerId, name, value, currentUserId);
        }

        private static List<Variable> _get_owner_variables(Guid applicationId,
            long? id, Guid? ownerId, string name, Guid? creatorUserId)
        {
            return RVParsers.variables(DBConnector.read(applicationId, GetFullyQualifiedName("GetOwnerVariables"),
                applicationId, id, ownerId, name, creatorUserId));
        }

        public static List<Variable> get_owner_variables(Guid applicationId, Guid ownerId, string name, Guid? creatorUserId)
        {
            long? id = null;
            return _get_owner_variables(applicationId, id, ownerId, name, creatorUserId);
        }

        public static Variable get_owner_variable(Guid applicationId, long id)
        {
            return _get_owner_variables(applicationId, id, ownerId: null, name: null, creatorUserId: null).FirstOrDefault();
        }

        public static bool remove_owner_variable(Guid applicationId, long id, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("RemoveOwnerVariable"),
                applicationId, id, currentUserId, DateTime.Now);
        }

        public static bool add_emails_to_queue(Guid applicationId, List<EmailQueueItem> items)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("AddEmailsToQueue"), 
                applicationId, EmailQueueItemTableType.getCompositeType(items));
        }

        public static List<EmailQueueItem> get_email_queue_items(Guid applicationId, int? count = 100)
        {
            return RVParsers.email_queue_items(DBConnector.read(applicationId, GetFullyQualifiedName("GetEmailQueueItems"),
                applicationId, count));
        }

        public static bool archive_email_queue_items(Guid applicationId, List<long> itemIds)
        {
            if (itemIds == null || itemIds.Count == 0) return true;
            
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ArchiveEmailQueueItems"), 
                applicationId, BigIntTableType.getCompositeType(itemIds), DateTime.Now);
        }

        public static List<KeyValuePair<string, Guid>> get_guids(Guid applicationId,
            List<string> ids, string type, bool? exist, bool? createIfNotExist)
        {
            return RVParsers.guids(DBConnector.read(applicationId, GetFullyQualifiedName("GetGuids"),
                applicationId, StringTableType.getCompositeType(ids), type, exist, createIfNotExist));
        }

        public static List<DeletedState> get_deleted_states(Guid applicationId, int? count, long? lowerBoundary)
        {
            return RVParsers.deleted_states(DBConnector.read(applicationId, GetFullyQualifiedName("GetDeletedStates"),
                applicationId, count, lowerBoundary));
        }

        public static bool save_tagged_items(Guid applicationId, List<TaggedItem> items, bool? removeOldTags, Guid currentUserId)
        {
            if (items == null || items.Count == 0 || currentUserId == Guid.Empty) return false;
            
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SaveTaggedItems"),
                applicationId, TaggedItemTableType.getCompositeType(items), removeOldTags, currentUserId);
        }

        protected static void _save_tagged_items(object info)
        {
            SortedList<string, object> obj = (SortedList<string, object>)info;
            save_tagged_items((Guid)obj["ApplicationID"], (List<TaggedItem>)obj["Items"],
                (bool)obj["RemoveOldTags"], (Guid)obj["CurrentUserID"]);
        }

        public static void save_tagged_items_offline(Guid applicationId, List<TaggedItem> items,
            bool? removeOldTags, Guid currentUserId)
        {
            if (items.Count == 0 || currentUserId == Guid.Empty) return;

            SortedList<string, object> obj = new SortedList<string, object>();
            obj["Items"] = items;
            obj["RemoveOldTags"] = removeOldTags.HasValue && removeOldTags.Value;
            obj["CurrentUserID"] = currentUserId;
            obj["ApplicationID"] = applicationId;

            ThreadPool.QueueUserWorkItem(new WaitCallback(_save_tagged_items), obj);
        }

        public static List<TaggedItem> get_tagged_items(Guid applicationId, Guid contextId, List<TaggedType> taggedTypes)
        {
            List<string> strTagged = taggedTypes == null ? new List<string>() :
                taggedTypes.Where(t => t != TaggedType.None).Select(t => t.ToString()).ToList();

            return RVParsers.tagged_items(DBConnector.read(applicationId, GetFullyQualifiedName("GetTaggedItems"),
                applicationId, contextId, StringTableType.getCompositeType(strTagged)));
        }

        public static List<TaggedItem> get_tagged_items(Guid applicationId, Guid contextId, TaggedType taggedType)
        {
            return get_tagged_items(applicationId, contextId, new List<TaggedType>() { taggedType });
        }

        public static bool add_system_admin(Guid applicationId, Guid userId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("AddSystemAdmin"), applicationId, userId);
        }

        public static bool is_system_admin(Guid? applicationId, Guid userId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("IsSystemAdmin"), applicationId, userId);
        }

        public static string get_file_extension(Guid applicationId, Guid fileId)
        {
            return DBConnector.get_string(applicationId, GetFullyQualifiedName("GetFileExtension"), applicationId, fileId);
        }

        private static bool _like_dislike_unlike(Guid applicationId, Guid userId, Guid likedId, bool? like)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("LikeDislikeUnlike"),
                applicationId, userId, likedId, like, DateTime.Now);
        }

        public static bool like(Guid applicationId, Guid userId, Guid likedId)
        {
            return _like_dislike_unlike(applicationId, userId, likedId, like: true);
        }

        public static bool dislike(Guid applicationId, Guid userId, Guid likedId)
        {
            return _like_dislike_unlike(applicationId, userId, likedId, like: false);
        }

        public static bool unlike(Guid applicationId, Guid userId, Guid likedId)
        {
            return _like_dislike_unlike(applicationId, userId, likedId, like: null);
        }

        public static List<Guid> get_fan_ids(Guid applicationId, Guid likedId)
        {
            return DBConnector.get_guid_list(applicationId, GetFullyQualifiedName("GetFanIDs"), applicationId, likedId);
        }

        private static bool _follow_unFollow(Guid applicationId, Guid userId, Guid followedId, bool? follow)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("FollowUnfollow"),
                applicationId, userId, followedId, follow, DateTime.Now);
        }

        public static bool follow(Guid applicationId, Guid userId, Guid followedId)
        {
            return _follow_unFollow(applicationId, userId, followedId, follow: true);
        }

        public static bool unfollow(Guid applicationId, Guid userId, Guid followedId)
        {
            return _follow_unFollow(applicationId, userId, followedId, follow: null);
        }

        public static bool set_system_settings(Guid applicationId,
            Dictionary<RVSettingsItem, string> items, Guid currentUserId)
        {
            if (items == null) items = new Dictionary<RVSettingsItem, string>();

            DBCompositeType<StringPairTableType> itemsParam = new DBCompositeType<StringPairTableType>()
                .add(items.Keys.Select(i => new StringPairTableType(i.ToString(), items[i])).ToList());

            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetSystemSettings"),
                applicationId, itemsParam, currentUserId, DateTime.Now);
        }

        public static Dictionary<RVSettingsItem, string> get_system_settings(Guid applicationId, List<RVSettingsItem> names)
        {
            names = names.Where(n => n != RVSettingsItem.UseLocalVariables && n != RVSettingsItem.UsePostgreSQL).ToList();

            if (applicationId == Guid.Empty && names.Count == 0) return new Dictionary<RVSettingsItem, string>();

            return RVParsers.setting_items(DBConnector.read(applicationId, GetFullyQualifiedName("GetSystemSettings"),
                applicationId, ProviderUtil.list_to_string<RVSettingsItem>(names), ','));
        }

        public static ArrayList get_last_content_creators(Guid applicationId, int? count)
        {
            return RVParsers.last_active_users(DBConnector.read(applicationId, GetFullyQualifiedName("GetLastContentCreators"),
                applicationId, count));
        }

        public static Dictionary<string, object> raaivan_statistics(Guid applicationId, DateTime? dateFrom, DateTime? dateTo)
        {
            return RVParsers.raaivan_statistics(DBConnector.read(applicationId, GetFullyQualifiedName("RaaiVanStatistics"),
                applicationId, dateFrom, dateTo));
        }

        public static List<SchemaInfo> get_schema_info()
        {
            return RVParsers.schema_info(DBConnector.read(null, GetFullyQualifiedName("SchemaInfo")));
        }

        public static List<ForeignKey> get_foreign_keys()
        {
            return RVParsers.foreign_keys(DBConnector.read(null, GetFullyQualifiedName("ForeignKeys")));
        }

        public static List<DBIndex> get_indexes()
        {
            return RVParsers.indexes(DBConnector.read(null, GetFullyQualifiedName("Indexes")));
        }

        public static List<SchemaInfo> get_user_defined_table_types()
        {
            return RVParsers.user_defined_table_types(DBConnector.read(null, GetFullyQualifiedName("UserDefinedTableTypes")));
        }

        public static List<SchemaInfo> get_full_text_indexes()
        {
            return RVParsers.full_text_indexes(DBConnector.read(null, GetFullyQualifiedName("FullTextIndexes")));
        }
    }
}
