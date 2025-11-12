using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Threading;
using RaaiVan.Modules.GlobalUtilities;

namespace RaaiVan.Modules.Log
{
    public class LogController
    {
        private static string GetFullyQualifiedName(string name)
        {
            return "[dbo]." + "[LG_" + name + "]"; //'[dbo].' is database owner and 'LG_' is module qualifier
        }

        private static bool _save_log(Guid? applicationId, Log info)
        {
            if (!info.UserID.HasValue) info.UserID = Guid.Empty;
            if (!info.Action.HasValue || info.Action == Action.None) return false;
            if (!info.Date.HasValue) info.Date = DateTime.Now;
            if (string.IsNullOrEmpty(info.Info)) info.Info = null;

            LogLevel level = LevelOfTheLog.get(info.Action.Value);
            string strLevel = level == LogLevel.None ? null : level.ToString();

            if (info.SubjectID.HasValue) info.SubjectIDs.Add(info.SubjectID.Value);
            else if (!info.SubjectID.HasValue && info.SubjectIDs.Count == 0) info.SubjectIDs.Add(Guid.Empty);

            info.NotAuthorized = info.Action.ToString().IndexOf('_') > 0 ||
                info.Action == Action.NotAuthorizedAnonymousRequest ||
                info.Action == Action.PotentialCSRFAttack || info.Action == Action.PotentialReplayAttack;

            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SaveLog"),
                applicationId, info.UserID, info.HostAddress, info.HostName, info.Action.ToString(), strLevel, info.NotAuthorized,
                ProviderUtil.list_to_string<Guid>(info.SubjectIDs), ',', info.SecondSubjectID, info.ThirdSubjectID, info.FourthSubjectID,
                info.Date, info.Info, (info.ModuleIdentifier.HasValue ? info.ModuleIdentifier.ToString() : string.Empty));
        }

        public static void save_log(Guid? applicationId, Log info)
        {
            PublicMethods.set_timeout(() => _save_log(applicationId, info));
        }

        public static List<Log> get_logs(Guid? applicationId, List<Guid> userIds, List<Action> actions, 
            DateTime? beginDate = null, DateTime? finishDate = null, long? lastId = null, int? count = null)
        {
            return LGParsers.logs(DBConnector.read(applicationId, GetFullyQualifiedName("GetLogs"),
                applicationId, ProviderUtil.list_to_string<Guid>(userIds), 
                ProviderUtil.list_to_string<Action>(actions), ',', beginDate, finishDate, lastId, count));
        }

        public static bool save_error_log(Guid? applicationId, Guid? userId, string subject, 
            string description, ModuleIdentifier? moduleIdentifier, LogLevel level = LogLevel.None)
        {
            string strLevel = level == LogLevel.None ? LogLevel.Debug.ToString() : level.ToString();

            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SaveErrorLog"),
                applicationId, userId, subject, description, DateTime.Now,
                (moduleIdentifier.HasValue ? moduleIdentifier.ToString() : string.Empty), strLevel);
        }

        public static bool save_error_log(Guid? applicationId, Guid? userId, string subject,
            Exception exception, ModuleIdentifier? moduleIdentifier, LogLevel level = LogLevel.None)
        {
            if (exception != null && !string.IsNullOrEmpty(exception.Message) && exception.Message.ToLower() == "thread was being aborted.")
                return true; //page redirect throws this error and there is no need to be logged

            string description = PublicMethods.get_exception(exception);

            return save_error_log(applicationId, userId, subject, description, moduleIdentifier, level);
        }

        public static bool save_raw_log(Guid? applicationId, Guid? userId, Dictionary<string, object> info)
        {
            //Check if data is a valid json
            return info != null && info.Keys.Count > 0 && 
                DBConnector.succeed(applicationId, GetFullyQualifiedName("SaveRawLog"),
                applicationId, userId, DateTime.Now, PublicMethods.toJSON(info));
        }
    }
}
