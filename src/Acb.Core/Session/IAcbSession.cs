using Acb.Core.Exceptions;
using Acb.Core.Extensions;
using System;
using Acb.Core.Domain.Dtos;

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
        /// <param name="session"></param>
        /// <returns></returns>
        IDisposable Use(SessionDto session);
    }

    public static class AcbSessionExtensions
    {
        /// <summary> 获取UserId </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="session"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public static T GetUserId<T>(this IAcbSession session, T def = default(T))
        {
            return session.UserId.CastTo(def);
        }

        /// <summary> 获取TenantId </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="session"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public static T GetTenantId<T>(this IAcbSession session, T def = default(T))
        {
            return session.TenantId.CastTo(def);
        }

        /// <summary> 获取必须的UserId(如果没有将抛出异常) </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="session"></param>
        /// <returns></returns>
        public static T GetRequiredUserId<T>(this IAcbSession session)
        {
            var value = session.UserId.CastTo<T>();
            if (Equals(value, default(T)))
                throw new BusiException("userId 不能为空");
            return value;
        }

        /// <summary> 获取必须的TenantId(如果没有将抛出异常) </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="session"></param>
        /// <returns></returns>
        public static T GetRequiredTenantId<T>(this IAcbSession session)
        {
            var value = session.TenantId.CastTo<T>();
            if (Equals(value, default(T)))
                throw new BusiException("tenantId 不能为空");
            return value;
        }

        /// <summary> 是否是租客 </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public static bool IsTenant(this IAcbSession session)
        {
            return session.TenantId != null;
        }

        /// <summary> 是否是匿名访问 </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public static bool IsGuest(this IAcbSession session)
        {
            return session.UserId == null || string.IsNullOrWhiteSpace(session.GetUserId<string>());
        }

    }
}
