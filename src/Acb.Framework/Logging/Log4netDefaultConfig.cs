using Acb.Core.Timing;
using log4net.Appender;
using log4net.Core;
using log4net.Filter;
using log4net.Layout;

namespace Acb.Framework.Logging
{
    internal class Log4NetDefaultConfig
    {
        private static RollingFileAppender BaseAppender(string name, string file, ILayout layout)
        {
            return new RollingFileAppender
            {
                Name = name,
                File = file,
                AppendToFile = true,
                LockingModel = new FileAppender.MinimalLock(),
                RollingStyle = RollingFileAppender.RollingMode.Date,
                DatePattern = "dd\".log\"",
                StaticLogFileName = false,
                MaxSizeRollBackups = 100,
                MaximumFileSize = "2MB",
                Layout = layout
            };
        }

        private static IAppender DebugAppender()
        {
            var layout = new PatternLayout("[%date] [%thread] %-5level %{LogSite} %logger %method [%message%exception]%n");
            var file = $"_logs\\debug_{Clock.Now:yyyyMM}\\";
            var appender = BaseAppender("rollingFile", file, layout);
            appender.ClearFilters();
            appender.AddFilter(new LevelRangeFilter
            {
                LevelMin = Level.Debug,
                LevelMax = Level.Warn
            });
            return appender;
        }

        private static IAppender ErrorAppender()
        {
            var layout = new PatternLayout("[%date] [%thread] %-5level %{LogSite} %logger %l [%message%n%exception]%n");
            var file = $"_logs\\error_{Clock.Now:yyyyMM}\\";
            var appender = BaseAppender("errorRollingFile", file, layout);
            appender.ClearFilters();
            appender.AddFilter(new LevelRangeFilter
            {
                LevelMin = Level.Error,
                LevelMax = Level.Fatal
            });
            return appender;
        }

        public static IAppender[] Appenders => new[] { DebugAppender(), ErrorAppender() };
    }
}
