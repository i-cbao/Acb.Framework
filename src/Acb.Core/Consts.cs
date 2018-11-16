using Acb.Core.Domain;
using Acb.Core.Extensions;
using System;

namespace Acb.Core
{
    /// <summary> 常量 </summary>
    public static class Consts
    {
        /// <summary> 版本号 </summary>
        public const string Version = "1.3.0";

        private const string ModeEnvironmentName = "ACB_MODE";
        private const string ModeConfigName = "mode";

        /// <summary> 产品模式 </summary>
        public static ProductMode Mode
        {
            get
            {
                var mode = ModeEnvironmentName.Env();
                if (string.IsNullOrWhiteSpace(mode))
                    mode = ModeConfigName.Config<string>();
                return mode.CastTo(ProductMode.Dev);
            }
        }

        /// <summary> Ticket配置键 </summary>
        public const string AppTicketKey = "ticketKey";
    }
}