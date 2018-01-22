using Acb.Core.Domain;
using Acb.Core.Extensions;

namespace Acb.Core
{
    /// <summary> 常量 </summary>
    public static class Consts
    {
        public const string Version = "0.1.0";

        /// <summary> 产品模式 </summary>
        public static ProductMode Mode => "mode".Config(ProductMode.Dev);
    }
}