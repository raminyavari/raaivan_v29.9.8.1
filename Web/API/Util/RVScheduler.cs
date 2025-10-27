using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.Diagnostics;
using RaaiVan.Modules.Documents;
using RaaiVan.Modules.Search;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.Jobs;
using RaaiVan.Modules.Log;

namespace RaaiVan.Web.API
{
    class RVScheduler
    {
        private static Dictionary<string, RVJob> jobsDic = new Dictionary<string, RVJob>();

        private static bool _inited = false;

        private static void _initialize()
        {
            if (_inited || RaaiVanSettings.SAASBasedMultiTenancy) return;
            _inited = true;

            List<ITenant> tenants = RaaiVanSettings.Tenants;

            if (RaaiVanSettings.DailyDatabaseBackup)
                jobsDic["DailyDatabaseBackup"] = new RVJob(Guid.NewGuid());

            foreach (ITenant tnt in tenants)
            {
                string postFix = "_" + tnt.Id.ToString();

                if (RaaiVanSettings.FileContentExtraction.FileContents(tnt.Id))
                    jobsDic["FileContents" + postFix] = new RVJob(tnt.Id);
                if (RaaiVanSettings.IndexUpdate.Index(tnt.Id))
                    jobsDic["Index" + postFix] = new RVJob(tnt.Id);
                if (RaaiVanSettings.EmailQueue.EnableEmailQueue(tnt.Id))
                    jobsDic["EmailQueue" + postFix] = new RVJob(tnt.Id);
                if (Modules.RaaiVanConfig.Modules.Knowledge(tnt.Id))
                    jobsDic["CheckExpiredKnowledges" + postFix] = new RVJob(tnt.Id);
                jobsDic["RemoveTemporaryFiles" + postFix] = new RVJob(tnt.Id);

                List<string> otherLists = RaaiVanSettings.Jobs.JobsList;

                foreach (string str in otherLists)
                {
                    string[] arr = str.Split(',');

                    string jobName = arr.Length > 0 ? arr[0].Trim() : string.Empty;
                    if (string.IsNullOrEmpty(jobName)) continue;

                    RVJob trd = new RVJob(tnt.Id);

                    int interval = 0;
                    if (arr.Length < 2 || !int.TryParse(arr[1], out interval) || interval <= 0) trd.Interval = 86400000; //equal to one day
                    else trd.Interval = interval;

                    if (arr.Length > 2)
                    {
                        string[] t = arr[2].Split(':');
                        int n = 0;
                        if (t.Length >= 2 && int.TryParse(t[0], out n) && n >= 0 && n <= 23 && int.TryParse(t[1], out n) && n >= 0 && n <= 59)
                            trd.StartTime = new DateTime(2000, 1, 1, int.Parse(t[0]), int.Parse(t[1]), 0, 0);
                    }
                    if (!trd.StartTime.HasValue) trd.StartTime = new DateTime(2000, 1, 1, 0, 0, 0, 0);

                    if (arr.Length > 3)
                    {
                        string[] t = arr[3].Split(':');
                        int n = 0;
                        if (t.Length >= 2 && int.TryParse(t[0], out n) && n >= 0 && n <= 23 && int.TryParse(t[1], out n) && n >= 0 && n <= 59)
                            trd.EndTime = new DateTime(2000, 1, 1, int.Parse(t[0]), int.Parse(t[1]), 59, 999);
                    }
                    if (!trd.EndTime.HasValue) trd.EndTime = new DateTime(2000, 1, 1, 0, 0, 59, 999);

                    jobsDic["_" + jobName + postFix] = trd;
                }
            }
        }

        private static void _start_job(string jobName, RVJob jobObject)
        {
            if (!jobObject.TenantID.HasValue) return;

            while (true)
            {
                //sleep thread be madate Interval saniye
                if (jobObject.LastActivityDate.HasValue || !string.IsNullOrEmpty(jobObject.ErrorMessage))
                    Thread.Sleep(jobObject.Interval.Value);

                //agar dar saati hastim ke bayad update shavad edame midahim
                if (!jobObject.check_time()) continue;

                Stopwatch sw = Stopwatch.StartNew();
                sw.Start();

                try
                {
                    Jobs.run(jobObject.TenantID.Value, jobName);
                    jobObject.LastActivityDate = DateTime.Now;
                }
                catch (Exception ex)
                {
                    LogController.save_error_log(jobObject.TenantID.Value, null, "JobFailed: " + jobName, ex, ModuleIdentifier.RV, LogLevel.Fatal);
                    jobObject.ErrorMessage = PublicMethods.get_exception(ex);
                }

                sw.Stop();
                jobObject.LastActivityDuration = sw.ElapsedMilliseconds;
            }
        }

