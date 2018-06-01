using Acb.Core;
using Acb.Core.Extensions;
using Acb.Core.Reflection;
using System;

namespace Acb.Framework
{
    public class DAssemblyFinder : DefaultAssemblyFinder
    {
        private const string FrameworkName = "Acb.";
        private static readonly string BaseAssemblyName;

        static DAssemblyFinder()
        {
            BaseAssemblyName = "baseAssembly".Config(string.Empty);
        }

        private static readonly Func<string, bool> AssemblyFinder =
            t =>
            {
                var frame = t.StartsWith(FrameworkName, StringComparison.CurrentCultureIgnoreCase);
                if (frame)
                    return true;
                return !string.IsNullOrWhiteSpace(BaseAssemblyName) && t.StartsWith(BaseAssemblyName);
            };

        public DAssemblyFinder()
            : base(ass => AssemblyFinder.Invoke(ass.FullName), AssemblyFinder)
        {
        }

        public static DAssemblyFinder Instance => Singleton<DAssemblyFinder>.Instance ??
                                                       (Singleton<DAssemblyFinder>.Instance = new DAssemblyFinder());
    }
}
