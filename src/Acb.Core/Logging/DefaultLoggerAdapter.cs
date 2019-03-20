using Acb.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;

namespace Acb.Core.Logging
{
    /// <summary> 系统默认日志 </summary>
    internal class DefaultLogger : LogBase
    {
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        public DefaultLogger(Microsoft.Extensions.Logging.ILogger logger)
        {
            _logger = logger;
        }

        private static Microsoft.Extensions.Logging.LogLevel Convert(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Trace:
                    return Microsoft.Extensions.Logging.LogLevel.Trace;
                case LogLevel.Debug:
                    return Microsoft.Extensions.Logging.LogLevel.Debug;
                case LogLevel.Info:
                    return Microsoft.Extensions.Logging.LogLevel.Information;
                case LogLevel.Warn:
                    return Microsoft.Extensions.Logging.LogLevel.Warning;
                case LogLevel.Error:
                    return Microsoft.Extensions.Logging.LogLevel.Error;
                case LogLevel.Fatal:
                    return Microsoft.Extensions.Logging.LogLevel.Critical;
            }

            return Microsoft.Extensions.Logging.LogLevel.None;
        }
        protected override void WriteInternal(LogLevel level, object message, Exception exception)
        {
            var msg = message.GetType().IsSimpleType() ? message.ToString() : JsonConvert.SerializeObject(message);

            _logger.Log(Convert(level), exception, msg);
        }

        public override bool IsTraceEnabled => _logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Trace);
        public override bool IsDebugEnabled => _logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Debug);
        public override bool IsInfoEnabled => _logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Information);
        public override bool IsWarnEnabled => _logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Warning);
        public override bool IsErrorEnabled => _logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Error);
        public override bool IsFatalEnabled => _logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Critical);
    }

    public class DefaultLoggerAdapter : LoggerAdapterBase
    {
        private readonly IServiceProvider _provider;
        public DefaultLoggerAdapter(IServiceProvider provider)
        {
            _provider = provider;
        }

        protected override ILog CreateLogger(string name)
        {
            var loggerFactory = _provider.GetService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger(name);
            return new DefaultLogger(logger);
        }
    }
}
