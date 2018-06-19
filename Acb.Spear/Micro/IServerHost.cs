using System;
using System.Net;
using System.Threading.Tasks;

namespace Acb.Spear.Micro
{
    /// <inheritdoc />
    /// <summary> 服务主机 </summary>
    public interface IServerHost : IDisposable
    {
        Task Start(EndPoint endPoint);
        Task Close();
    }
}
