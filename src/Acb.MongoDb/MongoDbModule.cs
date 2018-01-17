using Acb.Core;
using Acb.Core.Modules;

namespace Acb.MongoDb
{
    [DependsOn(typeof(CoreModule))]
    public class MongoDbModule : DModule
    {
        public override void Initialize()
        {
            base.Initialize();
        }
    }
}
