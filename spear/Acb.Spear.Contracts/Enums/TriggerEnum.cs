using System.ComponentModel;

namespace Acb.Spear.Contracts.Enums
{
    public enum TriggerType : byte
    {
        /// <summary> 无 </summary>
        None = 0,
        /// <summary> Corn表达式 </summary>
        Cron = 1,
        /// <summary> 简单 </summary>
        Simple = 2
    }

    /// <summary> 触发器状态 </summary>
    public enum TriggerStatus : byte
    {
        [Description("已禁用")]
        Disable = 0,
        [Description("已启用")]
        Enable = 1,
        [Description("已删除")]
        Delete = 4
    }
}
