﻿using Acb.Core.Extensions;
using Acb.Core.Logging;
using log4net.Core;
using System;
using ILogger = log4net.Core.ILogger;

namespace Acb.Framework.Logging
{
    public class Log4NetLog : LogBase
    {
        private readonly ILogger _logger;
        public Log4NetLog(ILoggerWrapper wrapper)
        {
            _logger = wrapper.Logger;
        }

        protected override void WriteInternal(LogLevel level, object message, Exception exception)
        {
            if (message == null)
                message = "NULL";
            else
            {
                var type = message.GetType();
                if (type.IsSimpleType())
                    message = message.ToString();
            }

            _logger.Log(typeof(Log4NetLog), ParseLevel(level), message, exception);
        }

        public override bool IsTraceEnabled => _logger.IsEnabledFor(Level.Trace);

        public override bool IsDebugEnabled => _logger.IsEnabledFor(Level.Debug);

        public override bool IsInfoEnabled => _logger.IsEnabledFor(Level.Info);

        public override bool IsWarnEnabled => _logger.IsEnabledFor(Level.Warn);

        public override bool IsErrorEnabled => _logger.IsEnabledFor(Level.Error);

        public override bool IsFatalEnabled => _logger.IsEnabledFor(Level.Fatal);

        /// <summary> 日志等级转换 </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static Level ParseLevel(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.All:
                    return Level.All;
                case LogLevel.Trace:
                    return Level.Trace;
                case LogLevel.Debug:
                    return Level.Debug;
                case LogLevel.Info:
                    return Level.Info;
                case LogLevel.Warn:
                    return Level.Warn;
                case LogLevel.Error:
                    return Level.Error;
                case LogLevel.Fatal:
                    return Level.Fatal;
                case LogLevel.Off:
                    return Level.Off;
                default:
                    return Level.Off;
            }
        }
    }
}
