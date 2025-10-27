using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaaiVan.Modules.GlobalUtilities
{
    public static class RVParsers
    {
        public static List<Application> applications(DBResultSet results, ref int totalCount)
        {
            List<Application> retList = new List<Application>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                totalCount = table.GetInt(i, "TotalCount", defaultValue: 0).Value;

                retList.Add(new Application()
                {
                    ApplicationID = table.GetGuid(i, "ApplicationID"),
                    Name = table.GetString(i, "ApplicationName"),
                    Title = table.GetString(i, "Title"),
                    Description = table.GetString(i, "Description"),
                    CreatorUserID = table.GetGuid(i, "CreatorUserID"),
                    ExpertiseFieldID = table.GetGuid(i, "ExpertiseFieldID"),
                    ExpertiseFieldName = table.GetString(i, "ExpertiseFieldName")
                });
            }

            return retList;
        }

        public static List<Application> applications(DBResultSet results) {
            int totalCount = 0;
            return applications(results, ref totalCount);
        }

        public static List<Variable> variables(DBResultSet results)
        {
            List<Variable> retList = new List<Variable>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                retList.Add(new Variable()
                {
                    ID = table.GetLong(i, "ID"),
                    OwnerID = table.GetGuid(i, "OwnerID"),
                    Name = table.GetString(i, "Name"),
                    Value = table.GetString(i, "Value"),
                    CreatorUserID = table.GetGuid(i, "CreatorUserID"),
                    CreationDate = table.GetDate(i, "CreationDate")
                });
            }

            return retList;
        }

        public static List<EmailQueueItem> email_queue_items(DBResultSet results)
        {
            List<EmailQueueItem> retList = new List<EmailQueueItem>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                EmailQueueItem item = new EmailQueueItem() {
                    ID = table.GetLong(i, "ID"),
                    SenderUserID = table.GetGuid(i, "SenderUserID"),
                    Email = table.GetString(i, "Email"),
                    Title = table.GetString(i, "Title"),
                    EmailBody = table.GetString(i, "EmailBody"),
                    Action = table.GetEnum<EmailAction>(i, "Action", defaultValue: EmailAction.None)
                };

                retList.Add(item);
            }

            return retList;
        }

        public static List<KeyValuePair<string, Guid>> guids(DBResultSet results)
        {
            List<KeyValuePair<string, Guid>> retList = new List<KeyValuePair<string, Guid>>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                string key = table.GetString(i, "ID");
                Guid? value = table.GetGuid(i, "Guid");

                if (!string.IsNullOrEmpty(key) && value.HasValue)
                    retList.Add(new KeyValuePair<string, Guid>(key, value.Value));
            }

            return retList;
        }

        public static List<DeletedState> deleted_states(DBResultSet results)
        {
            List<DeletedState> retList = new List<DeletedState>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                retList.Add(new DeletedState()
                {
                    ID = table.GetLong(i, "ID"),
                    ObjectID = table.GetGuid(i, "ObjectID"),
                    ObjectType = table.GetString(i, "ObjectType"),
                    Date = table.GetDate(i, "Date"),
                    Deleted = table.GetBool(i, "Deleted"),
                    Bidirectional = table.GetBool(i, "Bidirectional"),
                    HasReverse = table.GetBool(i, "HasReverse"),
                    RelSourceID = table.GetGuid(i, "RelSourceID"),
                    RelDestinationID = table.GetGuid(i, "RelDestinationID"),
                    RelSourceType = table.GetString(i, "RelSourceType"),
                    RelDestinationType = table.GetString(i, "RelDestinationType"),
                    RelCreatorID = table.GetGuid(i, "RelCreatorID")
                });
            }

            return retList;
        }

        public static List<TaggedItem> tagged_items(DBResultSet results)
        {
            List<TaggedItem> retList = new List<TaggedItem>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                TaggedItem item = new TaggedItem() {
                    TaggedID = table.GetGuid(i, "ID"),
                    TaggedType = table.GetEnum<TaggedType>(i, "Type", defaultValue: TaggedType.None)
                };

                if (item.TaggedType == TaggedType.None) continue;
                else if (item.TaggedType == TaggedType.Node_Form || item.TaggedType == TaggedType.Node_Wiki)
                    item.TaggedType = TaggedType.Node;

                retList.Add(item);
            }

            return retList;
        }

        public static Dictionary<RVSettingsItem, string> setting_items(DBResultSet results)
        {
            Dictionary<RVSettingsItem, string> dic = new Dictionary<RVSettingsItem, string>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                RVSettingsItem? item = table.GetEnum<RVSettingsItem>(i, "Name");
                string value = table.GetString(i, "Value");

                if (item.HasValue && !string.IsNullOrEmpty(value)) dic[item.Value] = value;
            }

            return dic;
        }

        public static ArrayList last_active_users(DBResultSet results)
        {
            ArrayList retList = new ArrayList();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                Dictionary<string, object> item = new Dictionary<string, object>();

                item["UserID"] = table.GetGuid(i, "UserID");
                item["UserName"] = Base64.encode(table.GetString(i, "UserName"));
                item["FirstName"] = Base64.encode(table.GetString(i, "FirstName"));
                item["LastName"] = Base64.encode(table.GetString(i, "LastName"));
                item["Date"] = table.GetDate(i, "Date");
                item["Types"] = table.GetString(i, "Types");

                retList.Add(item);
            }

            return retList;
        }

        public static Dictionary<string, object> raaivan_statistics(DBResultSet results)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                List<string> names = new List<string>() {
                    "NodesCount",
                    "QuestionsCount",
                    "AnswersCount",
                    "WikiChangesCount",
                    "PostsCount",
                    "CommentsCount",
                    "ActiveUsersCount",
                    "NodePageVisitsCount",
                    "SearchesCount"
                };

                foreach (string n in names)
                {
                    int? value = table.GetInt(row: i, column: n);

                    if (value.HasValue) dic[n] = value.Value;
                }
            }

            return dic;
        }

        public static List<SchemaInfo> schema_info(DBResultSet results)
        {
            List<SchemaInfo> retList = new List<SchemaInfo>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                retList.Add(new SchemaInfo()
                {
                    Table = table.GetString(i, "Table"),
                    Column = table.GetString(i, "Column"),
                    IsPrimaryKey = table.GetBool(i, "IsPrimaryKey"),
                    IsIdentity = table.GetBool(i, "IsIdentity"),
                    IsNullable = table.GetBool(i, "IsNullable"),
                    MaxLength = table.GetInt(i, "MaxLength"),
                    Order = table.GetInt(i, "Order"),
                    DefaultValue = table.GetString(i, "DefaultValue"),
                    DataType = table.GetEnum<MSSQLDataType>(i, "DataType", MSSQLDataType.None)
                });
            }

            return retList;
        }

        public static List<ForeignKey> foreign_keys(DBResultSet results)
        {
            List<ForeignKey> retList = new List<ForeignKey>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                retList.Add(new ForeignKey()
                {
                    Name = table.GetString(i, "Name"),
                    Table = table.GetString(i, "Table"),
                    Column = table.GetString(i, "Column"),
                    RefTable = table.GetString(i, "RefTable"),
                    RefColumn = table.GetString(i, "RefColumn")
                });
            }

            return retList;
        }

        public static List<DBIndex> indexes(DBResultSet results)
        {
            List<DBIndex> retList = new List<DBIndex>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                retList.Add(new DBIndex()
                {
                    Name = table.GetString(i, "Name"),
                    Table = table.GetString(i, "Table"),
                    Column = table.GetString(i, "Column"),
                    Order = table.GetInt(i, "Order"),
                    IsDescending = table.GetBool(i, "IsDescending"),
                    IsUnique = table.GetBool(i, "IsUnique"),
                    IsIncludedColumn = table.GetBool(i, "IsIncludedColumn")
                });
            }

            return retList;
        }

        public static List<SchemaInfo> user_defined_table_types(DBResultSet results)
        {
            List<SchemaInfo> retList = new List<SchemaInfo>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                retList.Add(new SchemaInfo()
                {
                    Table = table.GetString(i, "Name"),
                    Column = table.GetString(i, "Column"),
                    IsNullable = table.GetBool(i, "IsNullable"),
                    IsIdentity = table.GetBool(i, "IsIdentity"),
                    MaxLength = table.GetInt(i, "MaxLength"),
                    Order = table.GetInt(i, "Order"),
                    DataType = table.GetEnum<MSSQLDataType>(i, "DataType", defaultValue: MSSQLDataType.None)
                });
            }

            return retList;
        }

        public static List<SchemaInfo> full_text_indexes(DBResultSet results)
        {
            List<SchemaInfo> retList = new List<SchemaInfo>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                retList.Add(new SchemaInfo()
                {
                    Table = table.GetString(i, "Table"),
                    Column = table.GetString(i, "Column"),
                    IsIdentity = table.GetBool(i, "IsIdentity"),
                    MaxLength = table.GetInt(i, "MaxLength"),
                    DataType = table.GetEnum<MSSQLDataType>(i, "DataType", defaultValue: MSSQLDataType.None)
                });
            }

            return retList;
        }
    }
}
