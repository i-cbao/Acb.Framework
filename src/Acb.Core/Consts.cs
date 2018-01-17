using Acb.Core.Domain;
using Acb.Core.Extensions;

namespace Acb.Core
{
    public static class Consts
    {
        public const string Version = "0.1.0";
        public static ProductMode Mode => "mode".Config(ProductMode.Dev);
    }
}