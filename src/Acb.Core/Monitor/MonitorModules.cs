namespace Acb.Core.Monitor
{
    public static class MonitorModules
    {
        /// <summary> 微服务调用 </summary>
        public const string MicroService = "micro_service";
        public const string MicroClient = "micro_client";

        /// <summary> 网关调用 </summary>
        public const string Gateway = "gateway";

        public const string Action = "action";

        /// <summary> Http请求 </summary>
        public const string HttpRequest = "http";
    }
}
