using System.Net;

namespace Acb.Spear.Micro
{
    public interface IMicroClientFactory
    {
        /// <summary> 创建客户端 </summary>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        IMicroClient CreateClient(EndPoint endPoint);
    }
}
