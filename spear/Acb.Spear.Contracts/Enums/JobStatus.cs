using System.ComponentModel;

namespace Acb.Spear.Contracts.Enums
{
    public enum JobStatus : byte
    {
        /// <summary> 已暂停 </summary>
        [Description("已暂停")]
        Disabled = 0,
        /// <summary> 已启动 </summary>
        [Description("已启动")]
        Enabled = 1,

        /// <summary> 已异常 </summary>
        [Description("异常")]
        Error = 2,

        /// <summary> 已删除 </summary>
        [Description("已删除")]
        Delete = 4
    }
}