        public static void start(string jobName, int? interval = null, DateTime? startTime = null, DateTime? endTime = null)
        {
            if (RaaiVanSettings.SAASBasedMultiTenancy) return;

            _initialize();

            RVJob trd = jobsDic.ContainsKey(jobName) && jobsDic[jobName] != null ? jobsDic[jobName] : null;
            if (trd == null) return;
            if (trd.ThreadObject != null) trd.ThreadObject.Abort();
            
            if (interval.HasValue) trd.Interval = interval;
            if (startTime.HasValue) trd.StartTime = startTime;
            if (endTime.HasValue) trd.EndTime = endTime;

            if (jobName.LastIndexOf('_') > 0) jobName = jobName.Substring(0, jobName.LastIndexOf('_'));
            
            switch (jobName)
            {
                case "FileContents":
                    trd.ThreadObject = new Thread(ExtractFileContents.start_extract);
                    trd.ThreadObject.Start(trd);
                    break;
                case "Index":
                    trd.ThreadObject = new Thread(SearchUtilities.start_update);
                    trd.ThreadObject.Start(trd);
                    break;
                case "EmailQueue":
                    trd.ThreadObject = new Thread(EmailQueue.start_sending);
                    trd.ThreadObject.Start(trd);
                    break;
                case "DailyDatabaseBackup":
                    trd.ThreadObject = new Thread(PublicMethods.start_db_backup);
                    trd.ThreadObject.Start(trd);
                    break;
                case "CheckExpiredKnowledges":
                    trd.ThreadObject = new Thread((new KnowledgeAPI()).check_expired_knowledges);
                    trd.ThreadObject.Start(trd);
                    break;
                case "RemoveTemporaryFiles":
                    trd.ThreadObject = new Thread((new DocsAPI()).start_remove_temporary_files);
                    trd.ThreadObject.Start(trd);
                    break;
                default:
                    PublicMethods.set_timeout(() => _start_job(jobName.Substring(1), trd), 0);
                    break;
            }
            
            //Save Log
            LogController.save_log(trd.TenantID.Value, new Log()
            {
                Action = Modules.Log.Action.JobStarted,
                Info = "{\"JobName\":\"" + jobName + "\"}",
                ModuleIdentifier = ModuleIdentifier.RV
            });
            //end of Save Log

            jobsDic[jobName] = trd;
        }

        public static void run_jobs()
        {
            if (RaaiVanSettings.SAASBasedMultiTenancy) return;

            _initialize();

            List<string> keys = new List<string>(jobsDic.Keys);
            
            foreach (string jobName in keys) start(jobName);
        }

        public static void stop(string jobName)
        {
            if (!jobsDic.ContainsKey(jobName) || jobsDic[jobName] == null || RaaiVanSettings.SAASBasedMultiTenancy) return;

            jobsDic[jobName].ThreadObject.Abort();
            jobsDic.Remove(jobName);
        }

        public static void set_timing(string jobName, HttpContext context, 
            int? interval, DateTime? startTime, DateTime? endTime)
        {
            _initialize();

            RVJob trd = jobsDic.ContainsKey(jobName) && jobsDic[jobName] != null ? jobsDic[jobName] : null;
            if (trd == null) return;

            if (trd.ThreadObject == null)
                start(jobName, interval, startTime, endTime);
            else
            {
                if (interval.HasValue) trd.Interval = interval;
                if (startTime.HasValue) trd.StartTime = startTime;
                if (endTime.HasValue) trd.EndTime = endTime;
            }
        }

        public static RVJob get_job(string jobName)
        {
            _initialize();
            return jobsDic.ContainsKey(jobName) && jobsDic[jobName] != null ? jobsDic[jobName] : null;
        }

        public static Dictionary<string, RVJob> get_list()
        {
            _initialize();
            return jobsDic;
        }
    }
}