using System.Threading.Tasks;
using Acb.Spear.Message;

namespace Acb.Spear
{
    public interface IMicroClient
    {
        Task Send(IMicroMessage message, bool flush = true);
        T CreateProxy<T>();
    }
}
