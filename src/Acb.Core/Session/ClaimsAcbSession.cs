using Acb.Core.Dependency;
using Acb.Core.Security;
using System.Linq;

namespace Acb.Core.Session
{
    public class ClaimsAcbSession : AcbSessionBase, ISingleDependency
    {
        private readonly IPrincipalAccessor _principalAccessor;

        public ClaimsAcbSession(IPrincipalAccessor principalAccessor)
        {
            _principalAccessor = principalAccessor;
        }

        private string GetClaimValue(string type)
        {
            var claim = _principalAccessor.Principal?.Claims.FirstOrDefault(t => t.Type == type);
            return string.IsNullOrWhiteSpace(claim?.Value) ? null : claim.Value;
        }

        public override string UserName => GetClaimValue(AcbClaimTypes.UserName);

        public override string Role => GetClaimValue(AcbClaimTypes.Role);

        protected override object GetUserId()
        {
            return GetClaimValue(AcbClaimTypes.UserId);
        }

        protected override object GetTenantId()
        {
            var tenantId = GetClaimValue(AcbClaimTypes.TenantId);
            if (tenantId != null)
                return tenantId;
            return CurrentIocManager.IsRegistered<ITenantResolver>()
                ? CurrentIocManager.Resolve<ITenantResolver>().ResolveTenantId()
                : null;
        }
    }
}
