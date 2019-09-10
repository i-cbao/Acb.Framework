namespace Acb.MicroService.PureClient
{
    /// <summary> 服务配置 </summary>
    public class MicroServiceConfig
    {
        /// <summary>
        /// 开发模式
        /// env:MICRO_MODE,默认Test
        /// </summary>
        public ProductMode Mode { get; set; }

        private string _consulServer;

        /// <summary>
        /// Consul服务地址
        /// env:MICRO_CONSUL
        /// </summary>
        public string ConsulServer
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_consulServer))
                    return _consulServer;
                return Mode == ProductMode.Prod
                    ? "http://micro.icb/consul/"
                    : "http://192.168.0.231:8500";
            }
            set => _consulServer = value;
        }

        /// <summary>
        /// Consul令牌
        /// env:MICRO_CONSUL_TOKEN,默认空字符
        /// </summary>
        public string ConsulToken { get; set; }

        /// <summary> 微服务配置 </summary>
        public MicroServiceConfig()
        {
            Mode = "MICRO_MODE".Env(ProductMode.Test);
            ConsulServer = "MICRO_CONSUL".Env();
            ConsulToken = "MICRO_CONSUL_TOKEN".Env();
        }
    }
}
