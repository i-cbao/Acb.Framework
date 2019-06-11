namespace Acb.WebApi.Filters
{
    /// <summary> Action执行监控 </summary>
    public class ActionTimingFilter : RecordFilter
    {
        public ActionTimingFilter() : base("gateway")
        {
        }
    }
}
