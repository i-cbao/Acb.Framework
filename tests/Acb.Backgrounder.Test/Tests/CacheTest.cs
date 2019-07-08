using Acb.Core.Cache;
using Acb.Core.Extensions;
using Acb.Redis;
using Autofac;
using System;

namespace Acb.Backgrounder.Test.Tests
{
    public class CacheTest : ConsoleTest
    {

        public override void OnUseServiceProvider(IServiceProvider provider)
        {
            provider.UseCacheSync();
        }

        public override void OnCommand(string cmd, IContainer provider)
        {
            var cache = CacheManager.GetCacher("console", CacheLevel.Both);
            var arr = cmd.Split(new[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries);
            if (arr.Length < 2) return;
            switch (arr[0])
            {
                case "set":
                    cache.Set(arr[1], arr.Length > 2 ? arr[2] : null);
                    break;
                case "rm":
                    cache.Remove(arr[1]);
                    break;
                case "cnf":
                    var cnf = arr[1].Config(string.Empty);
                    cnf.PrintSucc();
                    break;
                default:
                    var value = cache.Get<string>(arr[1]);
                    value.PrintSucc();
                    break;
            }
        }
    }
}
