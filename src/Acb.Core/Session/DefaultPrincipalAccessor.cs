using Acb.Core.Dependency;
using System.Security.Claims;
using System.Threading;

namespace Acb.Core.Session
{
    public class DefaultPrincipalAccessor : IPrincipalAccessor, ISingleDependency
    {
        public virtual ClaimsPrincipal Principal => AcbHttpContext.Current == null
            ? Thread.CurrentPrincipal as ClaimsPrincipal
            : AcbHttpContext.Current.User;
    }
}
