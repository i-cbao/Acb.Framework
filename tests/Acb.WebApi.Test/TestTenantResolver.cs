using Acb.Core.Dependency;
using Acb.Core.Extensions;
using Acb.Core.Session;

namespace Acb.WebApi.Test
{
    public class TestTenantResolver : ITenantResolver, ISingleDependency
    {
        public object ResolveTenantId()
        {
            return "tenant".QueryOrForm<string>(null);
        }
    }
}
