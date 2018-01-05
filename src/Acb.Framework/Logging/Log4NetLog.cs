using log4net.Core;
using Acb.Core.Extensions;
using Acb.Core.Logging;
using Acb.Core.Serialize;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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

        private static LogInfo Format(string msg, Exception ex = null)
        {
            var result = new LogInfo
            {
                Message = msg,
                File = string.Empty,
                Method = string.Empty,
                Detail = string.Empty
            };
            if (ex != null)
            {
                ex = ex.GetBaseException();
                result.Detail = ex.Format();
            }
            var fileFrames = (new StackTrace(true).GetFrames() ?? new StackFrame[] { }).ToList();
            //Console.WriteLine($"count:{fileFrames.Count}");
            if (fileFrames.Any())
            {
                var index =
                    fileFrames.FindIndex(t =>
                    {
                        var declaringType = t.GetMethod().DeclaringType;
                        return declaringType != null && declaringType.FullName == "Acb.Core.Logging.Logger";
                    });
                if (index < 0) index = fileFrames.Count - 1;
                else if (index < fileFrames.Count - 1)
                    index++;
                //Console.WriteLine($"index:{index}");
                var frame = fileFrames[index];
                if (!string.IsNullOrWhiteSpace(frame.GetFileName()))
                {
                    result.File =
                        $"{frame.GetFileName()}:line({frame.GetFileLineNumber()},{frame.GetFileColumnNumber()})";
                }
                var method = frame.GetMethod();
                if (method.DeclaringType != null && method.DeclaringType.MemberType == MemberTypes.NestedType)
                    method = fileFrames.Last().GetMethod();
                result.Method = $"{method.DeclaringType}:{method.Name}";
            }
            //Console.WriteLine(JsonHelper.ToJson(result));
            result.SiteName = (log4net.GlobalContext.Properties["LogSite"] ?? string.Empty).ToString();
            return result;
        }

        protected override void WriteInternal(LogLevel level, object message, Exception exception)
        {
            string str;
            if (message == null)
                str = "NULL";
            else
            {
                var type = message.GetType();
                if (type.IsValueType || type == typeof(string))
                    str = message.ToString();
                else
                    str = JsonHelper.ToJson(message);
            }
            _logger.Log(typeof(Log4NetLog), ParseLevel(level), Format(str, exception),
                exception);
        }

        public override bool IsTraceEnabled => _logger.IsEnabledFor(Level.Trace);

        public override bool IsDebugEnabled => _logger.IsEnabledFor(Level.Debug);

        public override bool IsInfoEnabled => _logger.IsEnabledFor(Level.Info);

        public override bool IsWarnEnabled => _logger.IsEnabledFor(Level.Warn);

        public override bool IsErrorEnabled => _logger.IsEnabledFor(Level.Error);

        public override bool IsFatalEnabled => _logger.IsEnabledFor(Level.Fatal);

        private static Level ParseLevel(LogLevel level)
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
