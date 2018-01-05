using Acb.Core.Helper;
using Acb.Core.Logging;
using Acb.Core.Timing;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Filter;
using log4net.Layout;
using log4net.Repository;
using System.IO;

namespace Acb.Framework.Logging
{
    public class Log4NetAdapter : LoggerAdapterBase
    {
        private const string FileName = "log4net.config";

        private static string ConfigPath => ConfigHelper.Instance.Get(string.Empty);
        private static string LogSite => ConfigHelper.Instance.Get("local");

        private static readonly ILoggerRepository Repository = log4net.LogManager.CreateRepository(LogSite);

        /// <summary>
        /// 初始化一个<see cref="Log4NetAdapter"/>类型的新实例
        /// </summary>k
        public Log4NetAdapter()
        {
            var configFile = Path.Combine(ConfigPath, FileName);
            log4net.GlobalContext.Properties["LogSite"] = LogSite;

            if (File.Exists(configFile))
            {
                XmlConfigurator.ConfigureAndWatch(Repository, new FileInfo(configFile));
                return;
            }
            var appender = new RollingFileAppender
            {
                Name = "root",
                File = $"_logs\\{LogSite}\\{Clock.Now:yyyyMM}\\",
                AppendToFile = true,
                LockingModel = new FileAppender.MinimalLock(),
                RollingStyle = RollingFileAppender.RollingMode.Date,
                DatePattern = "dd\".log\"",
                StaticLogFileName = false,
                MaxSizeRollBackups = 100,
                MaximumFileSize = "2MB",
                Layout = new PatternLayout("[%d{yyyy-MM-dd HH:mm:ss.fff}] %-5p %c %t %w %n%m%n")
                //Layout = new PatternLayout("[%d [%t] %-5p %c [%x] - %m%n]")
            };
            appender.ClearFilters();
            appender.AddFilter(new LevelRangeFilter
            {
                LevelMin = Level.Debug,
                LevelMax = Level.Fatal
            });

            BasicConfigurator.Configure(Repository, appender);
            appender.ActivateOptions();
        }

        protected override ILog CreateLogger(string name)
        {
            var log = log4net.LogManager.GetLogger(LogSite, name);
            return new Log4NetLog(log);
        }
    }
}
