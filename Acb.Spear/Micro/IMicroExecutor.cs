using Acb.Spear.Message;
using System.Threading.Tasks;

namespace Acb.Spear.Micro
{
    /// <summary> 服务执行者 </summary>
    public interface IMicroExecutor
    {
        /// <summary> 执行 </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        Task Execute(IMicroSender sender, MicroMessage message);
    }
}
