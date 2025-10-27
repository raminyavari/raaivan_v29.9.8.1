using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.GlobalUtilities.DBCompositeTypes;

namespace RaaiVan.Modules.Privacy
{
    public static class PrivacyController
    {
        private static string GetFullyQualifiedName(string name)
        {
            return "[dbo]." + "[PRVC_" + name + "]"; //'[dbo].' is database owner and 'PRVC_' is module qualifier
        }

        public static bool initialize_confidentiality_levels(Guid applicationId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("InitializeConfidentialityLevels"), applicationId);
        }

        public static bool refine_access_roles(Guid applicationId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("RefineAccessRoles"), applicationId);
        }

        public static bool set_audience(Guid applicationId, List<Privacy> items, Guid currentUserId)
        {
            if (items == null || items.Count == 0) return true;

            DBCompositeType<GuidTableType> idsParam = new DBCompositeType<GuidTableType>()
                .add(items.Where(p => p != null && p.ObjectID.HasValue).Select(p => new GuidTableType(p.ObjectID)).ToList());

            //default permissions param
            DBCompositeType<GuidStringPairTableType> defaultPermissionsParam = new DBCompositeType<GuidStringPairTableType>();

            items.Where(p => p != null && p.ObjectID.HasValue).ToList().ForEach(p =>
            {
                p.DefaultPermissions.Where(d => d.PermissionType != PermissionType.None && d.DefaultValue != PrivacyType.NotSet)
                    .ToList().ForEach(d =>
                    {
                        defaultPermissionsParam.add(new GuidStringPairTableType(p.ObjectID, 
                            d.PermissionType.ToString(), d.DefaultValue.ToString()));
                    });
            });
            //end of default permissions param

            //audience param
            DBCompositeType<PrivacyAudienceTableType> audienceParam = new DBCompositeType<PrivacyAudienceTableType>();

            items.Where(p => p != null && p.ObjectID.HasValue).ToList().ForEach(p =>
            {
                p.Audience.Where(a => a.RoleID.HasValue && a.Allow.HasValue && a.PermissionType != PermissionType.None)
                    .ToList().ForEach(a =>
                    {
                        audienceParam.add(new PrivacyAudienceTableType(
                            objectId: p.ObjectID, 
                            roleId: a.RoleID, 
                            permissionType: a.PermissionType.ToString(), 
                            allow: a.Allow, 
                            expirationDate: a.ExpirationDate));
                    });
            });
            //end of audience param

            DBCompositeType<GuidPairBitTableType> settingsParam = new DBCompositeType<GuidPairBitTableType>()
                .add(items.Where(p => p != null && p.ObjectID.HasValue && (p.Confidentiality.LevelID.HasValue || p.CalculateHierarchy.HasValue))
                .Select(p => new GuidPairBitTableType(p.ObjectID, p.Confidentiality.ID, p.CalculateHierarchy)).ToList());

            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetAudience"),
                applicationId, idsParam, defaultPermissionsParam, audienceParam, settingsParam, currentUserId, DateTime.Now);
        }

        public static Dictionary<Guid, List<PermissionType>> check_access(Guid applicationId,
            Guid? userId, List<Guid> objectIds, PrivacyObjectType objectType, List<PermissionType> permissions)
        {
            if (permissions == null) permissions = new List<PermissionType>();

            if (!userId.HasValue) userId = Guid.NewGuid();

            if (objectIds.Count == 0) return new Dictionary<Guid, List<PermissionType>>();

            if (permissions.Count == 0)
            {
                foreach (string s in Enum.GetNames(typeof(PermissionType)))
                {
                    PermissionType pt = PermissionType.None;
                    if (Enum.TryParse<PermissionType>(s, ignoreCase: true, out pt)) permissions.Add(pt);
                }
            }

            permissions = permissions.Where(p => p != PermissionType.None).ToList();

            List<PermissionType> defaultItems = new List<PermissionType>() {
                PermissionType.Create,
                PermissionType.View,
                PermissionType.ViewAbstract,
                PermissionType.ViewRelatedItems,
                PermissionType.Download
            };

            Dictionary<Guid, List<PermissionType>> ret = new Dictionary<Guid, List<PermissionType>>();

            PublicMethods.split_list<Guid>(objectIds, 200, ids =>
            {
                DBCompositeType<StringPairTableType> permissionsParam = new DBCompositeType<StringPairTableType>()
                    .add(permissions.Select(p =>
                    {
                        string defaultPrivacy = objectType == PrivacyObjectType.Report ? PrivacyType.Restricted.ToString() : 
                        (defaultItems.Any(d => d == p) ? RaaiVanSettings.DefaultPrivacy(applicationId) : string.Empty);
                        
                        return new StringPairTableType(p.ToString(), defaultPrivacy);
                    }).ToList());

                string strObjectType = objectType == PrivacyObjectType.None ? null : objectType.ToString();

                PRVCParsers.access_checked_items(DBConnector.read(applicationId, GetFullyQualifiedName("CheckAccess"),
                    applicationId, userId, strObjectType, GuidTableType.getCompositeType(objectIds), permissionsParam, DateTime.Now))
                    .ToList().ForEach(x => ret.Add(x.Key, x.Value));
            });

            return ret;
        }

        public static Dictionary<Guid, List<PermissionType>> check_access(Guid applicationId,
            Guid? userId, List<Guid> objectIds, PrivacyObjectType objectType)
        {
            return check_access(applicationId, userId, objectIds, objectType, new List<PermissionType>());
        }

        public static List<Guid> check_access(Guid applicationId,
            Guid? userId, List<Guid> objectIds, PrivacyObjectType objectType, PermissionType permission)
        {
            return check_access(applicationId, userId, objectIds, objectType, new List<PermissionType>() { permission })
                .Keys.ToList();
        }

        public static List<PermissionType> check_access(Guid applicationId,
            Guid? userId, Guid objectId, PrivacyObjectType objectType, List<PermissionType> permissions)
        {
            Dictionary<Guid, List<PermissionType>> dic =
                check_access(applicationId, userId, new List<Guid>() { objectId }, objectType, permissions);
            return dic.ContainsKey(objectId) ? dic[objectId] : new List<PermissionType>();
        }

        public static List<PermissionType> check_access(Guid applicationId,
            Guid? userId, Guid objectId, PrivacyObjectType objectType)
        {
            Dictionary<Guid, List<PermissionType>> dic =
                check_access(applicationId, userId, new List<Guid>() { objectId }, objectType);
            return dic.ContainsKey(objectId) ? dic[objectId] : new List<PermissionType>();
        }

        public static bool check_access(Guid applicationId, Guid? userId, Guid objectId, 
            PrivacyObjectType objectType, PermissionType permission)
        {
            List<PermissionType> lst =
                check_access(applicationId, userId, objectId, objectType, new List<PermissionType>() { permission });
            return lst != null && lst.Count > 0;
        }

        public static Dictionary<Guid, List<Audience>> get_audience(Guid applicationId, List<Guid> objectIds)
        {
            return PRVCParsers.audience(DBConnector.read(applicationId, GetFullyQualifiedName("GetAudience"),
                applicationId, ProviderUtil.list_to_string<Guid>(objectIds), ','));
        }

        public static List<Audience> get_audience(Guid applicationId, Guid objectId)
        {
            Dictionary<Guid, List<Audience>> dic = get_audience(applicationId, new List<Guid>() { objectId });
            return dic.ContainsKey(objectId) ? dic[objectId] : new List<Audience>();
        }

        public static Dictionary<Guid, List<DefaultPermission>> get_default_permissions(Guid applicationId, List<Guid> objectIds)
        {
            return PRVCParsers.default_permissions(DBConnector.read(applicationId, GetFullyQualifiedName("GetDefaultPermissions"),
                applicationId, ProviderUtil.list_to_string<Guid>(objectIds), ','));
        }

        public static List<DefaultPermission> get_default_permissions(Guid applicationId, Guid objectId)
        {
            Dictionary<Guid, List<DefaultPermission>> dic =
                get_default_permissions(applicationId, new List<Guid>() { objectId });
            return dic.ContainsKey(objectId) ? dic[objectId] : new List<DefaultPermission>();
        }

        public static List<Privacy> get_settings(Guid applicationId, List<Guid> objectIds)
        {
            return PRVCParsers.settings(DBConnector.read(applicationId, GetFullyQualifiedName("GetSettings"),
                applicationId, ProviderUtil.list_to_string<Guid>(objectIds), ','));
        }

        public static Privacy get_settings(Guid applicationId, Guid objectId)
        {
            return get_settings(applicationId, new List<Guid>() { objectId }).FirstOrDefault();
        }

        public static bool add_confidentiality_level(Guid applicationId, 
            Guid id, int levelId, string title, Guid currentUserId, ref string errorMessage)
        {
            return DBConnector.succeed(applicationId, ref errorMessage, GetFullyQualifiedName("AddConfidentialityLevel"),
                applicationId, id, levelId, title, currentUserId, DateTime.Now);
        }

        public static bool modify_confidentiality_level(Guid applicationId, 
            Guid id, int newLevelId, string newTitle, Guid currentUserId, ref string errorMessage)
        {
            return DBConnector.succeed(applicationId, ref errorMessage, GetFullyQualifiedName("ModifyConfidentialityLevel"),
                applicationId, id, newLevelId, newTitle, currentUserId, DateTime.Now);
        }

        public static bool remove_confidentiality_level(Guid applicationId, Guid id, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("RemoveConfidentialityLevel"),
                applicationId, id, currentUserId, DateTime.Now);
        }

        public static List<ConfidentialityLevel> get_confidentiality_levels(Guid applicationId)
        {
            return PRVCParsers.confidentiality_levels(
                DBConnector.read(applicationId, GetFullyQualifiedName("GetConfidentialityLevels"), applicationId));
        }

        public static bool set_confidentiality_level(Guid applicationId, Guid itemId, Guid levelId, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetConfidentialityLevel"),
                applicationId, itemId, levelId, currentUserId, DateTime.Now);
        }

        public static bool unset_confidentiality_level(Guid applicationId, Guid itemId, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("UnsetConfidentialityLevel"),
                applicationId, itemId, currentUserId, DateTime.Now);
        }

        public static List<Guid> get_confidentiality_level_user_ids(Guid applicationId, 
            Guid confidentialityId, string searchText, int? count, long? lowerBoundary, ref long totalCount)
        {
            DBResultSet results = DBConnector.read(applicationId, GetFullyQualifiedName("GetConfidentialityLevelUserIDs"),
                applicationId, confidentialityId, ProviderUtil.get_search_text(searchText), count, lowerBoundary);

            return PRVCParsers.user_ids(results, ref totalCount);
        }

        public static List<Guid> get_confidentiality_user_ids(Guid applicationId, 
            Guid confidentialityId, string searchText = null, int? count = null, long? lowerBoundary = null)
        {
            long totalCount = 0;
            return get_confidentiality_level_user_ids(applicationId,
                confidentialityId, searchText, count, lowerBoundary, ref totalCount);
        }
    }
}
