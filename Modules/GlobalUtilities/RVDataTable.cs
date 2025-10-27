using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaaiVan.Modules.GlobalUtilities
{
    public class RVDataTable : DataTable
    {
        //this should be true only if it is used as database result set
        private bool PostgreSQLMode = false;

        public RVDataTable() : base("tbl")
        {
            PostgreSQLMode = false;
        }

        public RVDataTable(bool postgreSqlMode) : base("tbl")
        {
            PostgreSQLMode = postgreSqlMode;
        }

        public RVDataTable(string name, bool postgreSqlMode = false) : base(name)
        {
            PostgreSQLMode = postgreSqlMode;
        }

        public List<DataColumn> ColumnsList
        {
            get { return Enumerable.Range(0, this.Columns.Count).Select(i => this.Columns[i]).ToList(); }
        }

        public List<string> ColumnNames
        {
            get { return Enumerable.Range(0, this.Columns.Count).Select(i => this.Columns[i].ColumnName).ToList(); }
        }

        public int GetColumnIndex(string columnName)
        {
            return ColumnNames.FindIndex(n =>
            {
                return PostgreSQLMode ?
                    MSSQL2PostgreSQL.resolve_name(n) == MSSQL2PostgreSQL.resolve_name(columnName) :
                    n.ToLower() == columnName.ToLower();
            });
        }

        public List<DataRow> RowsList
        {
            get { return Enumerable.Range(0, this.Rows.Count).Select(i => this.Rows[i]).ToList(); }
        }

        public object GetValue(int row, int column, object defaultValue = null)
        {
            return Math.Min(row, column) < 0 || this.Rows.Count <= row || this.Columns.Count <= column ?
                defaultValue : (this.Rows[row][column] == DBNull.Value ? defaultValue : this.Rows[row][column]);
        }

        public object GetValue(int row, string column, object defaultValue = null)
        {
            return GetValue(row, GetColumnIndex(column), defaultValue);
        }

        public string GetString(int row, int column, string defaultValue = null)
        {
            object ret = GetValue(row, column);
            return ret == null ? defaultValue : ret.ToString();
        }

        public string GetString(int row, string column, string defaultValue = null)
        {
            return GetString(row, GetColumnIndex(column), defaultValue);
        }

        public char? GetChar(int row, int column, char? defaultValue = null)
        {
            string str = GetString(row, column);
            return string.IsNullOrEmpty(str) ? defaultValue : (char?)str[0];
        }

        public char? GetChar(int row, string column, char? defaultValue = null)
        {
            return GetChar(row, GetColumnIndex(column), defaultValue);
        }

        public Guid? GetGuid(int row, int column, Guid? defaultValue = null)
        {
            object ret = GetValue(row, column);

            if (ret == null) return defaultValue;
            else return ret.GetType() == typeof(Guid) ? (Guid?)ret : defaultValue;
        }

        public Guid? GetGuid(int row, string column, Guid? defaultValue = null)
        {
            return GetGuid(row, GetColumnIndex(column), defaultValue);
        }

        public int? GetInt(int row, int column, int? defaultValue = null)
        {
            return PublicMethods.parse_int(GetString(row, column), defaultValue);
        }

        public int? GetInt(int row, string column, int? defaultValue = null)
        {
            return GetInt(row, GetColumnIndex(column), defaultValue);
        }

        public long? GetLong(int row, int column, long? defaultValue = null)
        {
            return PublicMethods.parse_long(GetString(row, column), defaultValue);
        }

        public long? GetLong(int row, string column, long? defaultValue = null)
        {
            return GetLong(row, GetColumnIndex(column), defaultValue);
        }

        public double? GetDouble(int row, int column, double? defaultValue = null)
        {
            return PublicMethods.parse_double(GetString(row, column), defaultValue);
        }

        public double? GetDouble(int row, string column, double? defaultValue = null)
        {
            return GetDouble(row, GetColumnIndex(column), defaultValue);
        }

        public DateTime? GetDate(int row, int column, DateTime? defaultValue = null)
        {
            object ret = GetValue(row, column);

            if (ret == null) return defaultValue;
            else return ret.GetType() == typeof(DateTime) ? (DateTime?)ret : defaultValue;
        }

        public DateTime? GetDate(int row, string column, DateTime? defaultValue = null)
        {
            return GetDate(row, GetColumnIndex(column), defaultValue);
        }

        public bool? GetBool(int row, int column, bool? defaultValue = null)
        {
            return PublicMethods.parse_bool(GetString(row, column), defaultValue);
        }

        public bool? GetBool(int row, string column, bool? defaultValue = null)
        {
            return GetBool(row, GetColumnIndex(column), defaultValue);
        }

        public byte[] GetByteArray(int row, int column, byte[] defaultValue = null)
        {
            object ret = GetValue(row, column);

            if (ret == null) return defaultValue;
            else return ret.GetType() == typeof(byte[]) ? (byte[])ret : defaultValue;
        }

        public byte[] GetByteArray(int row, string column, byte[] defaultValue = null)
        {
            return GetByteArray(row, GetColumnIndex(column), defaultValue);
        }

        public T? GetEnum<T>(int row, int column, T? defaultValue = null) where T : struct
        {
            return PublicMethods.parse_enum<T>(GetString(row, column), defaultValue);
        }

        public T? GetEnum<T>(int row, string column, T? defaultValue = null) where T : struct
        {
            return GetEnum<T>(row, GetColumnIndex(column), defaultValue);
        }

        public T GetEnum<T>(int row, int column, T defaultValue) where T : struct
        {
            return PublicMethods.parse_enum<T>(GetString(row, column), defaultValue);
        }

        public T GetEnum<T>(int row, string column, T defaultValue) where T : struct
        {
            return GetEnum<T>(row, GetColumnIndex(column), defaultValue);
        }
    }
}
