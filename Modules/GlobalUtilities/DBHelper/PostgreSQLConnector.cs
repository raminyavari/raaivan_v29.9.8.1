using Npgsql;
using RaaiVan.Modules.GlobalUtilities.DBCompositeTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaaiVan.Modules.GlobalUtilities
{
    public static class PostgreSQLConnector
    {
        private static string HOST = "localhost";
        private static string DBNAME = "ekm_app";
        private static string USERNAME = "postgres";
        private static string PASSWORD = "hrdovjshjpvjo";

        private static bool COMPOSITE_MAPPINGS_DONE = false;

        private static string CONNECTION_STRING
        {
            get
            {
                if (!COMPOSITE_MAPPINGS_DONE)
                {
                    NpgsqlConnection.GlobalTypeMapper.MapComposite<BigIntTableType>("big_int_table_type");
                    NpgsqlConnection.GlobalTypeMapper.MapComposite<CNExtensionTableType>("cn_extension_table_type");
                    NpgsqlConnection.GlobalTypeMapper.MapComposite<DocFileInfoTableType>("doc_file_info_table_type");
                    NpgsqlConnection.GlobalTypeMapper.MapComposite<EmailQueueItemTableType>("email_queue_item_table_type");
                    NpgsqlConnection.GlobalTypeMapper.MapComposite<ExchangeAuthorTableType>("exchange_author_table_type");
                    NpgsqlConnection.GlobalTypeMapper.MapComposite<ExchangeMemberTableType>("exchange_member_table_type");
                    NpgsqlConnection.GlobalTypeMapper.MapComposite<ExchangeNodeTableType>("exchange_node_table_type");
                    NpgsqlConnection.GlobalTypeMapper.MapComposite<ExchangePermissionTableType>("exchange_permission_table_type");
                    NpgsqlConnection.GlobalTypeMapper.MapComposite<ExchangeRelationTableType>("exchange_relation_table_type");
                    NpgsqlConnection.GlobalTypeMapper.MapComposite<ExchangeUserTableType>("exchange_user_table_type");
                    NpgsqlConnection.GlobalTypeMapper.MapComposite<FormElementTableType>("form_element_table_type");
                    NpgsqlConnection.GlobalTypeMapper.MapComposite<FormFilterTableType>("form_filter_table_type");
                    NpgsqlConnection.GlobalTypeMapper.MapComposite<FormInstanceTableType>("form_instance_table_type");
                    NpgsqlConnection.GlobalTypeMapper.MapComposite<GuidFloatTableType>("guid_float_table_type");
                    NpgsqlConnection.GlobalTypeMapper.MapComposite<GuidPairBitTableType>("guid_pair_bit_table_type");
                    NpgsqlConnection.GlobalTypeMapper.MapComposite<GuidPairTableType>("guid_pair_table_type");
                    NpgsqlConnection.GlobalTypeMapper.MapComposite<GuidStringPairTableType>("guid_string_pair_table_type");
                    NpgsqlConnection.GlobalTypeMapper.MapComposite<GuidStringTableType>("guid_string_table_type");
                    NpgsqlConnection.GlobalTypeMapper.MapComposite<GuidTableType>("guid_table_type");
                    NpgsqlConnection.GlobalTypeMapper.MapComposite<MessageTableType>("message_table_type");
                    NpgsqlConnection.GlobalTypeMapper.MapComposite<PrivacyAudienceTableType>("privacy_audience_table_type");
                    NpgsqlConnection.GlobalTypeMapper.MapComposite<StringPairTableType>("string_pair_table_type");
                    NpgsqlConnection.GlobalTypeMapper.MapComposite<StringTableType>("string_table_type");
                    NpgsqlConnection.GlobalTypeMapper.MapComposite<TaggedItemTableType>("tagged_item_table_type");

                    COMPOSITE_MAPPINGS_DONE = true;
                }

                return "Host=" + HOST + "; Username=" + USERNAME + ";Password=" + PASSWORD + ";Database=" + DBNAME;
            }
        }

        public static void config(string host, string dbName, string username, string password)
        {
            if (!string.IsNullOrEmpty(host)) HOST = host;
            if (!string.IsNullOrEmpty(dbName)) DBNAME = dbName;
            if (!string.IsNullOrEmpty(username)) USERNAME = username;
            if (!string.IsNullOrEmpty(password)) PASSWORD = password;
        }

        private static Type resolve_db_data_type(Type dbType)
        {
            if (dbType == typeof(Int32)) return typeof(int);
            else if (dbType == typeof(Int64)) return typeof(long);
            else if (dbType == typeof(float)) return typeof(double);
            else if (dbType == typeof(Double)) return typeof(double);
            else if (dbType == typeof(Boolean)) return typeof(bool);
            else return dbType;
        }

        private static bool add_parameter(NpgsqlCommand cmd, object value)
        {
            Type x = typeof(int);

            if (value == null)
            {
                cmd.Parameters.AddWithValue(DBNull.Value);
                return true;
            }
            else if (typeof(IDBCompositeType).IsAssignableFrom(value.GetType()))
            {
                cmd.Parameters.Add(((IDBCompositeType)value).toNpgSqlParameter());
                return true;
            }
            else if (value.GetType() == typeof(Int32)) value = (int)value;
            else if (value.GetType() == typeof(Int64)) value = (long)value;
            else if (value.GetType() == typeof(float)) value = (double)value;
            else if (value.GetType() == typeof(Double)) value = (double)value;
            else if (value.GetType() == typeof(Boolean)) value = (bool)value;

            List<Type> validTypes = new List<Type>() {
                typeof(int),
                typeof(long),
                typeof(double),
                typeof(bool),
                typeof(char),
                typeof(string),
                typeof(DateTime),
                typeof(Guid),
                typeof(byte[])
            };

            if (!validTypes.Any(t => t == value.GetType())) return false;

            cmd.Parameters.AddWithValue(value);

            return true;
        }

        private static RVDataTable get_table(NpgsqlDataReader reader, DBReadOptions options)
        {
            RVDataTable tbl = new RVDataTable(postgreSqlMode: true);

            reader.GetColumnSchema().ToList().ForEach(col =>
            {
                string colName = string.IsNullOrEmpty(col.ColumnName) ?
                    PublicMethods.random_string(5) : col.ColumnName;
                tbl.Columns.Add(colName, resolve_db_data_type(col.DataType));
            });

            while (reader.Read())
            {
                object[] row = new object[tbl.Columns.Count];
                int cnt = reader.GetValues(row);

                //if tbl.Columns.Count > 1 then the result set is not report 'Actions'
                if (options != null && options.IsReport && tbl.Columns.Count > 1) {
                    for (int i = 0; i < row.Length; i++)
                    {
                        if (row[i] != null && row[i].GetType() == typeof(string) && !string.IsNullOrEmpty((string)row[i]))
                            row[i] = ((string)row[i]).Substring(0, Math.Min(1000, ((string)row[i]).Length));
                    }
                }

                tbl.Rows.Add(row);
            }

            return (RVDataTable)tbl;
        }

        public static DBResultSet read(Func<DBResultSet, bool> action, DBReadOptions options,
            string procedureName, params object[] parameters)
        {
            procedureName = MSSQL2PostgreSQL.resolve_table_name(procedureName);

            DBResultSet ret = new DBResultSet();
            NpgsqlConnection con = null;

            try
            {
                using (con = new NpgsqlConnection(CONNECTION_STRING))
                using (NpgsqlCommand cmd = new NpgsqlCommand(procedureName, con))
                {
                    con.Open();

                    using (NpgsqlTransaction tran = con.BeginTransaction())
                    {
                        bool handledError = false;
                        Exception unhandledException = null;

                        try
                        {
                            cmd.Transaction = tran;
                            cmd.CommandType = CommandType.StoredProcedure;

                            for (int i = 0; i < parameters.Length; i++)
                                if (!add_parameter(cmd, parameters[i])) return new DBResultSet();

                            List<string> cursors = new List<string>();

                            using (NpgsqlDataReader reader = cmd.ExecuteReader())
                            {
                                RVDataTable tbl = get_table(reader, options);

                                if (!reader.IsClosed) reader.Close();

                                //check if the results are cursors
                                //if the return table has only one column and the column name is not determined in the query,
                                //the column name will be exactly same as function name.
                                //this is true for cursors. besides, first character of a cursor is '<'

                                object firstValue = tbl.Columns.Count != 1 ||
                                    tbl.ColumnNames[0].ToLower() != procedureName.ToLower() ? null : tbl.GetValue(row: 0, column: 0);

                                string firstString = firstValue == null || firstValue.GetType() != typeof(string) ?
                                    null : (string)firstValue;

                                bool isCursor = !string.IsNullOrEmpty(firstString) && firstString[0] == '<';
                                //end of check if the results are cursors

                                if (!isCursor)
                                    ret.add_table(tbl);
                                else
                                {
                                    for (int i = 0; i < tbl.Rows.Count; i++)
                                        cursors.Add(tbl.GetString(row: i, column: 0));
                                }
                            }

                            cursors.ForEach(cr =>
                            {
                                cmd.CommandText = "FETCH ALL IN \"" + cr + "\";";
                                cmd.CommandType = CommandType.Text;

                                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                                {
                                    ret.add_table(get_table(reader, options));
                                    if (!reader.IsClosed) reader.Close();
                                }
                            });
                        }
                        catch (PostgresException ex)
                        {
                            string msg = ex?.MessageText;
                            Dictionary<string, object> hint = PublicMethods.fromJSON(ex?.Hint);

                            if (PublicMethods.get_dic_value(hint, "Type") == "RVException")
                            {
                                handledError = true;

                                int? code = PublicMethods.get_dic_value<int>(hint, "Code", defaultValue: 0);

                                ret = new DBResultSet();
                                RVDataTable tbl = new RVDataTable(postgreSqlMode: true);
                                ret.add_table(tbl);

                                tbl.Columns.Add("val", typeof(int));
                                tbl.Columns.Add("msg", typeof(string));

                                tbl.Rows.Add(code, msg);
                            }
                            else unhandledException = ex;
                        }
                        catch (Exception ex)
                        {
                            unhandledException = ex;
                        }

                        if (handledError || unhandledException != null)
                            tran.Rollback();
                        else if (action != null)
                        {
                            if (!action(ret)) tran.Rollback();
                            else tran.Commit();
                        }
                        else tran.Commit();

                        tran.Dispose();
                        con.Close();

                        if (unhandledException != null) throw unhandledException;
                    }
                }

                return ret;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                try
                {
                    if (con != null) con.Close();
                }
                catch (Exception e)
                {
                }
            }
        }

        public static bool delete(string tableName)
        {
            using (NpgsqlConnection con = new NpgsqlConnection(CONNECTION_STRING))
            {
                try
                {
                    con.Open();

                    string sql = "DELETE FROM public." + tableName;

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, con))
                    {
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    string strEx = ex.ToString();
                    return false;
                }
            }
        }

        public static bool insert(string tableName, Dictionary<string, object> row)
        {
            using (NpgsqlConnection con = new NpgsqlConnection(CONNECTION_STRING))
            {
                try
                {
                    con.Open();

                    string sql = "INSERT INTO " + tableName + "(" + string.Join(",", row.Keys.ToList()) + ") " +
                        "VALUES(" + string.Join(",", row.Keys.Select(k => row[k] == null ? "null" : "@" + k).ToList()) + ")";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, con))
                    {
                        row.Keys.Where(k => row[k] != null).ToList().ForEach(k => cmd.Parameters.AddWithValue(k, row[k]));

                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    string strEx = ex.ToString();
                    return false;
                }
            }
        }
    }
}
