using System;

namespace Acb.Middleware.JobScheduler.Domain.Enums
{
    [Flags]
    public enum ConfigSecurity : byte
    {
        /// <summary> 匿名 </summary>
        None = 0,
        /// <summary> 获取验证 </summary>
        Get = 1,
        /// <summary> 管理验证 </summary>
        Manage = 2
    }
}
