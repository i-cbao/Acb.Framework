using Acb.Core.Domain.Dtos;
using Acb.Core.Extensions;
using System;

namespace Acb.Core.Session
{
    public abstract class AcbSessionBase : IAcbSession
    {
        private const string ConfigKey = "tenancy:enable";
        /// <summary> 临时Session </summary>
        protected SessionDto TempSession;

        /// <summary> 是否开启多租户 </summary>
        protected bool EnableTenancy => ConfigKey.Config(false);

        /// <summary> 用户ID </summary>
        public object UserId => TempSession?.UserId ?? GetUserId();

        /// <summary> 租户ID </summary>
        public object TenantId => EnableTenancy ? TempSession?.TenantId ?? GetTenantId() : null;

        /// <summary> 用户名 </summary>
        public abstract string UserName { get; }

        /// <summary> 角色 </summary>
        public abstract string Role { get; }

        /// <summary> 获取用户ID </summary>
        /// <returns></returns>
        protected abstract object GetUserId();

        /// <summary> 获取租户ID </summary>
        /// <returns></returns>
        protected abstract object GetTenantId();

        /// <summary> 租户类型 </summary>
        public TenancySides TenancySides => EnableTenancy && TenantId != null
            ? TenancySides.Tenant
            : TenancySides.Host;

        /// <summary> 使用临时Session </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public IDisposable Use(SessionDto session)
        {
            TempSession = session;
            return new DisposeAction(() => { TempSession = null; });
        }
    }
}
