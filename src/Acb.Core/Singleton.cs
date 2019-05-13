﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Acb.Core
{
    /// <summary> 单例辅助 </summary>
    public class Singleton
    {
        /// <summary> 所有单例 </summary>
        protected static ConcurrentDictionary<Type, Lazy<object>> AllSingletons { get; }

        static Singleton()
        {
            AllSingletons = new ConcurrentDictionary<Type, Lazy<object>>();
        }
    }

    /// <summary> 单例泛型辅助 </summary>
    /// <typeparam name="T"></typeparam>
    public class Singleton<T> : Singleton
    {
        /// <summary> 单例 </summary>
        public static T Instance
        {
            get
            {
                if (AllSingletons.TryGetValue(typeof(T), out var instance))
                    return (T)instance.Value;
                return default(T);
            }
            set
            {
                var lazy = new Lazy<object>(() => value);
                AllSingletons.AddOrUpdate(typeof(T), lazy,
                    (type, obj) => new Lazy<object>(() => value == null ? obj : lazy));
            }
        }
    }

    /// <summary>
    /// 单例泛型列表辅助
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SingletonList<T> : Singleton<IList<T>>
    {
        static SingletonList()
        {
            Singleton<IList<T>>.Instance = new List<T>();
        }

        /// <summary>
        /// The singleton instance for the specified type T. Only one instance (at the time) of this list for each type of T.
        /// </summary>
        public new static IList<T> Instance => Singleton<IList<T>>.Instance;
    }
}