using System;
using System.Collections.Generic;
using System.Reflection;

namespace Acb.Core.Modules
{
    /// <summary> 模块信息类 </summary>
    public class DModuleInfo
    {
        public Assembly Assembly { get; private set; }

        public Type Type { get; private set; }

        public DModule Instance { get; private set; }

        public List<DModuleInfo> Dependencies { get; private set; }

        public DModuleInfo(DModule instance)
        {
            Type = instance.GetType();
            Instance = instance;
            Assembly = Type.Assembly;
            Dependencies = new List<DModuleInfo>();
        }
    }
}
