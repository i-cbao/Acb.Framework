using Acb.Core.Dependency;
using Acb.Core.Extensions;
using Acb.Core.Helper;
using Acb.Core.Modules;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Linq;

namespace Acb.Core
{
    /// <summary> 初始化模块 </summary>
    public class CoreModule : DModule
    {
        public override void Initialize()
        {
            CurrentIocManager.IocManager = IocManager;

            var configPath = "configPath".Config<string>();
            if (!string.IsNullOrWhiteSpace(configPath))
            {
                configPath = Path.Combine(Directory.GetCurrentDirectory(), configPath);
                if (Directory.Exists(configPath))
                {
                    var jsons = Directory.GetFiles(configPath, "*.json");
                    if (jsons != null && jsons.Any())
                    {
                        ConfigHelper.Instance.Build(b =>
                        {
                            foreach (var json in jsons)
                            {
                                b.AddJsonFile(json);
                            }
                        });
                    }
                }
            }
            base.Initialize();
        }
    }
}
