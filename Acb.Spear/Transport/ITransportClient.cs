using Acb.Spear.Message;
using System.Threading.Tasks;

namespace Acb.Spear.Transport
{
    /// <summary> 传输客户端 </summary>
    public interface ITransportClient
    {
        /// <summary> 发送消息 </summary>
        /// <param name="message">远程调用消息模型</param>
        /// <returns>远程调用消息的传输消息</returns>
        Task<InvokeResultMessage> SendAsync(InvokeMessage message);
    }
}
