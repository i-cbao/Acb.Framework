using Acb.Core.Extensions;
using Acb.Core.Logging;
using log4net.Config;
using log4net.Repository;

namespace Acb.Framework.Logging
{
    public class Log4NetAdapter : LoggerAdapterBase
    {
        internal static string LogSite => "site".Config("local");

        internal static ILoggerRepository Repository => log4net.LogManager.CreateRepository(LogSite);
        /// <inheritdoc />
        /// <summary>
        /// 初始化一个<see cref="T:Acb.Framework.Logging.Log4NetAdapter" />类型的新实例
        /// </summary>k
        public Log4NetAdapter()
        {
            log4net.GlobalContext.Properties["LogSite"] = LogSite;

            var appenders = Log4NetDefaultConfig.Appenders;

            BasicConfigurator.Configure(Repository, appenders);
        }

        protected override ILog CreateLogger(string name)
        {
            var log = log4net.LogManager.GetLogger(LogSite, name);
            return new Log4NetLog(log);
        }
    }
}
