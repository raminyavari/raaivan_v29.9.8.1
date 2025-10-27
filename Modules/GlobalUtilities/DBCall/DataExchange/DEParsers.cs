using RaaiVan.Modules.GlobalUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaaiVan.Modules.DataExchange
{
    public static class DEParsers
    {
        public static bool update_nodes_results(DBResultSet results, ref List<Guid> nodeIds)
        {
            RVDataTable table = results.get_table();

            bool succeed = table.GetBool(row: 0, column: 0, defaultValue: false).Value;

            if (results.TablesCount > 1)
            {
                table = results.get_table(1);

                for (int i = 0; i < table.Rows.Count; i++)
                {
                    Guid? id = table.GetGuid(0, "ID");
                    if (id.HasValue) nodeIds.Add(id.Value);
                }
            }

            return succeed;
        }
    }
}
