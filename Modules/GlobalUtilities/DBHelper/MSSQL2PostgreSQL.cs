using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace RaaiVan.Modules.GlobalUtilities
{
    public enum MSSQLDataType
    {
        None,

        BIGINT,
        BIT,
        CHAR,
        DATETIME,
        DECIMAL,
        FLOAT,
        IMAGE,
        INT,
        NTEXT,
        NVARCHAR,
        UNIQUEIDENTIFIER,
        VARBINARY,
        VARCHAR
    }

    public class SchemaInfo
    {
        public string Table;
        public string Column;
        public bool? IsPrimaryKey;
        public bool? IsIdentity;
        public bool? IsNullable;
        public MSSQLDataType DataType;
        public int? MaxLength;
        public int? Order;
        public string DefaultValue;

        public SchemaInfo()
        {
            DataType = MSSQLDataType.None;
        }

        private static string toScript(string tableName, List<SchemaInfo> columns, bool? withPartitioning)
        {
            string partitionColumnName = "ApplicationID";
            int partitionsCount = 10;

            if (!withPartitioning.HasValue) withPartitioning = false;
            else if (!columns.Any(c => c.Column.ToLower() == partitionColumnName.ToLower())) withPartitioning = false;

            List<string> strColumns = columns.Select(c =>
            {
                string dt = MSSQL2PostgreSQL.resolve_data_type(c.DataType, c.MaxLength, c.IsIdentity);
                string nullable = c.IsNullable.HasValue && c.IsNullable.Value ? string.Empty : " NOT NULL";
                string defaultValue = c.DataType == MSSQLDataType.UNIQUEIDENTIFIER &&
                    !string.IsNullOrEmpty(c.DefaultValue) && c.DefaultValue.ToLower().Contains("newid()") ?
                    " DEFAULT gen_random_uuid()" : string.Empty;

                return "\"" + MSSQL2PostgreSQL.resolve_name(c.Column) + "\" " + dt + nullable + defaultValue;
            }).Where(x => !string.IsNullOrEmpty(x)).ToList();

            List<string> primary = columns
                .Where(c => c.IsPrimaryKey.HasValue && c.IsPrimaryKey.Value)
                .Select(c => MSSQL2PostgreSQL.resolve_name(c.Column)).ToList();

            if (primary.Count > 0) strColumns.Add("PRIMARY KEY (" + string.Join(", ", primary.Select(p => "\"" + p + "\"")) + ")");

            string pgTableName = MSSQL2PostgreSQL.resolve_table_name(tableName);

            string strPartitioning = !withPartitioning.HasValue || !withPartitioning.Value ? string.Empty : 
                " PARTITION BY HASH (" + MSSQL2PostgreSQL.resolve_name(partitionColumnName) + ")";

            string strPartitions = string.IsNullOrEmpty(strPartitioning) ? string.Empty :
                "\r\n\r\n" + string.Join("\r\n\r\n", Enumerable.Range(1, partitionsCount).Select(number => {
                    string partitionName = pgTableName + "_" + number.ToString();

                    return "CREATE TABLE IF NOT EXISTS " + partitionName + " PARTITION OF " + pgTableName + "\r\n\t" +
                        "FOR VALUES WITH (MODULUS " + partitionsCount.ToString() + ", REMAINDER " + (number - 1).ToString() + ");";
                }));

            return "CREATE TABLE IF NOT EXISTS " + pgTableName + " (" +
                "\r\n\t" +
                string.Join(",\r\n\t", strColumns) +
                "\r\n" +
                ")" + strPartitioning + ";" + strPartitions;
        }

        public static string toScript(List<SchemaInfo> info, bool? withPartitioning)
        {
            List<string> tables = info.Select(i => i.Table).Distinct().ToList();

            return string.Join("\r\n\r\n\r\n", tables.Select(t => toScript(t, MSSQL2PostgreSQL.get_columns(t, info), withPartitioning)));
        }

        public static string toScript_UserDefinedTableTypes(string typeName, List<SchemaInfo> columns)
        {
            return "DROP TYPE IF EXISTS " + MSSQL2PostgreSQL.resolve_name(typeName) + ";\r\n\r\n" + 
                "CREATE TYPE " + MSSQL2PostgreSQL.resolve_name(typeName) + " AS (\r\n\t" +
                string.Join(",\r\n\t", columns.Select(c => "\"" + MSSQL2PostgreSQL.resolve_name(c.Column) + "\" " +
                    MSSQL2PostgreSQL.resolve_data_type(c.DataType, c.MaxLength, c.IsIdentity))) + 
                "\r\n);";
        }

        public static string toScript_UserDefinedTableTypes(List<SchemaInfo> info)
        {
            List<string> tables = info.Select(i => i.Table)
                .Where(i => !i.ToLower().StartsWith("keyless"))
                .Distinct().ToList();

            return string.Join("\r\n\r\n\r\n", tables.Select(t => toScript_UserDefinedTableTypes(t, MSSQL2PostgreSQL.get_columns(t, info))));
        }

        public static string toScript_FullTextIndexes(string tableName, List<SchemaInfo> columns)
        {
            string indexName = "ix_fts_" + MSSQL2PostgreSQL.resolve_table_name(tableName);

            return "DROP INDEX IF EXISTS " + indexName + ";\r\n\r\n" +
                "CREATE INDEX " + indexName + " ON " + MSSQL2PostgreSQL.resolve_table_name(tableName) + "\r\n" + 
                "USING pgroonga (\r\n\t" +
                string.Join(",\r\n\t", columns.Select(c =>
                {
                    string dataType = MSSQL2PostgreSQL.resolve_data_type(c.DataType, c.MaxLength, c.IsIdentity);

                    return "\"" + MSSQL2PostgreSQL.resolve_name(c.Column) + "\"" +
                        (dataType.ToLower().Contains("varchar") ? " pgroonga_varchar_full_text_search_ops_v2" : string.Empty);
                })) +
                "\r\n);";
        }

        public static string toScript_FullTextIndexes(List<SchemaInfo> info)
        {
            info.Where(i => i.Table.ToLower() == "usr_view_users" && i.Column.ToLower() == "username")
                .ToList().ForEach(i => i.Table = "aspnet_Users");

            info.Where(i => i.Table.ToLower() == "usr_view_users").ToList().ForEach(i => i.Table = "USR_Profile");

            List<string> tables = info.Select(i => i.Table).Distinct().ToList();

            return "CREATE EXTENSION IF NOT EXISTS pgroonga;\r\n\r\n\r\n" +  
                string.Join("\r\n\r\n\r\n", tables.Select(t => toScript_FullTextIndexes(t, MSSQL2PostgreSQL.get_columns(t, info))));
        }
    }
    
    public class ForeignKey
    {
        public string Name;
        public string Table;
        public string Column;
        public string RefTable;
        public string RefColumn;

        public string toScript()
        {
            string constraintName = "fk_" + MSSQL2PostgreSQL.resolve_table_name(Table) + "_" + 
                MSSQL2PostgreSQL.resolve_name(Column) + "_" +
                MSSQL2PostgreSQL.resolve_table_name(RefTable) + "_" + 
                MSSQL2PostgreSQL.resolve_name(RefColumn);

            return
                "ALTER TABLE " + MSSQL2PostgreSQL.resolve_table_name(Table) + "\r\n" +
                "ADD CONSTRAINT " + MSSQL2PostgreSQL.resolve_constraint_name(constraintName) + "\r\n" +
                "FOREIGN KEY(" + MSSQL2PostgreSQL.resolve_name(Column) + ")\r\n" +
                "REFERENCES " + MSSQL2PostgreSQL.resolve_table_name(RefTable) + "(" + MSSQL2PostgreSQL.resolve_name(RefColumn) + ")" +
                ";"; // "\r\n" + "ON DELETE CASCADE;";
        }

        public static string toScript(List<ForeignKey> foreignKeys)
        {
            return string.Join("\r\n\r\n", foreignKeys.Select(t => t.toScript()));
        }
    }

    public class DBIndex
    {
        public string Name;
        public string Table;
        public string Column;
        public int? Order;
        public bool? IsDescending;
        public bool? IsUnique;
        public bool? IsIncludedColumn;
        
        private static string toScript(string indexName, List<DBIndex> data)
        {
            if (data != null) data = data
                    .Where(d => d.Name == indexName)
                    .OrderBy(d => !d.Order.HasValue ? 0 : d.Order.Value)
                    .ToList();

            if (data == null || data.Count == 0) return string.Empty;

            string table = data[0].Table;
            bool isUnique = data[0].IsUnique.HasValue && data[0].IsUnique.Value;
            
            string constraintName = (isUnique ? "ux_" : "ix_") + 
                MSSQL2PostgreSQL.resolve_table_name(table) + "_" +
                MSSQL2PostgreSQL.resolve_name(data[0].Column) + "_" + 
                PublicMethods.random_string(8).ToLower();

            List<string> columnNames = data.Where(d => !d.IsIncludedColumn.HasValue || !d.IsIncludedColumn.Value)
                .Select(d =>
                {
                    bool isDesc = d.IsDescending.HasValue && d.IsDescending.Value;

                    return MSSQL2PostgreSQL.resolve_name(d.Column) + (isDesc ? " DESC" : "") + " NULLS LAST";
                }).ToList();

            List<string> included = data.Where(d => d.IsIncludedColumn.HasValue && d.IsIncludedColumn.Value)
                .Select(d => MSSQL2PostgreSQL.resolve_name(d.Column)).ToList();

            return columnNames.Count == 0 ? string.Empty :
                "CREATE " + (isUnique ? "UNIQUE " : "") + "INDEX " +
                    MSSQL2PostgreSQL.resolve_constraint_name(constraintName) + "\r\n\t" +
                "ON " + MSSQL2PostgreSQL.resolve_table_name(table) + " USING btree" + "\r\n\t" + 
                "(" + string.Join(", ", columnNames) + ")" +
                (included.Count > 0 ? "\r\n\t" + "INCLUDE(" + string.Join(", ", included) + ")" : "") + ";";
        }

        public static string toScript(List<DBIndex> data)
        {
            List<string> names = data.Select(i => i.Name).Distinct().ToList();

            return string.Join("\r\n\r\n", names.Select(t => toScript(t, data)));
        }
    }

    public static class MSSQL2PostgreSQL
    {
        public static string resolve_name(string name)
        {
            if (PublicMethods.is_all_upper(name)) return name.ToLower();

            string postgreName = string.Empty;

            string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            name.ToList().ForEach(chr => postgreName += (upper.IndexOf(chr) >= 0 ? "_" : "") + chr);

            string retStr = postgreName.ToLower();

            if (retStr.StartsWith("_") && retStr.Length > 1) retStr = retStr.Substring(1);

            if (retStr == "i_d") retStr = "id";
            else if (retStr == "e_mail") retStr = "email";
            else if (retStr == "user_name") retStr = "username";
            else if (retStr == "u_r_l") retStr = "url";
            else if (retStr == "birth_day") retStr = "birthdate";

            return retStr
                .Replace("__", "_")
                .Replace("___", "_")
                .Replace("_i_d", "_id")
                .Replace("w_f_", "wf_")
                .Replace("_p_i_n", "_pin")
                .Replace("work_space", "workspace")
                .Replace("sub_type", "subtype")
                .Replace("m_i_m_e", "mime")
                .Replace("work_flow", "workflow")
                .Replace("feed_back", "feedback")
                .Replace("_f_a_q", "_faq")
                .Replace("f_a_q_", "faq_")
                .Replace("_e_mail", "_email")
                .Replace("_user_name", "_username")
                .Replace("_u_r_l", "_url")
                .Replace("aspnet_", "rv_")
                .Replace("c_n_", "cn_")
                .Replace("_f_n_", "_fn_")
                .Replace("kwf_n_", "kw_fn_")
                .Replace("member_ship", "membership");
        }

        public static string resolve_table_name(string mssqlName)
        {
            int ind = mssqlName.IndexOf("_");

            string moduleIdentifier = ind > 0 ? mssqlName.Substring(0, ind + 1).ToLower() : string.Empty;
            string tableName = string.IsNullOrEmpty(moduleIdentifier) ? mssqlName : mssqlName.Substring(ind + 1);

            return resolve_name(moduleIdentifier + tableName);
        }

        public static string resolve_data_type(MSSQLDataType type, int? maxLength, bool? identity)
        {
            string strMaxLength = !maxLength.HasValue || maxLength.Value <= 0 ?
                string.Empty : "(" + maxLength.Value.ToString() + ")";

            string retStr = string.Empty;

            switch (type)
            {
                case MSSQLDataType.INT:
                    retStr = identity.HasValue && identity.Value ? "SERIAL" : "INTEGER";
                    break;
                case MSSQLDataType.BIGINT:
                    retStr = identity.HasValue && identity.Value ? "BIGSERIAL" : "BIGINT";
                    break;
                case MSSQLDataType.FLOAT:
                    retStr = "FLOAT";
                    break;
                case MSSQLDataType.BIT:
                    retStr = "BOOLEAN";
                    break;
                case MSSQLDataType.CHAR:
                    retStr = "CHAR";
                    break;
                case MSSQLDataType.DATETIME:
                    retStr = "TIMESTAMP";
                    break;
                case MSSQLDataType.VARCHAR:
                    retStr = "VARCHAR" + strMaxLength;
                    break;
                case MSSQLDataType.NVARCHAR:
                    retStr = "VARCHAR" + strMaxLength;
                    break;
                case MSSQLDataType.VARBINARY:
                    retStr = "BYTEA";
                    break;
                case MSSQLDataType.UNIQUEIDENTIFIER:
                    retStr = "UUID";
                    break;
                default:
                    retStr = string.Empty;
                    break;
            }

            return retStr;
        }

        public static string resolve_constraint_name(string name) {
            return string.IsNullOrEmpty(name) || name.Length <= 39 ? name :
                name.Substring(0, 30) + "_" + PublicMethods.random_string(8).ToLower();
        }

        public static List<SchemaInfo> get_columns(string tableName, List<SchemaInfo> columns)
        {
            List<MSSQLDataType> invalidDataTypes = new List<MSSQLDataType>() {
                MSSQLDataType.None,
                MSSQLDataType.DECIMAL,
                MSSQLDataType.IMAGE,
                MSSQLDataType.NTEXT
            };

            return columns
                .Where(c => c.Table == tableName && !invalidDataTypes.Any(x => x == c.DataType) &&
                    !string.IsNullOrEmpty(resolve_data_type(c.DataType, c.MaxLength, c.IsIdentity)))
                .OrderBy(c => !c.Order.HasValue ? 0 : c.Order.Value).ToList();
        }

        private static int rows_count_from_ms_sql(string tableName)
        {
            SqlConnection con = new SqlConnection(MSSQLConnector.ConnectionString);
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = con;
            cmd.CommandText = "SELECT COUNT(*) FROM dbo." + tableName;

            con.Open();
            
            try
            {
                IDataReader reader = (IDataReader)cmd.ExecuteReader();

                RVDataTable tbl = new RVDataTable("tbl");
                MSSQLConnector.reader2table(ref reader, ref tbl, options: null);

                return tbl.GetInt(row: 0, column: 0, defaultValue: 0).Value;
            }
            catch (Exception ex) { return 0; }
            finally { con.Close(); }
        }

        private static DataTable read_from_ms_sql(string tableName, int from, int to, string orderBy)
        {
            SqlConnection con = new SqlConnection(MSSQLConnector.ConnectionString);
            SqlCommand cmd = new SqlCommand();

            if (string.IsNullOrEmpty(orderBy)) orderBy = "GETDATE()";

            cmd.Connection = con;
            cmd.CommandText = 
                "SELECT * " + 
                "FROM ( " + 
                        "SELECT ROW_NUMBER() OVER(ORDER BY " + orderBy + " ASC) AS RNDORDER, * " +
                        "FROM dbo." + tableName + 
                    ") AS X " + 
                "WHERE X.RNDORDER BETWEEN " + from.ToString() + " AND " + to.ToString() + " " +
                "ORDER BY X.RNDORDER ASC";

            con.Open();

            try
            {
                IDataReader reader = (IDataReader)cmd.ExecuteReader();

                RVDataTable dt = new RVDataTable("tbl");

                if (MSSQLConnector.reader2table(ref reader, ref dt, options: null)) return dt;
                else return null;
            }
            catch (Exception ex) { return null; }
            finally { con.Close(); }
        }

        private static int transfer_data(string tableName, List<SchemaInfo> columns, int from, int to, string orderBy)
        {
            DataTable table = read_from_ms_sql(tableName, from, to, orderBy);

            int succeedCount = 0;

            if (table != null)
            {
                List<string> colNames = new List<string>();

                for (int i = 0; i < table.Columns.Count; i++)
                    colNames.Add(table.Columns[i].ColumnName);

                colNames = colNames.Where(n => columns.Any(c => c.Column == n && (!c.IsIdentity.HasValue || !c.IsIdentity.Value))).ToList();

                for (int i = 0; i < table.Rows.Count; i++)
                {
                    DataRow row = table.Rows[i];

                    Dictionary<string, object> data = new Dictionary<string, object>();

                    colNames.ForEach(n => data[resolve_name(n)] = row[n]);

                    if (PostgreSQLConnector.insert(resolve_table_name(tableName), data)) succeedCount++;
                }
            }

            return succeedCount;
        }

        public static int transfer_data(string tableName, List<SchemaInfo> columns)
        {
            if (!PostgreSQLConnector.delete(resolve_table_name(tableName))) return 0;

            string orderBy = columns.Where(c => c.IsIdentity.HasValue && c.IsIdentity.Value)
                .Select(c => c.Column).FirstOrDefault();

            int rowsCount = rows_count_from_ms_sql(tableName), succeedCount = 0;

            int step = 1000, from = 1;

            while (from <= rowsCount)
            {
                succeedCount += transfer_data(tableName, columns, from, to: from + step - 1, orderBy: orderBy);
                from += step;
            }

            return succeedCount;
        }

        public static Dictionary<string, object> transfer_data(List<SchemaInfo> info)
        {
            List<string> tables = info.Select(i => i.Table).Distinct().ToList();

            Dictionary<string, object> result = new Dictionary<string, object>();

            int total = 0;

            tables.ForEach(tbl =>
            {
                int count = transfer_data(tbl, get_columns(tbl, info));

                total += count;

                result["total"] = total;
                result[tbl] = count;
            });

            return result;
        }

        private static string convert_boolean_check_or_assignment(string script, string name)
        {
            //0 as isrelated
            //isnull(isrelated, 0) = 0
            //isnull(x.isrelated, 1) = 1

            string resolvedName = resolve_name(name);

            string nameExpression = string.Join("", name.ToCharArray()
                .Select(c => !char.IsLetter(c) ? c.ToString() : "[" + c.ToString().ToLower() + c.ToString().ToUpper() + "]"));

            script = Expressions.replace(script, @"\." + nameExpression + @"\s*\=\s*1", "." + resolvedName + " = TRUE");
            script = Expressions.replace(script, @"\." + nameExpression + @"\s*\=\s*0", "." + resolvedName + " = FALSE");

            script = Expressions.replace(script, @"\." + nameExpression + @"\s*\,\s*1", "." + resolvedName + ", TRUE");
            script = Expressions.replace(script, @"\." + nameExpression + @"\s*\,\s*0", "." + resolvedName + ", FALSE");

            script = Expressions.replace(script, @"\s" + nameExpression + @"\s*\=\s*1", " " + resolvedName + " = TRUE");
            script = Expressions.replace(script, @"\s" + nameExpression + @"\s*\=\s*0", " " + resolvedName + " = FALSE");

            script = Expressions.replace(script, @"\(\s*" + nameExpression + @"\s*\,\s*1\s*\)", "." + resolvedName + ", TRUE");
            script = Expressions.replace(script, @"\(\s*" + nameExpression + @"\s*\,\s*0\s*\)", "." + resolvedName + ", FALSE");

            return resolvedName.ToLower() == name.ToLower() ? script : convert_boolean_check_or_assignment(script, resolvedName);
        }

        private static string convert_mssql_to_postgresql_one_line(string script, int lineNumber)
        {
            //resove pattern: [dbo].[table_name]
            {
                List<string> matches = Expressions.get_matches_string(script, @"\[dbo\].\[[a-zA-Z0-9_]+\]").Distinct().ToList();

                int preLength = "[dbo].[".Length;

                matches.ForEach(mth =>
                {
                    int lnt = mth.Length - preLength - 1;
                    script = script.Replace(mth, resolve_table_name(mth.Substring(preLength, lnt)));
                });
            }
            //end of resove pattern: [dbo].[table_name]


            //resove pattern: ' AS column_name,'
            {
                List<string> matches = Expressions.get_matches_string(script, @"\s[aA][sS]\s+\[?[a-zA-Z0-9_]*\]?").Distinct().ToList();

                matches.ForEach(mth =>
                {
                    string alias = mth.Replace("[", "").Replace("]", "").Substring(mth.ToLower().IndexOf("as") + 2).Trim();
                    script = script.Replace(mth, " AS " + resolve_name(alias));
                });
            }
            //end of resove pattern: ' AS [dbo].[table_name],'


            //resolve pattern: alias.column_name
            {
                List<string> matches = Expressions.get_matches_string(script, @"\[?[a-zA-Z_0-9]*\]?\.\[?[a-zA-Z_0-9]*\]?")
                    .Distinct().ToList();

                matches.ForEach(mth =>
                {
                    string[] parts = mth.Replace("[", "").Replace("]", "").Split('.');
                    script = script.Replace(mth, resolve_name(parts[0]) + "." + resolve_name(parts[1]));
                });
            }
            //end of resolve pattern: alias.column_name


            //modify 'CREATE VIEW' statements
            if (script.ToLower().Trim().StartsWith("create view"))
                script = script.Substring(0, script.IndexOf(" with ", StringComparison.OrdinalIgnoreCase));
            //end of modify 'CREATE VIEW' statements


            //convert '[name]' to 'name'
            {
                List<string> matches = Expressions.get_matches_string(script, @"\[[a-zA-Z0-9_]+\]").Distinct().ToList();

                matches.ForEach(mth =>
                {
                    string alias = resolve_name(mth.Substring(1, mth.Length - 2));
                    script = script.Replace(mth, alias);
                });
            }
            //end of convert '[name]' to 'name'


            //convert boolean checks and assignments
            new List<string>() {
                "Searchable",
                "Deleted",
                "IsKnowledge",
                "IsDocument",
                "Validated",
                "IsPending",
                "HideCreators",
                "IsApproved",
                "Approved",
                "SocialApproved",
                "Allow",
                "CalculateHierarchy",
                "RemoveHierarchy",
                "ParentDeleted",
                "Compulsory",
                "MatchAll",
                "IsTemporary",
                "IsSender",
                "HasPicture",
                "Like",
                "LikeStatus",
                "SetupService",
                "Archive",
                "UniqueMembership",
                "UniqueAdminMember",
                "Exists",
                "Done",
                "TypeDeleted",
                "Full",
                "GrabNoContentServices",
                "NoContent",
                "CheckAccess",
                "HasNodeTypeID",
                "Admin",
                "IsAdmin",
                "Exact",
                "Or",
                "IsRelated",
                "IsTagged",
                "AllowMultiple",
                "AreFriends",
                "AutoGenerated",
                "Necessary",
                "HasFormLimit",
                "NoContent",
                "BitValue",
                "Anonymous",
                "NotAuthorized"
            }.Distinct().ToList().ForEach(name => script = convert_boolean_check_or_assignment(script, name));
            //end of convert boolean checks and assignments


            return script
                .Replace("ISNULL(", "COALESCE(")
                .Replace("NEWID()", "gen_random_uuid()")
                .Replace("newid()", "gen_random_uuid()")
                .Replace("CAST(1 AS bit)", "TRUE")
                .Replace("CAST(0 AS bit)", "FALSE");
        }

        public static string convert_mssql_to_postgresql(string script)
        {
            if (string.IsNullOrEmpty(script)) return string.Empty;

            //convert 'GO' to ';'
            script = Expressions.replace(script, @"\s+[gG][oO](?=\s+)", ";");
            script = Expressions.replace(script, @"\s+[gG][oO]$", ";");

            //delete CREATE INDEX statements of views
            script = Expressions.replace(script, @"[iI][fF]\s[^;]*[sS][yY][sS]\.[iI][nN][dD][eE][xX][eE][sS][^;]*;\s*", "");
            script = Expressions.replace(script, @"[cC][rR][eE][aA][tT][eE][^;]+[iI][nN][dD][eE][xX]\s+[^;]*;\s*", "");

            //delete 'SET options ON or OFF'
            script = Expressions.replace(script, @"[sS][eE][tT]\s+[^;]*\s+([oO][nN]|[oO][fF][fF])\s*;\s*", "");


            //convert DROP VIEW statements
            {
                List<string> matches = Expressions.get_matches_string(script,
                    @"[iI][fF]\s[^;]*[sS][yY][sS]\.[vV][iI][eE][wW][sS][^;]*\s[dD][rR][oO][pP]\s[^;]*;\s*").Distinct().ToList();

                matches.ForEach(mth =>
                {
                    string ptrn = "DROP VIEW";
                    string alias = mth.Substring(mth.IndexOf(ptrn, StringComparison.OrdinalIgnoreCase) + ptrn.Length).Trim();
                    script = script.Replace(mth, "DROP VIEW IF EXISTS " + alias + "\r\n\r\n");
                });
            }
            //end of convert DROP VIEW statements


            //convert DROP PROCEDURE statements
            {
                List<string> matches = Expressions.get_matches_string(script,
                    @"[iI][fF]\s[^;]*[iI][sS][pP][rR][oO][cC][eE][dD][uU][rR][eE][^;]*\s[dD][rR][oO][pP]\s[^;]*;\s*")
                    .Distinct().ToList();

                matches.AddRange(Expressions.get_matches_string(script,
                    @"[iI][fF]\s[^;]*[oO][bB][jJ][eE][cC][tT]_[iI][dD]\s[^;]*\'[pP][rR][oO][cC][eE][dD][uU][rR][eE]\'[^;]*\s[dD][rR][oO][pP]\s[^;]*;\s*")
                    .Distinct());

                matches.ForEach(mth =>
                {
                    string ptrn = "DROP PROCEDURE";
                    string alias = mth.Substring(mth.IndexOf(ptrn, StringComparison.OrdinalIgnoreCase) + ptrn.Length).Trim();
                    script = script.Replace(mth, "DROP PROCEDURE IF EXISTS " + alias + "\r\n\r\n");
                });
            }
            //end of convert DROP PROCEDURE statements


            //convert DROP FUNCTION statements
            {
                List<string> matches = Expressions.get_matches_string(script,
                    @"[iI][fF]\s[^;]*\s[dD][rR][oO][pP]\s+[fF][uU][nN][cC][tT][iI][oO][nN][^;]*;\s*")
                    .Distinct().ToList();

                matches.ForEach(mth =>
                {
                    string ptrn = "DROP FUNCTION";
                    string alias = mth.Substring(mth.IndexOf(ptrn, StringComparison.OrdinalIgnoreCase) + ptrn.Length).Trim();
                    script = script.Replace(mth, "DROP FUNCTION IF EXISTS " + alias + "\r\n\r\n");
                });
            }
            //end of convert DROP PROCEDURE statements


            //convert datatypes
            script = Expressions.replace(script, @"[uU][nN][iI][qQ][uU][eE][iI][dD][eE][nN][tT][iI][fF][iI][eE][rR]", "UUID");
            script = Expressions.replace(script, @"\s[bB][iI][tT](?![a-zA-Z0-9_])", " BOOLEAN");
            script = Expressions.replace(script, @"\s[iI][nN][tT](?![a-zA-Z0-9_])", " INTEGER");
            script = Expressions.replace(script, @"\s[dD][aA][tT][eE][tT][iI][mM][eE](?![a-zA-Z0-9_])", " TIMESTAMP");
            script = Expressions.replace(script, @"\s[nN][vV][aA][rR][cC][hH][aA][rR]\(", " VARCHAR(");
            script = Expressions.replace(script, @"\s[vV][aA][rR][cC][hH][aA][rR]\([mM][aA][xX]\)", " VARCHAR");
            //end of convert datatypes


            //convert variable names
            Expressions.get_matches_string(script, @"\@[a-zA-Z0-9_]+")
                .Distinct()
                .Select(mth => mth.Substring(1)) //remove the @ character
                .ToList()
                .ForEach(mth => script = script.Replace("@" + mth, "vr_" + resolve_name(mth)));
            //end of convert variable names


            //convert the script line by line
            script = string.Join("\n", script.Split('\n').Select((ln, ind) => convert_mssql_to_postgresql_one_line(ln, ind)));


            return script;
        }
    }
}
