using Acb.Core.Extensions;
using Acb.Core.Logging;
using Acb.Core.Serialize;
using Acb.Core.Timing;
using System;
using System.Collections.Generic;

namespace Acb.Framework.Logging
{
    public class ConsoleLog : LogBase
    {
        private readonly Dictionary<LogLevel, ConsoleColor> _logColors = new Dictionary<LogLevel, ConsoleColor>
        {
            {LogLevel.Trace, ConsoleColor.Green},
            {LogLevel.Debug, ConsoleColor.DarkGreen},
            {LogLevel.Info, ConsoleColor.Cyan},
            {LogLevel.Warn, ConsoleColor.Yellow},
            {LogLevel.Error, ConsoleColor.Red},
            {LogLevel.Fatal, ConsoleColor.Magenta}
        };

        /// <summary> 写日志方法 </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        protected override void WriteInternal(LogLevel level, object message, Exception exception)
        {
            var prints = new List<PrintItem>();
            ConsoleColor? color = null;
            if (_logColors.ContainsKey(level))
            {
                color = _logColors[level];
            }

            prints.Add(new PrintItem($"[{level}]\t", color, false));
            prints.Add(new PrintItem($"{Clock.Now:yyyy-MM-dd HH:mm:ss}\t{LoggerName}", ConsoleColor.DarkCyan));
            if (message != null)
            {
                var content = message.GetType().IsSimpleType()
                    ? message.ToString()
                    : JsonHelper.ToJson(message, NamingType.CamelCase, true);
                prints.Add(new PrintItem($"\t{content}"));
            }

            if (exception != null)
            {
                prints.Add(new PrintItem($"\t{exception.Format()}", ConsoleColor.Red));
            }

            prints.Print();
        }

        public override bool IsTraceEnabled => true;
        public override bool IsDebugEnabled => true;
        public override bool IsInfoEnabled => true;
        public override bool IsWarnEnabled => true;
        public override bool IsErrorEnabled => true;
        public override bool IsFatalEnabled => true;
    }

    /// <summary> 控制台日志适配器 </summary>
    public class ConsoleAdapter : LoggerAdapterBase
    {
        /// <summary> 创建日志 </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override ILog CreateLogger(string name)
        {
            return new ConsoleLog();
        }
    }
}
