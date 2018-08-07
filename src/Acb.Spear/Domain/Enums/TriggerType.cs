namespace Acb.Middleware.JobScheduler.Domain.Enums
{
    public enum TriggerType
    {
        /// <summary> 无 </summary>
        None = 0,
        /// <summary> Corn表达式 </summary>
        Cron = 1,
        /// <summary> 简单 </summary>
        Simple = 2
    }
}
