using RaaiVan.Modules.GlobalUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaaiVan.Modules.Log
{
    public static class LGParsers
    {
        public static List<Log> logs(DBResultSet results)
        {
            List<Log> retList = new List<Log>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                retList.Add(new Log()
                {
                    LogID = table.GetLong(i, "LogID"),
                    UserID = table.GetGuid(i, "UserID"),
                    Date = table.GetDate(i, "Date"),
                    HostName = table.GetString(i, "HostName"),
                    HostAddress = table.GetString(i, "HostAddress"),
                    Info = table.GetString(i, "Info"),
                    Action = table.GetEnum<Action>(i, "Action"),
                    ModuleIdentifier = table.GetEnum<ModuleIdentifier>(i, "ModuleIdentifier")
                });
            }

            return retList.Where(lg => lg.Action.HasValue && lg.ModuleIdentifier.HasValue).ToList();
        }
    }
}
