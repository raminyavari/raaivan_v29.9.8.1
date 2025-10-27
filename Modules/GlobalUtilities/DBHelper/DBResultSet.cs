using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaaiVan.Modules.GlobalUtilities
{
    public class DBResultSet
    {
        private List<RVDataTable> Tables = new List<RVDataTable>();

        private string _ConnectionErrorMessage = null;

        public string ConnectionErrorMessage
        {
            get { return _ConnectionErrorMessage; }
        }

        public DBResultSet() { }

        public DBResultSet(string connectionErrorMessage)
        {
            _ConnectionErrorMessage = connectionErrorMessage;
        }

        public int TablesCount { get { return Tables.Count; } }

        public void add_table(RVDataTable tbl)
        {
            if (tbl != null) Tables.Add(tbl);
        }

        public RVDataTable get_table(int tableIndex = 0)
        {
            return tableIndex >= Tables.Count ? new RVDataTable() : Tables[tableIndex];
        }
    }
}
