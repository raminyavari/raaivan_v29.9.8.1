using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.Reports;
using RaaiVan.Modules.Users;
using RaaiVan.Modules.Log;
using RaaiVan.Modules.Privacy;
using RaaiVan.Modules.GlobalUtilities.DBCompositeTypes;

namespace RaaiVan.Web.API
{
    /// <summary>
    /// Summary description for ReportsAPI
    /// </summary>
    public class ReportsAPI : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        ParamsContainer paramsContainer = null;

        public void ProcessRequest(HttpContext context)
        {
            paramsContainer = new ParamsContainer(context, nullTenantResponse: true);
            if (!paramsContainer.ApplicationID.HasValue) return;

            string responseText = string.Empty;

            string command = PublicMethods.parse_string(context.Request.Params["Command"], false);

            switch (command)
            {
                case "GetReport":
                    string reportName = PublicMethods.parse_string(context.Request.Params["ReportName"], false);

                    ModuleIdentifier? moduleIdentifier = 
                        PublicMethods.parse_enum<ModuleIdentifier>(context.Request.Params["ModuleIdentifier"]);

                    RVLang language = PublicMethods.get_current_language();

                    //Chart Mode
                    bool chartMode = PublicMethods.parse_bool(context.Request.Params["ChartMode"], defaultValue: false).Value;
                    DateTime? chartDateFrom = PublicMethods.parse_date(context.Request.Params["ChartDateFrom"]);
                    DateTime? chartDateTo = PublicMethods.parse_date(context.Request.Params["ChartDateTo"]); //don't add a day, it will be handled later
                    string chartPeriod = PublicMethods.parse_string(context.Request.Params["ChartPeriod"], decode: false);
                    
                    DBCompositeType<BigIntTableType> periodList = !chartMode ? new DBCompositeType<BigIntTableType>() :
                        get_period_list(chartDateFrom, chartDateTo, chartPeriod, language);

                    if (chartMode && periodList.count == 0)
                    {
                        responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                        _return_response(ref responseText);
                        return;
                    }

                    Dictionary<string, object> periodsDesctructured = destructure_period_list(language, chartPeriod, periodList);
                    //end of ChartMode

                    bool excel = !chartMode && PublicMethods.parse_bool(context.Request.Params["Excel"], defaultValue: false).Value;

                    List<string> paramsList = ListMaker.get_string_items(context.Request.Params["ParamsOrder"], '|');

                    List<object> parameters = new List<object>()
                    {
                        paramsContainer.ApplicationID,
                        paramsContainer.CurrentUserID
                    };

                    if (chartMode)
                    {
                        parameters.Add(chartPeriod);
                        parameters.Add(GenericDate.fromDateTime(DateTime.Now, language).Type.ToString());
                        parameters.Add(periodList);
                    }

                    for (int i = 0; i < paramsList.Count; ++i)
                    {
                        string[] item = paramsList[i].Split(':');

                        if (item == null) item = new string[0];

                        string paramName = item.Length < 1 ? string.Empty : item[0];
                        string paramType = item.Length < 2 ? string.Empty : item[1];
                        string paramValue = context.Request.Params[paramName];

                        parameters.Add(ReportUtilities.get_parameter(paramName, paramType, paramValue));
                    }

                    Dictionary<string, string> dictionary = _get_dictionary("Dictionary");

                    int pageNumber = string.IsNullOrEmpty(context.Request.Params["PageNumber"]) ? 1 : int.Parse(context.Request.Params["PageNumber"]);
                    int pageSize = string.IsNullOrEmpty(context.Request.Params["PageSize"]) ? 100 : int.Parse(context.Request.Params["PageSize"]);

                    if (!string.IsNullOrEmpty(reportName) && moduleIdentifier.HasValue)
                    {
                        get_report(moduleIdentifier.Value, reportName, excel, ref dictionary,
                            pageNumber, pageSize, ref responseText, parameters,
                            PublicMethods.parse_string(context.Request.Params["PS"]), chartMode, periodsDesctructured);
                    }

                    _return_response(ref responseText);
                    return;

            }

            paramsContainer.return_response(PublicConsts.BadRequestResponse);
        }

        protected void _return_response(ref string responseText)
        {
            paramsContainer.return_response(ref responseText);
        }

        protected Dictionary<string, string> _get_dictionary(string dicName)
        {
            string _strDic = string.IsNullOrEmpty(HttpContext.Current.Request.Params[dicName]) ? string.Empty :
                HttpContext.Current.Request.Params[dicName];
            List<string> _dicList = ListMaker.get_string_items(_strDic, '|');
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            for (int i = 0; i < _dicList.Count; ++i)
            {
                string[] item = _dicList[i].Split(':');
                dictionary.Add(Base64.decode(item[0]), Base64.decode(item[1]));
            }
            return dictionary;
        }

