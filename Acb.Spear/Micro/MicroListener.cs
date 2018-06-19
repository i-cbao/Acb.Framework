using Acb.Spear.Message;
using System.Threading.Tasks;

namespace Acb.Spear.Micro
{
    /// <summary> 默认服务监听者 </summary>
    public class MicroListener : IMicroListener
    {
        public event ReceivedDelegate Received;
        public async Task OnReceived(IMicroSender sender, MicroMessage message)
        {
            if (Received == null)
                return;
            await Received(sender, message);
        }
    }
}
