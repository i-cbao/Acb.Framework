using Acb.Core.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Acb.Core.Logging
{
    /// <summary> 日志管理器 </summary>
    public static class LogManager
    {
        private static readonly ConcurrentDictionary<string, Logger> LoggerDictionary;
        //private const string MessageFormat = "Method:{1}({2}) {3}:{4}{0}Msg:{5}{0}";
        private static readonly object LockObj = new object();

        /// <summary> 日志适配器集合 </summary>
        private static readonly ConcurrentDictionary<ILoggerAdapter, LogLevel> LoggerAdapters;

        private static LogLevel _logLevel;
        private static readonly bool CanSetLevel = true;

        /// <summary> 静态构造 </summary>
        static LogManager()
        {
            LoggerDictionary = new ConcurrentDictionary<string, Logger>();
            LoggerAdapters = new ConcurrentDictionary<ILoggerAdapter, LogLevel>();
            var level = "LogLevel".Config(string.Empty);
            if (string.IsNullOrWhiteSpace(level))
                return;
            _logLevel = level.CastTo(LogLevel.Off);
            CanSetLevel = false;
        }

        /// <summary> 是否启用日志级别 </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private static bool IsEnableLevel(LogLevel level)
        {
            return level >= _logLevel;
        }

        /// <summary> 添加适配器 </summary>
        /// <param name="adapter">适配器</param>
        public static void AddAdapter(ILoggerAdapter adapter)
        {
            if (LoggerAdapters.ContainsKey(adapter))
            {
                return;
            }
            LoggerAdapters.TryAdd(adapter, LogLevel.All);
        }

        /// <summary> 添加适配器 </summary>
        /// <param name="adapter">适配器</param>
        /// <param name="level">日志等级(可多位与)</param>
        public static void AddAdapter(ILoggerAdapter adapter, LogLevel level)
        {
            if (LoggerAdapters.ContainsKey(adapter))
            {
                LoggerAdapters[adapter] = level;
                return;
            }
            LoggerAdapters.TryAdd(adapter, level);
        }

        /// <summary> 移除适配器 </summary>
        /// <param name="adapter"></param>
        public static void RemoveAdapter(ILoggerAdapter adapter)
        {
            RemoveAdapter(adapter.GetType());
        }

        /// <summary> 移除适配器 </summary>
        /// <param name="adapterType"></param>
        public static void RemoveAdapter(Type adapterType)
        {
            var adapters = LoggerAdapters.Where(t => t.Key.GetType() == adapterType).ToList();
            if (!adapters.Any())
                return;
            LogLevel level;
            adapters.Foreach(t => LoggerAdapters.TryRemove(t.Key, out level));
        }

        /// <summary> 清空适配器 </summary>
        public static void ClearAdapter()
        {
            LoggerAdapters?.Clear();
        }

        /// <summary>
        /// 获取日志记录实例
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Logger Logger(string name)
        {
            Logger logger;
            if (LoggerDictionary.TryGetValue(name, out logger))
            {
                return logger;
            }
            logger = new Logger(name);
            LoggerDictionary.TryAdd(name, logger);
            return logger;
        }

        /// <summary>
        /// 获取日志记录实例
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Logger Logger(Type type)
        {
            return Logger(type.FullName);
        }

        /// <summary>
        /// 获取日志记录实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Logger Logger<T>()
        {
            return Logger(typeof(T));
        }

        /// <summary> 设置日志等级 </summary>
        /// <param name="level"></param>
        public static void SetLevel(LogLevel level)
        {
            if (!CanSetLevel)
                return;
            _logLevel = level;
        }

        internal static IEnumerable<ILog> GetAdapters(string name, LogLevel level)
        {
            return LoggerAdapters.Where(t => (t.Value & level) > 0).Select(t => t.Key.GetLogger(name));
        }

        internal static void EachAdapter(this string loggerName, LogLevel level, Action<ILog> action)
        {
            if (string.IsNullOrWhiteSpace(loggerName) || !IsEnableLevel(level))
                return;
            foreach (var log in GetAdapters(loggerName, level))
            {
                log.LoggerName = loggerName;
                action(log);
            }
        }
    }
}
