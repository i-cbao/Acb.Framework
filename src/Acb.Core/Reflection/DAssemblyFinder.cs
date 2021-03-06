﻿using Acb.Core.Extensions;
using System;

namespace Acb.Core.Reflection
{
    /// <summary> 默认程序集查找器 </summary>
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
    }
}
