using Acb.Core;
using Acb.Core.Dependency;
using Acb.Core.Domain;
using Acb.Core.Logging;
using Acb.Core.Logging.Remote;
using log4net.Appender;
using log4net.Core;
using log4net.Filter;
using log4net.Layout;

namespace Acb.Framework.Logging
{
    internal class Log4NetDefaultConfig
    {
        private static readonly ILayout NormalLayout =
            new PatternLayout(
                "%date [%-5level] [%property{LogSite}] %r %thread %logger %message %exception%n");

        private static readonly ILayout ErrorLayout =
            new PatternLayout("%date [%-5level] [%property{LogSite}] %r %thread %logger %message %n%exception%n");

        private static RollingFileAppender BaseAppender(string name, string file, ILayout layout)
        {
            return new RollingFileAppender
            {
                Name = name,
                File = "_logs\\",
                AppendToFile = true,
                LockingModel = new FileAppender.MinimalLock(),
                RollingStyle = RollingFileAppender.RollingMode.Date,
                DatePattern = file,
                StaticLogFileName = false,
                MaxSizeRollBackups = 100,
                MaximumFileSize = "2MB",
                Layout = layout
            };
        }

        internal static IAppender DebugAppender()
        {
            const string file = "yyyyMM\\\\dd'.log'";
            var appender = BaseAppender("rollingFile", file, NormalLayout);
            appender.ClearFilters();

            var minLevel = Consts.Mode == ProductMode.Prod ? Level.Info : Level.Debug;
            appender.AddFilter(new LevelRangeFilter
            {
                LevelMin = minLevel,
                LevelMax = Level.Warn
            });
            appender.ActivateOptions();
            return appender;
        }

        internal static IAppender ErrorAppender()
        {
            const string file = "yyyyMM\\\\dd'_error.log'";
            var appender = BaseAppender("errorRollingFile", file, ErrorLayout);
            appender.AddFilter(new LevelRangeFilter
            {
                LevelMin = Level.Error,
                LevelMax = Level.Fatal
            });
            appender.ActivateOptions();
            return appender;
        }

        internal static IAppender TcpAppender()
        {
            var logger = CurrentIocManager.Resolve<IRemoteLogger>();
            var tcpAppender = new TcpAppender(logger);
            var tcp = RemoteLoggerConfig.Config();
            var level = Log4NetLog.ParseLevel(tcp?.Level ?? LogLevel.Error);
            tcpAppender.AddFilter(new LevelRangeFilter
            {
                LevelMin = level
            });
            tcpAppender.ActivateOptions();
            return tcpAppender;
        }

        public static IAppender[] Appenders => new[] { DebugAppender(), ErrorAppender() };
    }
}
