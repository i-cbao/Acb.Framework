using System;

namespace Acb.Spear.Contracts.Enums
{
    [Flags]
    public enum SecurityEnum : byte
    {
        /// <summary> 匿名 </summary>
        None = 0,
        /// <summary> 获取时验证 </summary>
        Get = 1,
        /// <summary> 管理时验证 </summary>
        Manage = 2
    }
}
