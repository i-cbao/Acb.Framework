using System;
using System.Linq;

namespace Acb.Core.Logging
{
    public abstract class LogBase : ILog
    {
        private readonly WriteHandler _write;
        /// <summary>
        /// 初始化一个<see cref="LogBase"/>类型的新实例
        /// </summary>
        protected LogBase()
        {
            _write = GetWriteHandler() ?? WriteInternal;
        }

        /// <summary>
        /// 获取日志输出处理委托实例
        /// </summary>
        /// <returns></returns>
        private static WriteHandler GetWriteHandler()
        {
            return null;
        }

        /// <summary>
        /// 获取日志输出处理委托实例
        /// </summary>
        /// <param name="level">日志输出级别</param>
        /// <param name="message">日志消息</param>
        /// <param name="exception">日志异常</param>
        protected abstract void WriteInternal(LogLevel level, object message, Exception exception);

        public string LoggerName { get; set; }

        /// <summary>
        /// 获取 是否允许输出<see cref="LogLevel.Trace"/>级别的日志
        /// </summary>
        public abstract bool IsTraceEnabled { get; }

        /// <summary>
        /// 获取 是否允许输出<see cref="LogLevel.Debug"/>级别的日志
        /// </summary>
        public abstract bool IsDebugEnabled { get; }

        /// <summary>
        /// 获取 是否允许输出<see cref="LogLevel.Info"/>级别的日志
        /// </summary>
        public abstract bool IsInfoEnabled { get; }

        /// <summary>
        /// 获取 是否允许输出<see cref="LogLevel.Warn"/>级别的日志
        /// </summary>
        public abstract bool IsWarnEnabled { get; }

        /// <summary>
        /// 获取 是否允许输出<see cref="LogLevel.Error"/>级别的日志
        /// </summary>
        public abstract bool IsErrorEnabled { get; }

        /// <summary>
        /// 获取 是否允许输出<see cref="LogLevel.Fatal"/>级别的日志
        /// </summary>
        public abstract bool IsFatalEnabled { get; }

        /// <summary>
        /// 写入<see cref="LogLevel.Trace"/>日志消息
        /// </summary>
        /// <param name="message">日志消息</param>
        public virtual void Trace<T>(T message)
        {
            if (IsTraceEnabled)
                _write(LogLevel.Trace, message);
        }

        /// <summary>
        /// 写入<see cref="LogLevel.Trace"/>日志消息
        /// </summary>
        /// <param name="format">日志消息格式</param>
        /// <param name="args">格式化参数</param>
        public virtual void Trace(string format, params object[] args)
        {
            if (IsTraceEnabled)
                _write(LogLevel.Trace, args != null && args.Any() ? string.Format(format, args) : format);
        }

        /// <summary>
        /// 写入<see cref="LogLevel.Debug"/>日志消息
        /// </summary>
        /// <param name="message">日志消息</param>
        public virtual void Debug<T>(T message)
        {
            if (IsDebugEnabled)
                _write(LogLevel.Debug, message);
        }

        /// <summary>
        /// 写入<see cref="LogLevel.Debug"/>格式化日志消息
        /// </summary>
        /// <param name="format">日志消息格式</param>
        /// <param name="args">格式化参数</param>
        public virtual void Debug(string format, params object[] args)
        {
            if (IsDebugEnabled)
                _write(LogLevel.Debug, args != null && args.Any() ? string.Format(format, args) : format);
        }

        /// <summary>
        /// 写入<see cref="LogLevel.Info"/>日志消息
        /// </summary>
        /// <param name="message">日志消息</param>
        public virtual void Info<T>(T message)
        {
            if (IsInfoEnabled)
                _write(LogLevel.Info, message);
        }

        /// <summary>
        /// 写入<see cref="LogLevel.Info"/>格式化日志消息
        /// </summary>
        /// <param name="format">日志消息格式</param>
        /// <param name="args">格式化参数</param>
        public virtual void Info(string format, params object[] args)
        {
            if (IsInfoEnabled)
                _write(LogLevel.Info, args != null && args.Any() ? string.Format(format, args) : format);
        }

