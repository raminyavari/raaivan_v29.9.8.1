using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RaaiVan.Modules.GlobalUtilities;
using System.Web.UI;

namespace RaaiVan.Modules.Reports
{
    public static class ReportsController
    {
        private static string GetFullyQualifiedName(string name, ModuleIdentifier moduleIdentifier = ModuleIdentifier.RPT)
        {
            return "[dbo]." + "[" + moduleIdentifier.ToString() + "_" + name + "]"; //'[dbo].' is database owner and 'RPT_' is module qualifier
        }

        public static RVDataTable get_report(Guid applicationId, ModuleIdentifier moduleIdentifier, string reportName, 
            ref string retActions, ref Dictionary<string, string> columnsDic, List<object> parameters)
        {
            if (parameters == null) parameters = new List<object>();

            DBReadOptions options = new DBReadOptions()
            {
                IsReport = true
            };

            DBResultSet results = DBConnector.read(options, applicationId, 
                GetFullyQualifiedName(reportName, moduleIdentifier), parameters.ToArray());

            RVDataTable retTable = results.get_table();
            retActions = results.get_table(1).GetString(row: 0, column: 0);

            List<Pair> otherTbls = new List<Pair>();

            for (int i = 2; i < results.TablesCount; i += 2)
                otherTbls.Add(new Pair(results.get_table(i), results.get_table(i + 1).GetString(row: 0, column: 0)));

            if (otherTbls.Count > 0) retTable = _fetch(applicationId, retTable, otherTbls, ref columnsDic);

            return retTable;
        }

        private static RVDataTable _fetch(Guid applicationId, RVDataTable mainTable,
            List<Pair> otherTables, ref Dictionary<string, string> columnsDic)
        {
            try
            {
                RVDataTable retTable = new RVDataTable();

                Dictionary<string, string> localDic = null;

                bool fetched = false;

                foreach (Pair p in otherTables)
                {
                    RVDataTable otherTable = (RVDataTable)p.First;
                    Dictionary<string, string> dic = PublicMethods.json2dictionary((string)p.Second);

                    if (dic.ContainsKey("IsDescription") && dic["IsDescription"].ToLower() == "true")
                    {
                        localDic = _parse_description_table(otherTable);
                        if (dic.ContainsKey("IsColumnsDictionary") && dic["IsColumnsDictionary"].ToLower() == "true")
                            columnsDic = localDic;
                        continue;
                    }
                    else
                    {
                        retTable = _fetch(applicationId, mainTable, otherTable, dic, localDic, ref columnsDic);
                        localDic = null;
                        fetched = true;
                    }
                }

                return fetched ? retTable : mainTable;
            }
            catch { return mainTable; }
        }

        private static RVDataTable _fetch(Guid applicationId, RVDataTable mainTable, RVDataTable otherTable,
            Dictionary<string, string> info, Dictionary<string, string> localDic, ref Dictionary<string, string> columnsDic)
        {
            if (!info.ContainsKey("ColumnsMap") || !info.ContainsKey("ColumnsToTransfer")) return mainTable;

            Dictionary<string, string> map = new Dictionary<string, string>();
            foreach (string itm in info["ColumnsMap"].Split(','))
            {
                string[] tuple = itm.Split(':');
                if (tuple.Length == 2) map[tuple[0].Trim()] = tuple[1].Trim();
            }

            List<string> transfer = info["ColumnsToTransfer"].Split(',').ToList();

            RVDataTable retTable = mainTable;

            Dictionary<string, string> colNamesDic = new Dictionary<string, string>();

            foreach (string str in transfer)
            {
                bool isValidName = (new System.Text.RegularExpressions.Regex("^[A-Za-z][A-Za-z0-9_]*$")).IsMatch(str);

                colNamesDic[str] = isValidName ? str : "r_" + PublicMethods.get_random_number(8) + "r";
                retTable.Columns.Add(colNamesDic[str], typeof(string));
                if (localDic != null && localDic.ContainsKey(str)) columnsDic[colNamesDic[str]] = localDic[str];
            }

            for (int i = 0, lnt = retTable.Rows.Count; i < lnt; ++i)
            {
                DataRow dr = null;
                foreach (DataRow r in otherTable.Rows)
                {
                    if (_is_equal(retTable.Rows[i], r, ref map)) dr = r;
                    else continue;
                }
                if (dr == null) continue;

                foreach (string str in transfer) retTable.Rows[i][colNamesDic[str]] =
                        PublicMethods.markup2plaintext(applicationId,
                        Expressions.replace(dr[str].ToString(), Expressions.Patterns.HTMLTag, " "));
            }

            return retTable;
        }

        private static bool _is_equal(DataRow sourceDataRow, DataRow destDataRow,
            ref Dictionary<string, string> columnsMap)
        {
            foreach (string key in columnsMap.Keys)
                if (sourceDataRow[key].ToString().ToLower() != destDataRow[columnsMap[key]].ToString().ToLower()) return false;
            return true;
        }

        private static Dictionary<string, string> _parse_description_table(RVDataTable tbl)
        {
            Dictionary<string, string> retDic = new Dictionary<string, string>();

            try { foreach (DataRow rw in tbl.Rows) retDic[(string)rw["ColumnName"]] = (string)rw["Translation"]; } //0: ColumnName, 1: Translation
            catch (Exception ex) { string strEx = ex.ToString(); }

            return retDic;
        }
    }
}
