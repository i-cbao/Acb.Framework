using Acb.Core.Logging;
using log4net;
using log4net.Appender;
using log4net.Core;

namespace Acb.Framework.Logging
{
    internal class TcpAppender : AppenderSkeleton
    {
        private readonly IRemoteLogger _logger;

        public TcpAppender(IRemoteLogger logger)
        {
            _logger = logger;
        }

        protected override bool RequiresLayout => false;

        protected override void Append(LoggingEvent loggingEvent)
        {
            var site = GlobalContext.Properties["LogSite"]?.ToString();
            _logger.Logger(loggingEvent.MessageObject, ParseLevel(loggingEvent.Level), loggingEvent.ExceptionObject,
                loggingEvent.TimeStamp, loggingEvent.LoggerName, site);
        }

        /// <summary> 日志等级转换 </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static LogLevel ParseLevel(Level level)
        {
            if (level == Level.All)
                return LogLevel.All;
            if (level == Level.Trace)
                return LogLevel.Trace;
            if (level == Level.Debug)
                return LogLevel.Debug;
            if (level == Level.Info)
                return LogLevel.Info;
            if (level == Level.Warn)
                return LogLevel.Warn;
            if (level == Level.Error)
                return LogLevel.Error;
            if (level == Level.Fatal)
                return LogLevel.Fatal;
            return LogLevel.Info;
        }
    }

}
