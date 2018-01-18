using Acb.Core.Extensions;
using Acb.Core.Helper;
using Acb.Core.Logging;
using log4net.Appender;
using log4net.Config;
using log4net.Repository;
using System.IO;

namespace Acb.Framework.Logging
{
    public class Log4NetAdapter : LoggerAdapterBase
    {
        private const string FileName = "log4net.config";

        private static string ConfigPath => ConfigHelper.Instance.Get(string.Empty);
        private static string LogSite => "site".Config("local");

        private static ILoggerRepository Repository => log4net.LogManager.CreateRepository(LogSite);

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

            var appenders = Log4NetDefaultConfig.Appenders;

            BasicConfigurator.Configure(Repository, appenders);

            appenders.Foreach(t =>
            {
                if (t is RollingFileAppender appender)
                    appender.ActivateOptions();
            });
        }

        protected override ILog CreateLogger(string name)
        {
            var log = log4net.LogManager.GetLogger(LogSite, name);
            return new Log4NetLog(log);
        }
    }
}