        protected void get_report(ModuleIdentifier moduleIdentifier, string reportName, bool excel, 
            ref Dictionary<string, string> dic, int pageNumber, int pageSize, ref string responseText,
            List<object> parameters, string password, bool chartMode, Dictionary<string, object> chartPeriods)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            bool isSystemAdmin =
                PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value);

            RVLang language = PublicMethods.get_current_language();
            bool rtl = PublicMethods.is_rtl_language(paramsContainer.ApplicationID, language);

            Guid? reportId = ReportUtilities.get_report_id(moduleIdentifier, reportName);

            bool hasAccess = reportId.HasValue && (isSystemAdmin ||
                AuthorizationManager.has_right(AccessRoleName.Reports, paramsContainer.CurrentUserID));
            hasAccess = hasAccess && (isSystemAdmin ||
                PrivacyController.check_access(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID,
                    reportId.Value, PrivacyObjectType.Report, PermissionType.View));

            if (!hasAccess)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            string actions = string.Empty;
            Dictionary<string, string> columnsDic = new Dictionary<string, string>();

            string resolvedReportName = chartMode ? reportName + "_Chart" : reportName;

            RVDataTable tbl = ReportsController.get_report(paramsContainer.Tenant.Id,
                moduleIdentifier, resolvedReportName, ref actions, ref columnsDic, parameters);

            int firstRow = excel ? 0 : (pageSize * (pageNumber - 1));
            int lastRow = (excel ? tbl.Rows.Count : Math.Min(firstRow + pageSize, tbl.Rows.Count)) - 1;

            for (int c = 0; c < tbl.Columns.Count; ++c)
            {
                if (tbl.Columns[c].DataType != typeof(string) || tbl.Columns[c].ColumnName.IndexOf("_HTML") < 0) continue;

                tbl.Columns[c].ColumnName = tbl.Columns[c].ColumnName.Replace("_HTML", string.Empty);

                for (int r = firstRow; r <= lastRow; ++r)
                {
                    if (tbl.Rows[r][c] != DBNull.Value && !string.IsNullOrEmpty((string)tbl.Rows[r][c]))
                    {
                        //tbl.Rows[r][c] = PublicMethods.markup2plaintext(
                        //Expressions.replace((string)tbl.Rows[r][c], Expressions.Patterns.HTMLTag, " ")).Trim();

                        string str = (string)tbl.Rows[r][c];
                        str = Expressions.replace(str, Expressions.Patterns.HTMLTag, " ");
                        str = PublicMethods.markup2plaintext(paramsContainer.Tenant.Id, str).Trim();

                        tbl.Rows[r][c] = str;
                    }
                }
            }

            if (excel)
            {
                try
                {
                    Dictionary<string, bool> usedColNames = new Dictionary<string, bool>();

                    foreach (string n in dic.Values) usedColNames[n] = true;

                    for (int c = 0; c < tbl.Columns.Count; ++c)
                    {
                        if (columnsDic.ContainsKey(tbl.Columns[c].ColumnName))
                        {
                            string colName = columnsDic[tbl.Columns[c].ColumnName];
                            int num = 1;
                            while (true)
                            {
                                string tmpName = colName + (num <= 1 ? string.Empty : " (" + num.ToString() + ")");

                                if (!usedColNames.ContainsKey(tmpName))
                                {
                                    usedColNames[tmpName] = true;
                                    colName = tmpName;
                                    break;
                                }
                                else ++num;
                            }

                            tbl.Columns[c].ColumnName = colName;
                        }

                        bool isString = tbl.Columns[c].DataType == typeof(string);
                        bool isDic = tbl.Columns[c].ColumnName.IndexOf("_Dic") >= 0;

                        if (isString && isDic)
                        {
                            Dictionary<string, string> colDic = _get_dictionary(tbl.Columns[c].ColumnName);

                            for (int r = 0; r < tbl.Rows.Count; ++r)
                            {
                                bool isNull = tbl.Rows[r].ItemArray[c] == DBNull.Value || tbl.Rows[r].ItemArray[c] == null;

                                if (!isNull && colDic.ContainsKey((string)tbl.Rows[r].ItemArray[c]))
                                    tbl.Rows[r][c] = colDic[(string)tbl.Rows[r].ItemArray[c]];
                            }
                        }

                        if (isDic) tbl.Columns[c].ColumnName = tbl.Columns[c].ColumnName.Replace("_Dic", string.Empty);
                    }

                    //meta data for exported file
                    Privacy p = !reportId.HasValue ? null :
                        PrivacyController.get_settings(paramsContainer.Tenant.Id, reportId.Value);
                    string confidentiality = p == null ? null : p.Confidentiality.Title;

                    User currentUser = !paramsContainer.CurrentUserID.HasValue ? null :
                        UsersController.get_user(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value);
                    if (currentUser == null) currentUser = new User();
                    DownloadedFileMeta meta = new DownloadedFileMeta(PublicMethods.get_client_ip(HttpContext.Current),
                        currentUser.UserName, currentUser.FirstName, currentUser.LastName, confidentiality);
                    //end of meta data for exported file

                    string reportFileName = "Reports_" + PublicMethods.get_random_number().ToString();

                    ExcelUtilities.ExportToExcel(reportFileName, tbl, rtl, dic, password, meta);
                }
                catch (Exception ex)
                {
                    responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

                    LogController.save_error_log(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID,
                        "ExportReportToExcel", ex, ModuleIdentifier.RPT, LogLevel.Fatal);
                }

                return;
            }

            Dictionary<string, bool> isFloatDic = new Dictionary<string, bool>();

            responseText = "{\"ChartPeriods\":" + (chartPeriods == null ? "{}" : PublicMethods.toJSON(chartPeriods)) +
                ",\"Columns\":[";

            for (int i = 0, lnt = tbl.Columns.Count; i < lnt; ++i)
            {
                object obj = null;
                for (int j = firstRow; j <= lastRow; ++j)
                    if (tbl.Rows[j][i] != DBNull.Value && tbl.Rows[j][i] != null && !string.IsNullOrEmpty(tbl.Rows[j][i].ToString()))
                    {
                        obj = tbl.Rows[j][i];
                        break;
                    }

                bool isNumber = false;
                bool isFloat = false;
                if (obj != null)
                {
                    var objType = obj.GetType();
                    isNumber = objType == typeof(int) || objType == typeof(long) ||
                        objType == typeof(float) || objType == typeof(double) || objType == typeof(decimal);
                    isFloat = objType == typeof(float) || objType == typeof(double) || objType == typeof(decimal);
                }

                isFloatDic.Add(tbl.Columns[i].ColumnName, isFloat);

                string colTitle = columnsDic.ContainsKey(tbl.Columns[i].ColumnName) ?
                    columnsDic[tbl.Columns[i].ColumnName] : tbl.Columns[i].ColumnName;

                responseText += (i == 0 ? string.Empty : ",") + "{\"ID\":\"" + tbl.Columns[i].ColumnName + "\"" + 
                    ",\"Title\":\"" + Base64.encode(colTitle) + "\"" + 
                    ",\"Encoded\":true" +
                    ",\"IsNumber\":" + isNumber.ToString().ToLower() + "}";
            }

            responseText += "],\"Total\":" + tbl.Rows.Count.ToString() + ",\"Rows\":[";

            for (int i = firstRow; i <= lastRow; ++i)
            {
                responseText += (i == firstRow ? string.Empty : ",") + "{";
                for (int j = 0, _ln = tbl.Columns.Count; j < _ln; ++j)
                {
                    if (isFloatDic[tbl.Columns[j].ColumnName]) tbl.Rows[i].ItemArray[j] =
                        Math.Round(double.Parse((tbl.Rows[i].ItemArray[j] == DBNull.Value ? 0 : tbl.Rows[i].ItemArray[j]).ToString()), 2);

                    string strVal = tbl.Rows[i].ItemArray[j].GetType() == typeof(DateTime) ?
                        GenericDate.get_local_date((DateTime)tbl.Rows[i].ItemArray[j], true) :
                        (isFloatDic[tbl.Columns[j].ColumnName] ?
                        Math.Round(double.Parse((tbl.Rows[i].ItemArray[j] == DBNull.Value ? 0 : tbl.Rows[i].ItemArray[j]).ToString()), 2) :
                        tbl.Rows[i].ItemArray[j]).ToString();

                    responseText += (j == 0 ? string.Empty : ",") + "\"" + tbl.Columns[j].ColumnName + "\":\"" + Base64.encode(strVal) + "\"";
                }
                responseText += "}";
            }

            responseText += "],\"Actions\":" + (string.IsNullOrEmpty(actions) ? "{}" : actions) + "}";
        }

        protected long period_value(int year, int partOfYear, string period) {
            if (string.IsNullOrEmpty(period)) period = string.Empty;

            switch (period.ToLower())
            {
                case "year":
                    return (long)year;
                case "season":
                    return long.Parse(year.ToString() + partOfYear.ToString());
                case "month":
                    return long.Parse(year.ToString() + (partOfYear < 10 ? "0" : "") + partOfYear.ToString());
            }

            return 0;
        }

        protected DBCompositeType<BigIntTableType> get_period_list(DateTime? dateFrom, DateTime? dateTo, string period, RVLang language)
        {
            DBCompositeType<BigIntTableType> param = new DBCompositeType<BigIntTableType>();

            if (!dateFrom.HasValue || !dateTo.HasValue) return param;

            GenericDate genFrom = GenericDate.fromDateTime(dateFrom.Value, language);
            GenericDate genTo = GenericDate.fromDateTime(dateTo.Value, language);

            if (string.IsNullOrEmpty(period)) period = string.Empty;

            int fullPeriodSize = 0, periodFrom = 0, periodTo = 0;

            switch (period.ToLower())
            {
                case "year":
                    fullPeriodSize = periodFrom = periodTo = 1;
                    break;
                case "season":
                    fullPeriodSize = 4;
                    periodFrom = genFrom.Month % 3 == 0 ? genFrom.Month / 3 : (int)Math.Floor(genFrom.Month / 3.0) + 1;
                    periodTo = genTo.Month % 3 == 0 ? genTo.Month / 3 : (int)Math.Floor(genTo.Month / 3.0) + 1;
                    break;
                case "month":
                    fullPeriodSize = 12;
                    periodFrom = genFrom.Month;
                    periodTo = genTo.Month;
                    break;
            }

            for (int y = genFrom.Year; y <= genTo.Year; y++)
            {
                int pFrom = y == genFrom.Year ? periodFrom : 1;
                int pTo = y == genTo.Year ? periodTo : fullPeriodSize;

                for (int p = pFrom; p <= pTo; p++)
                    param.add(new BigIntTableType(period_value(y, p, period)));
            }

            return param;
        }

        protected Dictionary<string, object> destructure_period_list(RVLang language, string period, DBCompositeType<BigIntTableType> list)
        {
            if (string.IsNullOrEmpty(period)) period = string.Empty;

            if (list == null) list = new DBCompositeType<BigIntTableType>();

            Dictionary<string, object> result = new Dictionary<string, object>();

            list.Values.Where(itm => itm.Value.HasValue)
                .Select(itm => itm.Value.Value.ToString()).Where(itm => itm.Length >= 4).ToList().ForEach(itm =>
                {
                    int year = int.Parse(itm.Substring(0, 4));
                    int datePart = itm.Length <= 4 ? 0 : int.Parse(itm.Substring(4));
                    
                    DateTime? dateFrom = null;
                    DateTime? dateTo = null;
                    
                    switch (period.ToLower())
                    {
                        case "year":
                            dateFrom = new GenericDate(language, year, month: 1, day: 1).getDateTime();
                            dateTo = new GenericDate(language, year + 1, month: 1, day: 1).getDateTime().AddDays(-1);
                            break;
                        case "season":
                            if (datePart >= 1 && datePart <= 4)
                            {
                                //'to' must point to a day after the end of the season
                                int yearTo = year + (datePart == 4 ? 1 : 0); //point to the next year if we are in the last season
                                int monthTo = datePart == 4 ? 1 : (3 * datePart) + 1; //point to the first month of the next year if we are in the last season

                                dateFrom = new GenericDate(language, year, month: (3 * datePart) - 2, day: 1).getDateTime();
                                dateTo = new GenericDate(language, yearTo, month: monthTo, day: 1).getDateTime().AddDays(-1);
                            }
                            break;
                        case "month":
                            if (datePart >= 1 && datePart <= 12)
                            {
                                //'to' must point to a day after the end of the last day of the month
                                int yearTo = year + (datePart == 12 ? 1 : 0); //point to the next year if we are in the last month
                                int monthTo = datePart == 12 ? 1 : datePart + 1; //point to the first month of the next year if we are in the last month

                                dateFrom = new GenericDate(language, year, month: datePart, day: 1).getDateTime();
                                dateTo = new GenericDate(language, yearTo, month: monthTo, day: 1).getDateTime().AddDays(-1);
                            }
                            break;
                    }

                    if (dateFrom.HasValue && dateTo.HasValue)
                        result[itm] = new Dictionary<string, object>()
                        {
                            { "From", new Dictionary<string, object>() {
                                { "Value", dateFrom.Value.ToString(format: "yyyy-MM-dd") },
                                { "Label", GenericDate.get_local_date(dateFrom.Value) }
                            } },
                            { "To", new Dictionary<string, object>() {
                                { "Value", dateTo.Value.ToString(format: "yyyy-MM-dd") },
                                { "Label", GenericDate.get_local_date(dateTo.Value) }
                            } }
                        };
                });

            return result;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}