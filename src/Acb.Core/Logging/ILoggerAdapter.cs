﻿using System;

namespace Acb.Core.Logging
{
    /// <summary>
    /// 日志实现适配器
    /// </summary>
    public interface ILoggerAdapter
    {
        /// <summary>
        /// 由指定类型获取<see cref="ILog"/>日志实例
        /// </summary>
        /// <param name="type">指定类型</param>
        /// <returns></returns>
        ILog GetLogger(Type type);

        /// <summary>
        /// 由指定名称获取<see cref="ILog"/>日志实例
        /// </summary>
        /// <param name="name">指定名称</param>
        /// <returns></returns>
        ILog GetLogger(string name);
    }
}
