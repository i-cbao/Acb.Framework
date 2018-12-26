using System;

namespace Acb.Aop
{
    public interface IAopFactory
    {
        /// <summary> 创建拦截器 </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TImp"></typeparam>
        /// <param name="interceptorType"></param>
        /// <param name="actionType"></param>
        /// <returns></returns>
        T Create<T, TImp>(Type interceptorType = null, Type actionType = null)
            where TImp : class, new() where T : class;

        /// <summary> 创建拦截器 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="interceptorType"></param>
        /// <param name="actionType"></param>
        /// <returns></returns>
        T Create<T>(Type interceptorType = null, Type actionType = null) where T : class, new();
    }
}
