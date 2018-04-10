using System.Collections.Generic;
using System.Reflection;

namespace Acb.MicroService.Register
{
    internal interface IRegister
    {
        void Regist(HashSet<Assembly> asses, MicroServiceConfig config);

        void Deregist();
    }
}
