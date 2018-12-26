using System;

namespace Acb.Aop.Impl
{
    public class AopFactory : IAopFactory
    {
        public T Create<T, TImp>(Type interceptorType = null, Type actionType = null) where T : class where TImp : class, new()
        {
            throw new NotImplementedException();
        }

        public T Create<T>(Type interceptorType = null, Type actionType = null) where T : class, new()
        {
            throw new NotImplementedException();
        }
    }
}
