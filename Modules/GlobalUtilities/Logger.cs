using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Appender;
using log4net.Layout;

namespace RaaiVan.Modules.GlobalUtilities
{
    public enum LoggerName
    {
        Ceph
    }

    public static class Logger
    {
        private static ConcurrentDictionary<LoggerName, ILog> Loggers = new ConcurrentDictionary<LoggerName, ILog>();

        private static ILog logger(LoggerName name)
        {
            if (Loggers.ContainsKey(name)) return Loggers[name];

            //GlobalContext.Properties["CephLogFileName"] = PublicMethods.map_path("~/log_ceph.txt");
            //log4net.Config.XmlConfigurator.Configure();

            //ILog iLog = LogManager.GetLogger(name.ToString().ToLower());
            ILog iLog = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            log4net.Repository.Hierarchy.Logger l = (log4net.Repository.Hierarchy.Logger)iLog.Logger;

            l.AddAppender(create_console_appender());
            l.AddAppender(create_debug_appender());

            l.AddAppender(create_rolling_file_appender(name: name, 
                fileName: PublicMethods.map_path("~/Logs/log_" + name.ToString().ToLower() + ".txt")));

            l.Level = l.Hierarchy.LevelMap["All"];
            
            return iLog;
        }

        private static IAppender create_console_appender()
        {
            //layout
            PatternLayout layout = new PatternLayout()
            {
                ConversionPattern = "%date %level %logger - %message%newline"
            };

            layout.ActivateOptions();
            //end of layout
            

            //appender
            ConsoleAppender appender = new ConsoleAppender()
            {
                Name = "Console",
                Layout = layout
            };

            appender.ActivateOptions();
            //end of appender


            return appender;
        }

        private static IAppender create_debug_appender()
        {
            //layout
            PatternLayout layout = new PatternLayout()
            {
                ConversionPattern = "%date %level %logger - %message%newline"
            };

            layout.ActivateOptions();
            //end of layout


            //appender
            DebugAppender appender = new DebugAppender()
            {
                Name = "Debug",
                Layout = layout
            };

            appender.ActivateOptions();
            //end of appender


            return appender;
        }

        private static IAppender create_rolling_file_appender(LoggerName name, string fileName)
        {
            //layout
            PatternLayout layout = new PatternLayout()
            {
                ConversionPattern = "%date [%thread] %level %logger - %message%newline"
            };

            layout.ActivateOptions();
            //end of layout
            
            //appender
            RollingFileAppender appender = new RollingFileAppender() {
                Name = "File",
                File = fileName,
                AppendToFile = true,
                RollingStyle = RollingFileAppender.RollingMode.Size,
                MaxSizeRollBackups = 5,
                MaximumFileSize = "10MB",
                //StaticLogFileName = true,
                Layout = layout
            };

            appender.ActivateOptions();
            //end of appender


            return appender;
        }

        public static void info(LoggerName name, object info, Exception ex = null)
        {
            return;

            ILog iLog = logger(name);

            //FileAppender appender = iLog.Logger.Repository.GetAppenders().OfType<FileAppender>().FirstOrDefault();

            iLog.Info(info, ex);
        }
    }
}
