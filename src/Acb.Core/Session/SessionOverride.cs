namespace Acb.Core.Session
{
    public class SessionOverride
    {
        public object UserId { get; }
        public object TenantId { get; }

        public SessionOverride(object userId, object tenantId)
        {
            UserId = userId;
            TenantId = tenantId;
        }
    }
}
