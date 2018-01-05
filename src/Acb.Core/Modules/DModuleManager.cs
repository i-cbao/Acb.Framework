using Acb.Core.Dependency;
using Acb.Core.Extensions;
using Acb.Core.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Acb.Core.Modules
{
    /// <summary> 模块依赖项管理 </summary>
    public class DModuleManager : IModuleManager
    {
        private readonly IList<DModuleInfo> _modules;

        private readonly IIocManager _iocManager;

        public DModuleManager(IIocManager iocManager)
        {
            _iocManager = iocManager;
            _modules = new List<DModuleInfo>();
        }

        public ITypeFinder TypeFinder { get; set; }

        /// <summary> 初始化模块 </summary>
        public void InitializeModules()
        {
            LoadModules();
            var sortedModules = _modules.SortByDependencies(m => m.Dependencies).ToList();
            //模块前置操作
            sortedModules.ForEach(m => m.Instance.PreInitialize());
            //模块初始化
            sortedModules.ForEach(m => m.Instance.Initialize());
            //模块加载完成
            sortedModules.ForEach(m => m.Instance.PostInitialize());
        }

        /// <summary> 关闭模块 </summary>
        public void ShutdownModules()
        {
            var sortedModules = _modules.SortByDependencies(m => m.Dependencies).ToList();
            sortedModules.Reverse();
            sortedModules.ForEach(sm => sm.Instance.Shutdown());
        }

        /// <summary> 加载所有模块项目 </summary>
        private void LoadModules()
        {
            var types = AddMissingDependedModules(TypeFinder.Find(ModuleExtend.IsDModule));
            foreach (var type in types)
            {
                var instance = (DModule)_iocManager.Resolve(type);
                if (instance == null)
                    continue;
                instance.IocManager = _iocManager;
                _modules.Add(new DModuleInfo(instance));
            }
            SetDependencies();
        }

        /// <summary> 设置依赖项 </summary>
        private void SetDependencies()
        {
            foreach (var moduleInfo in _modules)
            {
                //Set dependencies according to assembly dependency
                foreach (var referencedAssemblyName in moduleInfo.Assembly.GetReferencedAssemblies())
                {
                    var referencedAssembly = Assembly.Load(referencedAssemblyName);
                    var dependedModuleList = _modules.Where(m => m.Assembly == referencedAssembly).ToList();
                    if (dependedModuleList.Count > 0)
                    {
                        moduleInfo.Dependencies.AddRange(dependedModuleList);
                    }
                }

                //Set dependencies for defined DependsOnAttribute attribute(s).
                foreach (var dependedModuleType in moduleInfo.Type.FindDependencyModules())
                {
                    var dependedModuleInfo = _modules.FirstOrDefault(m => m.Type == dependedModuleType);
                    if (dependedModuleInfo == null)
                    {
                        continue;
                    }

                    if ((moduleInfo.Dependencies.FirstOrDefault(dm => dm.Type == dependedModuleType) == null))
                    {
                        moduleInfo.Dependencies.Add(dependedModuleInfo);
                    }
                }
            }
        }

        /// <summary> 添加缺失的依赖模块 </summary>
        /// <param name="allModules">所有模块</param>
        /// <returns></returns>
        private static ICollection<Type> AddMissingDependedModules(ICollection<Type> allModules)
        {
            var initialModules = allModules.ToList();
            foreach (var module in initialModules)
            {
                FillDependedModules(module, allModules);
            }

            return allModules;
        }

        /// <summary> 填充依赖模块 </summary>
        /// <param name="module">当前模块</param>
        /// <param name="allModules">所有模块</param>
        private static void FillDependedModules(Type module, ICollection<Type> allModules)
        {
            foreach (var dependedModule in module.FindDependencyModules())
            {
                if (allModules.Contains(dependedModule)) continue;
                allModules.Add(dependedModule);
                FillDependedModules(dependedModule, allModules);
            }
        }
    }
}
