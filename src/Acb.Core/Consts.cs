using Acb.Core.Domain;
using Acb.Core.Extensions;

namespace Acb.Core
{
    /// <summary> 常量 </summary>
    public static class Consts
    {
        /// <summary> 版本号 </summary>
        public const string Version = "1.3.0";

        /// <summary> 产品模式 </summary>
        public static ProductMode Mode => "mode".Config(ProductMode.Dev);

        /// <summary> Ticket配置键 </summary>
        public const string AppTicketKey = "ticketKey";
    }
}