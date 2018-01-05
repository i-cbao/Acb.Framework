using System;
using System.Collections.Generic;

namespace Acb.Core
{
    /// <summary> 单例辅助 </summary>
    public class Singleton
    {
        static Singleton()
        {
            AllSingletons = new Dictionary<Type, object>();
        }

        /// <summary>
        /// Dictionary of type to singleton instances.
        /// </summary>
        public static IDictionary<Type, object> AllSingletons { get; }
    }

    /// <summary> 单例泛型辅助 </summary>
    /// <typeparam name="T"></typeparam>
    public class Singleton<T> : Singleton
    {
        private static T _instance;

        public static T Instance
        {
            get => _instance;
            set
            {
                _instance = value;
                AllSingletons[typeof(T)] = value;
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