using Acb.Core.Extensions;
using System;
using System.Collections.Generic;

namespace Acb.Core.Session
{
    public abstract class AcbSessionBase : IAcbSession
    {
        private const string ConfigKey = "tenancy:enable";
        private object _tempUserId;
        private object _tempTenantId;

        protected bool EnableTenancy => ConfigKey.Config(false);

        public object UserId => _tempUserId ?? GetUserId();

        public object TenantId => EnableTenancy ? _tempTenantId ?? GetTenantId() : null;

        public abstract string UserName { get; }
        public abstract string Role { get; }

        protected abstract object GetUserId();
        protected abstract object GetTenantId();

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