        /// <summary>
        /// 写入<see cref="LogLevel.Warn"/>日志消息
        /// </summary>
        /// <param name="message">日志消息</param>
        public virtual void Warn<T>(T message)
        {
            if (IsWarnEnabled)
                _write(LogLevel.Warn, message);
        }

        /// <summary>
        /// 写入<see cref="LogLevel.Warn"/>格式化日志消息
        /// </summary>
        /// <param name="format">日志消息格式</param>
        /// <param name="args">格式化参数</param>
        public virtual void Warn(string format, params object[] args)
        {
            if (IsWarnEnabled)
                _write(LogLevel.Warn, args != null && args.Any() ? string.Format(format, args) : format);
        }

        /// <summary>
        /// 写入<see cref="LogLevel.Error"/>日志消息
        /// </summary>
        /// <param name="message">日志消息</param>
        public virtual void Error<T>(T message)
        {
            if (IsErrorEnabled)
                _write(LogLevel.Error, message);
        }

        /// <summary>
        /// 写入<see cref="LogLevel.Error"/>格式化日志消息
        /// </summary>
        /// <param name="format">日志消息格式</param>
        /// <param name="args">格式化参数</param>
        public void Error(string format, params object[] args)
        {
            if (IsErrorEnabled)
                _write(LogLevel.Error, (args != null && args.Any()) ? string.Format(format, args) : format);
        }

        /// <summary>
        /// 写入<see cref="LogLevel.Error"/>日志消息，并记录异常
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="exception">异常</param>
        public virtual void Error<T>(T message, Exception exception)
        {
            if (IsErrorEnabled)
                _write(LogLevel.Error, message, exception);
        }

        /// <summary>
        /// 写入<see cref="LogLevel.Error"/>格式化日志消息，并记录异常
        /// </summary>
        /// <param name="format">日志消息格式</param>
        /// <param name="exception">异常</param>
        /// <param name="args">格式化参数</param>
        public virtual void Error(string format, Exception exception, params object[] args)
        {
            if (IsErrorEnabled)
                _write(LogLevel.Error, (args != null && args.Any()) ? string.Format(format, args) : format, exception);
        }

        /// <summary>
        /// 写入<see cref="LogLevel.Fatal"/>日志消息
        /// </summary>
        /// <param name="message">日志消息</param>
        public virtual void Fatal<T>(T message)
        {
            if (IsFatalEnabled)
                _write(LogLevel.Fatal, message);
        }

        /// <summary>
        /// 写入<see cref="LogLevel.Fatal"/>格式化日志消息
        /// </summary>
        /// <param name="format">日志消息格式</param>
        /// <param name="args">格式化参数</param>
        public void Fatal(string format, params object[] args)
        {
            if (IsFatalEnabled)
                _write(LogLevel.Fatal, (args != null && args.Any()) ? string.Format(format, args) : format);
        }

        /// <summary>
        /// 写入<see cref="LogLevel.Fatal"/>日志消息，并记录异常
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="exception">异常</param>
        public virtual void Fatal<T>(T message, Exception exception)
        {
            if (IsFatalEnabled)
                _write(LogLevel.Fatal, message, exception);
        }

        /// <summary>
        /// 写入<see cref="LogLevel.Fatal"/>格式化日志消息，并记录异常
        /// </summary>
        /// <param name="format">日志消息格式</param>
        /// <param name="exception">异常</param>
        /// <param name="args">格式化参数</param>
        public virtual void Fatal(string format, Exception exception, params object[] args)
        {
            if (IsFatalEnabled)
                _write(LogLevel.Fatal, (args != null && args.Any()) ? string.Format(format, args) : format, exception);
        }

        protected delegate void WriteHandler(LogLevel level, object message, Exception exception = null);
    }
}
