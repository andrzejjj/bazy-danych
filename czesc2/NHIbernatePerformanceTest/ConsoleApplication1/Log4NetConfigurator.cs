using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Repository.Hierarchy;
using log4net.Appender;
using log4net.Layout;
using log4net.Core;
using log4net;

namespace ConsoleApplication1
{
    public class Log4NetConfigurator
    {
        public static void ConfigureLog4Net()
        {
            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();
            // Remove any other appenders
            hierarchy.Root.RemoveAllAppenders();
            // define some basic settings for the root
            Logger rootLogger = hierarchy.Root;
            rootLogger.Level = Level.Debug;

            // declare a RollingFileAppender with 5MB per file and max. 10 files
            RollingFileAppender appenderNH = new RollingFileAppender();
            appenderNH.Name = "RollingLogFileAppenderNHibernate";
            appenderNH.AppendToFile = true;
            appenderNH.MaximumFileSize = "5MB";
            appenderNH.MaxSizeRollBackups = 10;
            appenderNH.RollingStyle = RollingFileAppender.RollingMode.Size;
            appenderNH.StaticLogFileName = true;
            appenderNH.LockingModel = new FileAppender.MinimalLock();
            appenderNH.File = "log-nhibernate.log";
            appenderNH.Layout = new PatternLayout("%date - %message%newline");
            // this activates the FileAppender (without it, nothing would be written)
            appenderNH.ActivateOptions();

            // This is required, so that we can access the Logger by using 
            // LogManager.GetLogger("NHibernate.SQL") and it can used by NHibernate
            Logger loggerNH = hierarchy.GetLogger("NHibernate") as Logger;
            loggerNH.Level = Level.Debug;
            loggerNH.AddAppender(appenderNH);

            // this is required to tell log4net that we're done 
            // with the configuration, so the logging can start
            hierarchy.Configured = true;
        }
    }
}
