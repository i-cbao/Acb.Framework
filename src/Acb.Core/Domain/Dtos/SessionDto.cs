namespace Acb.Core.Domain.Dtos
{
    /// <summary> Session </summary>
    public class SessionDto
    {
        /// <summary> 用户ID </summary>
        public object UserId { get; set; }

        /// <summary> 租户ID </summary>
        public object TenantId { get; set; }

        /// <summary> 用户名 </summary>
        public string UserName { get; set; }

        /// <summary> 角色 </summary>
        public string Role { get; set; }
    }
}
