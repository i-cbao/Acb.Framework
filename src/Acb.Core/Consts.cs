using Acb.Core.Config;

namespace Acb.Core
{
    public static class Consts
    {
        public const string Version = "0.1.0";

        public static DConfig Config => ConfigUtils<DConfig>.Config;
    }
}
