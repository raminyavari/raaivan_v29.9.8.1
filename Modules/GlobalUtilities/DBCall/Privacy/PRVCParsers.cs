using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaaiVan.Modules.GlobalUtilities;

namespace RaaiVan.Modules.Privacy
{
    public static class PRVCParsers
    {
        public static Dictionary<Guid, List<Audience>> audience(DBResultSet results)
        {
            Dictionary<Guid, List<Audience>> ret = new Dictionary<Guid, List<Audience>>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                Audience audience = new Audience()
                {
                    ObjectID = table.GetGuid(i, "ObjectID"),
                    RoleID = table.GetGuid(i, "RoleID"),
                    PermissionType = table.GetEnum<PermissionType>(i, "PermissionType", PermissionType.None),
                    Allow = table.GetBool(i, "Allow"),
                    ExpirationDate = table.GetDate(i, "ExpirationDate"),
                    RoleName = table.GetString(i, "Name"),
                    RoleType = table.GetString(i, "Type"),
                    NodeType = table.GetString(i, "NodeType"),
                    AdditionalID = table.GetString(i, "AdditionalID")
                };

                if (audience.ObjectID.HasValue && audience.PermissionType != PermissionType.None)
                {
                    if (!ret.ContainsKey(audience.ObjectID.Value)) ret[audience.ObjectID.Value] = new List<Audience>();

                    ret[audience.ObjectID.Value].Add(audience);
                }

            }

            return ret;
        }

        public static Dictionary<Guid, List<PermissionType>> access_checked_items(DBResultSet results)
        {
            Dictionary<Guid, List<PermissionType>> ret = new Dictionary<Guid, List<PermissionType>>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                Guid? id = table.GetGuid(i, "ID");
                PermissionType type = table.GetEnum<PermissionType>(i, "Type", PermissionType.None);

                if (id.HasValue && type != PermissionType.None)
                {
                    if (!ret.ContainsKey(id.Value)) ret[id.Value] = new List<PermissionType>();
                    if (!ret[id.Value].Any(u => u == type)) ret[id.Value].Add(type);
                }
            }

            return ret;
        }

        public static Dictionary<Guid, List<DefaultPermission>> default_permissions(DBResultSet results)
        {
            Dictionary<Guid, List<DefaultPermission>> ret = new Dictionary<Guid, List<DefaultPermission>>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                Guid? id = table.GetGuid(i, "ID");
                PermissionType type = table.GetEnum<PermissionType>(i, "Type", PermissionType.None);
                PrivacyType defaultValue = table.GetEnum<PrivacyType>(i, "DefaultValue", PrivacyType.NotSet);

                if (id.HasValue && type != PermissionType.None && defaultValue != PrivacyType.NotSet)
                {
                    if (!ret.ContainsKey(id.Value)) ret[id.Value] = new List<DefaultPermission>();

                    if (!ret[id.Value].Any(u => u.PermissionType == type)) ret[id.Value].Add(new DefaultPermission()
                    {
                        PermissionType = type,
                        DefaultValue = defaultValue
                    });
                }
            }

            return ret;
        }

        public static List<Privacy> settings(DBResultSet results)
        {
            List<Privacy> retList = new List<Privacy>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                retList.Add(new Privacy()
                {
                    ObjectID = table.GetGuid(i, "ObjectID"),
                    CalculateHierarchy = table.GetBool(i, "CalculateHierarchy"),
                    Confidentiality = new ConfidentialityLevel()
                    {
                        ID = table.GetGuid(i, "ConfidentialityID"),
                        LevelID = table.GetInt(i, "LevelID"),
                        Title = table.GetString(i, "Level")
                    }
                });
            }

            return retList;
        }

        public static List<Guid> user_ids(DBResultSet results, ref long totalCount)
        {
            List<Guid> retList = new List<Guid>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                totalCount = table.GetLong(i, "TotalCount", defaultValue: 0).Value;

                Guid? id = table.GetGuid(i, "UserID");

                if (id.HasValue) retList.Add(id.Value);
            }

            return retList;
        }

        public static List<ConfidentialityLevel> confidentiality_levels(DBResultSet results)
        {
            List<ConfidentialityLevel> retList = new List<ConfidentialityLevel>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                retList.Add(new ConfidentialityLevel()
                {
                    ID = table.GetGuid(i, "ID"),
                    LevelID = table.GetInt(i, "LevelID"),
                    Title = table.GetString(i, "Title")
                });
            }

            return retList;
        }
    }
}
