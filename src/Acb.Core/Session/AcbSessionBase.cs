using Acb.Core.Extensions;
using System;

namespace Acb.Core.Session
{
    public abstract class AcbSessionBase : IAcbSession
    {
        private const string ConfigKey = "tenancy:enable";
        private object _tempUserId;
        private object _tempTenantId;

        protected bool EnableTenancy => ConfigKey.Config(false);

        public object UserId => _tempUserId ?? CurrentUserId;

        public object TenantId => EnableTenancy ? _tempTenantId ?? CurrentTenantId : null;

        public abstract string UserName { get; }
        public abstract string Role { get; }

        protected abstract object CurrentUserId { get; }
        protected abstract object CurrentTenantId { get; }

        public TenancySides TenancySides => EnableTenancy && TenantId != null
            ? TenancySides.Tenant
            : TenancySides.Host;

        public IDisposable Use(object userId, object tenantId)
        {
            _tempUserId = userId;
            _tempTenantId = tenantId;
            return new DisposeAction(() =>
            {
                _tempUserId = null;
                _tempTenantId = null;
            });
        }
    }
}
