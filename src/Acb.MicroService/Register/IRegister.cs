using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Acb.MicroService.Register
{
    internal interface IRegister
    {
        Task Regist(HashSet<Assembly> asses);

        Task Deregist();
    }
}
