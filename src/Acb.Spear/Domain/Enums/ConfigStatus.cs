namespace Acb.Middleware.JobScheduler.Domain.Enums
{
    public enum ConfigStatus : byte
    {
        /// <summary> 正常 </summary>
        Normal = 0,
        /// <summary> 历史版本 </summary>
        History = 1,
        /// <summary> 已删除 </summary>
        Delete = 2
    }
}
