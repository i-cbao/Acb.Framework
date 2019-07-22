using Acb.Core.Extensions;
using System;

namespace Acb.Core.Session
{
    /// <summary> 租户站点类型 </summary>
    [Flags]
    public enum TenancySides
    {
        /// <summary> 租户 </summary>
        Tenant = 1,
        /// <summary> 主机 </summary>
        Host = 2
    }


    public interface IAcbSession
    {
        /// <summary> 用户ID </summary>
        object UserId { get; }

        /// <summary> 租户ID </summary>
        object TenantId { get; }

        /// <summary> 用户名 </summary>
        string UserName { get; }

        /// <summary> 角色 </summary>
        string Role { get; }

        /// <summary> 多租户类型 </summary>
        TenancySides TenancySides { get; }

        /// <summary> 使用租户 </summary>
        /// <param name="userId"></param>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        IDisposable Use(object userId, object tenantId);
    }

    public static class AcbSessionExtensions
    {
        public static T GetUserId<T>(this IAcbSession session, T def = default(T))
        {
            return session.UserId.CastTo(def);
        }

        public static T GetTenantId<T>(this IAcbSession session, T def = default(T))
        {
            return session.TenantId.CastTo(def);
        }
    }
}
