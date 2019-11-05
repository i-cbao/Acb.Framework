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

        /// <summary> 用户名 </summary>
        public override string UserName =>
            TempSession != null ? TempSession.UserName : GetClaimValue(AcbClaimTypes.UserName);

        /// <summary> 用户角色 </summary>
        public override string Role => TempSession != null ? TempSession.Role : GetClaimValue(AcbClaimTypes.Role);

        /// <summary> 用户ID </summary>
        /// <returns></returns>
        protected override object GetUserId()
        {
            return GetClaimValue(AcbClaimTypes.UserId);
        }

        /// <summary> 租户ID </summary>
        /// <returns></returns>
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
