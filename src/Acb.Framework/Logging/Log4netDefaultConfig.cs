using Acb.Core;
using Acb.Core.Domain;
using Acb.Core.Timing;
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
                "#%property{LogSite}#%date#%r#%thread#%-5level#%logger#%message#%exception#%n");
        private static readonly ILayout ErrorLayout =
            new PatternLayout("#%property{LogSite}#%date#%r#%thread#%-5level#%logger#%message#%exception#%n");

        private static RollingFileAppender BaseAppender(string name, string file, ILayout layout)
        {
            return new RollingFileAppender
            {
                Name = name,
                File = $"_logs\\{Clock.Now:yyyyMM}\\",
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

        private static IAppender DebugAppender()
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
            return appender;
        }

        private static IAppender ErrorAppender()
        {
            const string file = "dd\"_error.log\"";
            var appender = BaseAppender("errorRollingFile", file, ErrorLayout);
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
