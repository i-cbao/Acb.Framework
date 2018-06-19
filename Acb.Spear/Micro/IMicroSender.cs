using System.Threading.Tasks;
using Acb.Spear.Message;

namespace Acb.Spear.Micro
{
    /// <summary>  消息发送者 </summary>
    public interface IMicroSender
    {
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="flush"></param>
        /// <returns></returns>
        Task Send(IMicroMessage message, bool flush = true);
    }
}
