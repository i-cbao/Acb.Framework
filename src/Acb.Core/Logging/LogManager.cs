using Acb.Core.Dependency;
using Acb.Core.Extensions;
using Acb.Core.Helper;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Acb.Core.Logging
{
    /// <summary> 日志管理器 </summary>
    public static class LogManager
    {
        private const string ConfigLevel = "logLevel";
        private const string ConfigLevelEnvironmentName = "ACB_LOGLEVEL";

        private static readonly ConcurrentDictionary<string, Logger> LoggerDictionary;

        /// <summary> 日志适配器集合 </summary>
        private static readonly ConcurrentDictionary<ILoggerAdapter, LogLevel> LoggerAdapters;

        private static LogLevel _logLevel;

        /// <summary> 静态构造 </summary>
        static LogManager()
        {
            LoggerDictionary = new ConcurrentDictionary<string, Logger>();
            LoggerAdapters = new ConcurrentDictionary<ILoggerAdapter, LogLevel>();
            LogLevel();
        }

        /// <summary> 设置日志登记 </summary>
        /// <param name="level">为空时重置</param>
        public static void LogLevel(LogLevel? level = null)
        {
            LogLevel logLevel;
            if (level.HasValue)
                logLevel = level.Value;
            else
            {
                var mode = ConfigLevelEnvironmentName.Env();
                if (string.IsNullOrWhiteSpace(mode))
                    mode = ConfigLevel.Config<string>();
                logLevel = mode.CastTo(Logging.LogLevel.Info);
            }
            if (_logLevel == logLevel) return;
            _logLevel = logLevel;
            foreach (var adapter in LoggerAdapters)
            {
                //if (adapter.Value == Logging.LogLevel.Off || !IsEnableLevel(adapter.Value))
                LoggerAdapters[adapter.Key] = _logLevel;
            }
        }

        /// <summary> 是否启用日志级别 </summary>
        /// <param name="level">日志等级</param>
        /// <param name="targetLevel">当前日志等级</param>
        /// <returns></returns>
        private static bool IsEnableLevel(LogLevel level, LogLevel? targetLevel = null)
        {
            targetLevel = targetLevel ?? _logLevel;
            if (targetLevel == Logging.LogLevel.All) return true;
            if (targetLevel == Logging.LogLevel.Off) return false;
            return level >= targetLevel;
        }

        /// <summary> 添加适配器 </summary>
        /// <param name="adapter">适配器</param>
        public static void AddAdapter(ILoggerAdapter adapter)
        {
            if (LoggerAdapters.ContainsKey(adapter))
            {
                return;
            }
            LoggerAdapters.TryAdd(adapter, _logLevel);
        }

        /// <summary> 添加适配器 </summary>
        /// <param name="adapter">适配器</param>
        /// <param name="level">日志等级(可多位与)</param>
        public static void AddAdapter(ILoggerAdapter adapter, LogLevel level)
        {
            if (!IsEnableLevel(level))
                level = _logLevel;

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
            adapters.Foreach(t => LoggerAdapters.TryRemove(t.Key, out _));
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
            if (LoggerDictionary.TryGetValue(name, out var logger))
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

        internal static IEnumerable<ILog> GetLogs(string name, LogLevel level)
        {
            return LoggerAdapters.Where(t => IsEnableLevel(level, t.Value)).Select(t => t.Key.GetLogger(name)).ToList();
        }

        internal static void EachAdapter(this string loggerName, LogLevel level, Action<ILog> action)
        {
            if (string.IsNullOrWhiteSpace(loggerName) || !IsEnableLevel(level))
                return;
            var logs = GetLogs(loggerName, level);
            foreach (var log in logs)
            {
                log.LoggerName = loggerName;
                action(log);
            }
        }
    }
}
