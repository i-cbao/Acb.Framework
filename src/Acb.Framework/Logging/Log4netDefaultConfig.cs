using Acb.Core;
using Acb.Core.Domain;
using Acb.Core.Extensions;
using Acb.Core.Timing;
using log4net.Appender;
using log4net.Core;
using log4net.Filter;
using log4net.Layout;
using System.Net;
using System.Text;

namespace Acb.Framework.Logging
{
    internal class Log4NetDefaultConfig
    {
        private const string TcpLoggerConfigName = "tcpLogger";

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
                File = $"_logs/{Clock.Now:yyyyMM}/",
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
            const string file = "dd\".log\"";
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
            const string file = "dd\"_error.log\"";
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
            var tcp = TcpLoggerConfigName.Config<TcpLoggerConfig>();
            if (tcp == null || string.IsNullOrWhiteSpace(tcp.Address) || tcp.Port <= 0)
                return null;
            var tcpAppender = new TcpAppender
            {
                Encoding = Encoding.UTF8,
                RemoteAddress = IPAddress.Parse(tcp.Address),
                RemotePort = tcp.Port,
                Layout = string.IsNullOrWhiteSpace(tcp.Layout) ? ErrorLayout : new PatternLayout(tcp.Layout)
            };
            var level = Log4NetLog.ParseLevel(tcp.Level);
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
