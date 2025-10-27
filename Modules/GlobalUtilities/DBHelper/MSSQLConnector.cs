using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ApplicationBlocks.Data;
using RaaiVan.Modules.GlobalUtilities.DBCompositeTypes;

namespace RaaiVan.Modules.GlobalUtilities
{
    public static class MSSQLConnector
    {
        private static string _connectionString;
        public static string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                {
                    string name = "EKMConnectionString";
                    string env = RaaiVanSettings.UseLocalVariables ? string.Empty : PublicMethods.get_environment_variable("rv_" + name);
                    _connectionString = !string.IsNullOrEmpty(env) ? env :
                        System.Configuration.ConfigurationManager.ConnectionStrings[name].ConnectionString;
                }
                return _connectionString;
            }
        }

        private static RVDataTable get_table(IDataReader reader, DBReadOptions options)
        {
            RVDataTable tbl = new RVDataTable("tbl");
            return !reader2table(ref reader, ref tbl, options, closeReader: false) ? null : tbl;
        }

        public static DBResultSet read(Func<DBResultSet, bool> action, DBReadOptions options,
            string procedureName, params object[] parameters)
        {
            procedureName = "[dbo].[" + procedureName + "]";

            IDataReader reader = null;

            try
            {
                if (action != null || parameters.Any(p => p != null && typeof(IDBCompositeType).IsAssignableFrom(p.GetType())))
                    return read_structured(action, options, procedureName, parameters);

                DBResultSet ret = new DBResultSet();

                using (reader = (IDataReader)SqlHelper.ExecuteReader(ConnectionString, procedureName, parameters))
                {
                    do
                    {
                        ret.add_table(get_table(reader, options));
                    } while (reader.NextResult());

                    if (!reader.IsClosed) reader.Close();
                }

                return ret;
            }
            catch (Exception ex)
            {
                try
                {
                    if (reader != null && !reader.IsClosed)
                    {
                        reader.Close();
                        reader.Dispose();
                    }
                }
                catch { }

                throw ex;
            }
        }

        private static DBResultSet read_structured(Func<DBResultSet, bool> action, DBReadOptions options,
            string procedureName, params object[] parameters)
        {
            DBResultSet ret = new DBResultSet();

            SqlConnection con = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;

            IDataReader reader = null;

            try
            {
                List<string> args = new List<string>();

                for (int i = 0; i < parameters.Length; i++)
                {
                    object p = parameters[i];
                    string name = "@p" + (i + 1).ToString();

                    if (p == null || p == DBNull.Value) args.Add("null");
                    else if (typeof(IDBCompositeType).IsAssignableFrom(p.GetType()))
                    {
                        SqlParameter param = ((IDBCompositeType)p).toMSSQLParameter(name);
                        if (param == null) return ret;
                        cmd.Parameters.Add(param);
                        args.Add(name);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue(name, p);
                        args.Add(name);
                    }
                }

                cmd.CommandText = "EXEC" + " " + procedureName + " " + string.Join(", ", args);

                con.Open();

                SqlTransaction tran = action == null ? null : con.BeginTransaction();
                if (tran != null) cmd.Transaction = tran;

                using (reader = (IDataReader)cmd.ExecuteReader())
                {
                    do
                    {
                        ret.add_table(get_table(reader, options));
                    } while (reader.NextResult());

                    if (!reader.IsClosed) reader.Close();
                }

                if(action != null && tran != null)
                {
                    if (!action(ret)) tran.Rollback();
                    else tran.Commit();

                    tran.Dispose();
                }

                return ret;
            }
            catch (Exception ex)
            {
                try
                {
                    if (reader != null && !reader.IsClosed)
                    {
                        reader.Close();
                        reader.Dispose();
                    }
                }
                catch { }

                throw ex;
            }
            finally
            {
                con.Close();
            }
        }

        public static bool reader2table(ref IDataReader reader, ref RVDataTable retTable, 
            DBReadOptions options, bool closeReader = true)
        {
            try
            {
                int fieldCount = reader.FieldCount;

                for (int i = 0; i < fieldCount; ++i)
                {
                    string colName = reader.GetName(i);
                    if (string.IsNullOrEmpty(colName)) colName = PublicMethods.random_string(5);
                    retTable.Columns.Add(colName, reader.GetFieldType(i));
                }

                while (reader.Read())
                {
                    object[] values = new object[fieldCount];

                    for (int i = 0; i < fieldCount; ++i)
                    {
                        //if fieldCount > 1 then the result set is not report 'Actions'
                        if (options != null && options.IsReport && fieldCount > 1)
                        {
                            values[i] = reader[i].GetType() == typeof(string) && !string.IsNullOrEmpty((string)reader[i]) ?
                                ((string)reader[i]).Substring(0, Math.Min(1000, ((string)reader[i]).Length)) : reader[i];
                        }
                        else values[i] = reader[i];
                    }

                    retTable.Rows.Add(values);
                }
            }
            catch (Exception e) { closeReader = true; return false; }
            finally
            {
                if (closeReader && !reader.IsClosed)
                {
                    reader.Close();
                    reader.Dispose();
                }
            }

            return true;
        }
    }
}
