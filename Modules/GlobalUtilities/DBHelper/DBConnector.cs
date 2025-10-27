using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaaiVan.Modules.GlobalUtilities
{
    public class DBReadOptions {
        public bool IsReport = false;
    }

    public static class DBConnector
    {
        //to be removed
        public static DBResultSet read_postgre(Guid? applicationId, string procedureName, params object[] parameters)
        {
            if (parameters == null) parameters = new object[0];

            if (procedureName.ToLower().StartsWith("[dbo].["))
                procedureName = procedureName.Substring("[dbo].[".Length, procedureName.Length - "[dbo].[".Length - 1);

            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i] != null && parameters[i].GetType() == typeof(Guid) && (Guid)parameters[i] == Guid.Empty)
                    parameters[i] = null;
            }

            try
            {
                return PostgreSQLConnector.read(action: null, options: null, procedureName, parameters);
            }
            catch {
                return new DBResultSet();
            }
        }
        //end of to be removed

        public static DBResultSet read(Func<DBResultSet, bool> action, DBReadOptions options,
            Guid? applicationId, string procedureName, params object[] parameters)
        {
            if (parameters == null) parameters = new object[0];

            if (procedureName.ToLower().StartsWith("[dbo].["))
                procedureName = procedureName.Substring("[dbo].[".Length, procedureName.Length - "[dbo].[".Length - 1);
            
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i] != null && parameters[i].GetType() == typeof(Guid) && (Guid)parameters[i] == Guid.Empty)
                    parameters[i] = null;
            }

            try
            {
                return RaaiVanSettings.UsePostgreSQL ?
                    PostgreSQLConnector.read(action, options, procedureName, parameters) :
                    MSSQLConnector.read(action, options, procedureName, parameters);
            }
            catch (Exception ex)
            {
                //check if the exception is related to the connection
                DBResultSet connectionExceptionResult = handle_connection_exception(ex);
                if (connectionExceptionResult != null) return connectionExceptionResult;

                int index = procedureName.IndexOf("_");

                if (index > 0 && !(new List<string>() { "save", "error", "log" }).All(x => procedureName.ToLower().Contains(x)))
                {
                    string moduleIdentifier = procedureName.Substring(0, procedureName.IndexOf("_"));

                    Log.LogController.save_error_log(
                        applicationId: applicationId,
                        userId: null,
                        subject: procedureName,
                        exception: ex,
                        moduleIdentifier: PublicMethods.parse_enum<ModuleIdentifier>(moduleIdentifier));
                }

                return new DBResultSet();
            }
        }

        public static DBResultSet read(Func<DBResultSet, bool> action,
            Guid? applicationId, string procedureName, params object[] parameters)
        {
            return read(action, options: null, applicationId, procedureName, parameters);
        }

        public static DBResultSet read(DBReadOptions options,
            Guid? applicationId, string procedureName, params object[] parameters)
        {
            return read(action: null, options, applicationId, procedureName, parameters);
        }

        public static DBResultSet read(Guid? applicationId, string procedureName, params object[] parameters)
        {
            return read(action: null, applicationId, procedureName, parameters);
        }

        public static DBResultSet handle_connection_exception(Exception ex)
        {
            if (ex == null) return null;
            
            string message = string.Empty;

            if (RaaiVanSettings.UsePostgreSQL)
            {
                if (ex is NpgsqlException && !string.IsNullOrEmpty(ex.InnerException?.Message) &&
                    ex.InnerException.Message.ToLower().Contains("timeout") &&
                    ex.InnerException.Message.ToLower().Contains("connection"))
                {
                    message = Messages.CannotConnectToTheDatabaseServer.ToString();
                }
                else if (!string.IsNullOrEmpty(ex.Message) && ex.Message.ToLower().Contains("no such host is known"))
                {
                    message = Messages.CannotConnectToTheDatabaseServer.ToString();
                }
                else if (ex is PostgresException)
                {
                    PostgresException px = (PostgresException)ex;

                    if (!string.IsNullOrEmpty(px.SqlState) && px.SqlState.ToLower() == "3D000".ToLower())
                        message = Messages.CannotOpenTheDatabase.ToString();
                    else if (!string.IsNullOrEmpty(px.SqlState) && px.SqlState.ToLower() == "28P01".ToLower())
                        message = Messages.DatabaseLoginFailed.ToString();
                }
            }
            else
            {
                if (!(ex is SqlException)) return null;

                switch (((SqlException)ex).Number)
                {
                    case 53:
                        message = Messages.CannotConnectToTheDatabaseServer.ToString();
                        break;
                    case 4060:
                        message = Messages.CannotOpenTheDatabase.ToString();
                        break;
                    case 233:
                    case 18456:
                        message = Messages.DatabaseLoginFailed.ToString();
                        break;
                }
            }

            return string.IsNullOrEmpty(message) ? null : new DBResultSet(connectionErrorMessage: message);
        }

        public static bool succeed(Guid? applicationId, ref string errorMessage, 
            ref List<Dashboard> retDashboards, string procedureName, params object[] parameters)
        {
            DBResultSet result = read(applicationId, procedureName, parameters);

            RVDataTable table = result.get_table();

            try
            {
                object value = table.GetValue(row: 0, column: 0);
                object error = table.GetValue(row: 0, column: 1);

                if (error != null && error.GetType() == typeof(string))
                    errorMessage += error;
                
                if (result.TablesCount > 0) retDashboards = parse_dashboards(result.get_table(1));

                if (value == null) return false;

                return value != null && ((value.GetType() == typeof(bool) && ((bool)value)) ||
                       (value.GetType() == typeof(int) && ((int)value) > 0));
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool succeed(Guid? applicationId, ref string errorMessage, string procedureName, params object[] parameters)
        {
            List<Dashboard> dashboards = new List<Dashboard>();
            return succeed(applicationId, ref errorMessage, ref dashboards, procedureName, parameters);
        }

        public static bool succeed(Guid? applicationId, ref List<Dashboard> retDashboards, 
            string procedureName, params object[] parameters)
        {
            string errorMessage = string.Empty;
            return succeed(applicationId, ref errorMessage, ref retDashboards, procedureName, parameters);
        }

        public static bool succeed(Guid? applicationId, string procedureName, params object[] parameters)
        {
            string errorMessage = string.Empty;
            List<Dashboard> dashboards = new List<Dashboard>();
            return succeed(applicationId, ref errorMessage, ref dashboards, procedureName, parameters);
        }

        public static int get_int(Guid? applicationId, string procedureName, params object[] parameters)
        {
            return read(applicationId, procedureName, parameters).get_table().GetInt(row: 0, column: 0, defaultValue: 0).Value;
        }

        public static long get_long(Guid? applicationId, string procedureName, params object[] parameters)
        {
            return read(applicationId, procedureName, parameters).get_table().GetLong(row: 0, column: 0, defaultValue: 0).Value;
        }

        public static DateTime? get_date(Guid? applicationId, string procedureName, params object[] parameters)
        {
            return read(applicationId, procedureName, parameters).get_table().GetDate(row: 0, column: 0);
        }

        public static Guid? get_guid(Guid? applicationId, string procedureName, params object[] parameters)
        {
            return read(applicationId, procedureName, parameters).get_table().GetGuid(row: 0, column: 0);
        }

        public static string get_string(Guid? applicationId, string procedureName, params object[] parameters)
        {
            return read(applicationId, procedureName, parameters).get_table().GetString(row: 0, column: 0);
        }

        public static List<string> get_string_list(Guid? applicationId, string procedureName, params object[] parameters)
        {
            RVDataTable table = read(applicationId, procedureName, parameters).get_table();
            return Enumerable.Range(0, table.Rows.Count).Select(r => table.GetString(row: r, column: 0)).ToList();
        }

        public static List<Guid> parse_guid_list(RVDataTable table)
        {
            List<Guid> lst = new List<Guid>();

            for (int i = 0, lnt = table.Rows.Count; i < lnt; ++i)
            {
                Guid? val = table.GetGuid(row: i, column: 0);
                if (val.HasValue) lst.Add(val.Value);
            }

            return lst;
        }

        public static List<Guid> parse_guid_list(DBResultSet result, ref long totalCount, ref string errorMessage)
        {
            RVDataTable table = result.get_table();

            List<Guid> lst = parse_guid_list(table);

            if (result.TablesCount > 1)
            {
                table = result.get_table(1);

                totalCount = table.GetLong(row: 0, column: 0, defaultValue: 0).Value;
                errorMessage += table.GetString(row: 0, column: 1);
            }

            return lst;
        }

        public static List<Guid> parse_guid_list(DBResultSet result, ref long totalCount) {
            string errorMessage = string.Empty;
            return parse_guid_list(result, ref totalCount, ref errorMessage);
        }

        public static List<Guid> get_guid_list(Guid? applicationId, ref long totalCount, ref string errorMessage,
            string procedureName, params object[] parameters)
        {
            DBResultSet result = read(applicationId, procedureName, parameters);
            return parse_guid_list(result, ref totalCount, ref errorMessage);
        }

        public static List<Guid> get_guid_list(Guid? applicationId, string procedureName, params object[] parameters)
        {
            string errorMessage = string.Empty;
            long totalCount = 0;
            return get_guid_list(applicationId, ref totalCount, ref errorMessage, procedureName, parameters);
        }

        public static List<Guid> get_guid_list(Guid? applicationId, ref long totalCount, string procedureName, params object[] parameters)
        {
            string errorMessage = string.Empty;
            return get_guid_list(applicationId, ref totalCount, ref errorMessage, procedureName, parameters);
        }

        public static List<Guid> get_guid_list(Guid? applicationId, 
            ref string errorMessage, string procedureName, params object[] parameters)
        {
            long totalCount = 0;
            return get_guid_list(applicationId, ref totalCount, errorMessage, procedureName, parameters);
        }

        public static List<Hierarchy> get_hierarchy(Guid? applicationId, string procedureName, params object[] parameters)
        {
            RVDataTable table = read(applicationId, procedureName, parameters).get_table();

            List<Hierarchy> ret = new List<Hierarchy>();

            for (int i = 0, lnt = table.Rows.Count; i < lnt; ++i)
            {
                Hierarchy e = new Hierarchy();

                e.ID = table.GetGuid(i, "ID");
                e.ParentID = table.GetGuid(i, "ParentID");
                e.Level = table.GetInt(i, "Level");
                e.Name = table.GetString(i, "Name");

                ret.Add(e);
            }

            return ret;
        }

        public static List<Dashboard> parse_dashboards(RVDataTable table, ref long totalCount)
        {
            List<Dashboard> retList = new List<Dashboard>();

            if (table == null) return retList;

            for(int i = 0; i < table.Rows.Count; i++)
            {
                totalCount = table.GetLong(i, "TotalCount", defaultValue: 0).Value;

                retList.Add(new Dashboard()
                {
                    DashboardID = table.GetLong(i, "ID"),
                    UserID = table.GetGuid(i, "UserID"),
                    NodeID = table.GetGuid(i, "NodeID"),
                    NodeAdditionalID = table.GetString(i, "NodeAdditionalID"),
                    NodeName = table.GetString(i, "NodeName"),
                    NodeType = table.GetString(i, "NodeType"),
                    Type = table.GetEnum<DashboardType>(i, "Type", DashboardType.NotSet),
                    SubType = table.GetEnum<DashboardSubType>(i, "SubType", DashboardSubType.NotSet),
                    Info = table.GetString(i, "Info"),
                    Removable = table.GetBool(i, "Removable"),
                    SenderUserID = table.GetGuid(i, "SenderUserID"),
                    SendDate = table.GetDate(i, "SendDate"),
                    ExpirationDate = table.GetDate(i, "ExpirationDate"),
                    Seen = table.GetBool(i, "Seen"),
                    ViewDate = table.GetDate(i, "ViewDate"),
                    Done = table.GetBool(i, "Done"),
                    ActionDate = table.GetDate(i, "ActionDate")
                });
            };

            return retList.Where(itm => itm.DashboardID.HasValue).ToList();
        }

        public static List<Dashboard> parse_dashboards(RVDataTable table) {
            long totalCount = 0;
            return parse_dashboards(table, ref totalCount);
        }

        public static int get_dashboards(RVDataTable table, ref string errorMessage, ref List<Dashboard> retDashboards)
        {
            if (table.Columns.Count <= 2)
            {
                string value = table.GetString(row: 1, column: 0);

                if (table.Columns.Count > 1 && !string.IsNullOrEmpty(value)) errorMessage += value;

                return table.GetInt(row: 0, column: 0, defaultValue: 0).Value;
            }

            retDashboards = parse_dashboards(table);

            return 1;
        }

        public static int get_dashboards(RVDataTable table, ref List<Dashboard> retDashboards) {
            string errorMessage = string.Empty;
            return get_dashboards(table, ref errorMessage, ref retDashboards);
        }

        public static int get_dashboards(Guid? applicationId, ref string errorMessage, ref List<Dashboard> retDashboards,
            string procedureName, params object[] parameters)
        {
            DBResultSet result = read(applicationId, procedureName, parameters);
            return get_dashboards(result.get_table(), ref errorMessage, ref retDashboards);
        }

        public static int get_dashboards(Guid? applicationId, ref List<Dashboard> retDashboards, 
            string procedureName, params object[] parameters)
        {
            string msg = string.Empty;
            return get_dashboards(applicationId, ref msg, ref retDashboards, procedureName, parameters);
        }

        public static List<DashboardCount> get_dashboards_count(Guid? applicationId, string procedureName, params object[] parameters)
        {
            RVDataTable table = read(applicationId, procedureName, parameters).get_table();

            List<DashboardCount> retList = new List<DashboardCount>();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                DashboardType type = table.GetEnum<DashboardType>(i, "Type", defaultValue: DashboardType.NotSet);
                string subTypeTitle = table.GetString(i, "SubType");
                DashboardSubType subType = table.GetEnum<DashboardSubType>(i, "SubType", defaultValue: DashboardSubType.NotSet);
                Guid? nodeTypeId = table.GetGuid(i, "NodeTypeID");
                string nodeType = table.GetString(i, "NodeType");

                if (subType != DashboardSubType.NotSet) subTypeTitle = string.Empty;

                if (type == DashboardType.NotSet || (subType == DashboardSubType.NotSet && !nodeTypeId.HasValue)) continue;

                DashboardCount item = retList.Where(u => u.Type == type).FirstOrDefault();

                if (item == null)
                {
                    item = new DashboardCount() { Type = type };
                    retList.Add(item);
                }

                DashboardCount subItem = new DashboardCount()
                {
                    Type = type,
                    SubType = subType,
                    SubTypeTitle = subTypeTitle,
                    NodeTypeID = nodeTypeId,
                    NodeType = nodeType
                };

                if (nodeTypeId.HasValue)
                {
                    DashboardCount ntCount = item.Sub.Where(u => u.NodeTypeID == nodeTypeId.Value).FirstOrDefault();

                    if (ntCount == null)
                    {
                        ntCount = new DashboardCount()
                        {
                            Type = type,
                            NodeTypeID = nodeTypeId,
                            NodeType = nodeType
                        };

                        item.Sub.Add(ntCount);
                    }

                    ntCount.Sub.Add(subItem);
                }
                else item.Sub.Add(subItem);

                subItem.DateOfEffect = table.GetDate(i, "DateOfEffect");
                subItem.ToBeDone = table.GetInt(i, "ToBeDone", defaultValue: 0).Value;
                subItem.NotSeen = table.GetInt(i, "NotSeen", defaultValue: 0).Value;
                subItem.Done = table.GetInt(i, "Done", defaultValue: 0).Value;
                subItem.Done = table.GetInt(i, "Done", defaultValue: 0).Value;
                subItem.DoneAndInWorkFlow = table.GetInt(i, "DoneAndInWorkFlow", defaultValue: 0).Value;
                subItem.DoneAndNotInWorkFlow = table.GetInt(i, "DoneAndNotInWorkFlow", defaultValue: 0).Value;
            }

            return retList;
        }

        public static List<KeyValuePair<Guid, int>> get_items_count_list(Guid? applicationId, 
            string procedureName, params object[] parameters)
        {
            RVDataTable table = read(applicationId, procedureName, parameters).get_table();

            List<KeyValuePair<Guid, int>> retDic = new List<KeyValuePair<Guid, int>>();

            for (int i = 0, lnt = table.Rows.Count; i < lnt; ++i)
            {
                Guid? id = table.GetGuid(i, "ID");
                int? count = table.GetInt(i, "Count");

                if (id.HasValue && count.HasValue) retDic.Add(new KeyValuePair<Guid, int>(id.Value, count.Value));
            }

            return retDic;
        }

        public static Dictionary<Guid, int> get_items_count(Guid? applicationId, string procedureName, params object[] parameters)
        {
            List<KeyValuePair<Guid, int>> lst = get_items_count_list(applicationId, procedureName, parameters);

            Dictionary<Guid, int> retDic = new Dictionary<Guid, int>();

            foreach (KeyValuePair<Guid, int> e in lst)
                retDic.Add(e.Key, e.Value);

            return retDic;
        }

        public static Dictionary<Guid, bool> get_items_status_bool(Guid? applicationId, ref long totalCount, 
            string procedureName, params object[] parameters)
        {
            RVDataTable table = read(applicationId, procedureName, parameters).get_table();

            Dictionary<Guid, bool> retDic = new Dictionary<Guid, bool>();

            for (int i = 0, lnt = table.Rows.Count; i < lnt; ++i)
            {
                totalCount = table.GetLong(row: i, column: "TotalCount", defaultValue: 0).Value;

                Guid? id = table.GetGuid(i, "ID");
                bool? val = table.GetBool(i, "Value");

                if (id.HasValue && val.HasValue) retDic.Add(id.Value, val.Value);
            }

            return retDic;
        }
    }
}
