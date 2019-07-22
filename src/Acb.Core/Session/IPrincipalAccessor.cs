using System.Security.Claims;

namespace Acb.Core.Session
{
    public interface IPrincipalAccessor
    {
        ClaimsPrincipal Principal { get; }
    }
}
