namespace Acb.MicroService.PureClient
{
    /// <summary> 服务配置 </summary>
    public class MicroServiceConfig
    {
        private ProductMode _mode = ProductMode.Test;
        /// <summary>
        /// 开发模式
        /// env:MICRO_MODE,默认Test
        /// </summary>
        public ProductMode Mode
        {
            get
            {
                var env = "MICRO_MODE".Env<ProductMode?>(null);
                return env ?? _mode;
            }
            set => _mode = value;
        }

        private string _consulServer;

        /// <summary>
        /// Consul服务地址
        /// env:MICRO_CONSUL
        /// </summary>
        public string ConsulServer
        {
            get
            {
                var env = "MICRO_CONSUL".Env<string>();
                if (!string.IsNullOrWhiteSpace(env))
                    return env;
                if (!string.IsNullOrWhiteSpace(_consulServer))
                    return _consulServer;
                return Mode == ProductMode.Prod
                    ? "http://micro.icb/consul/"
                    : "http://192.168.0.231:8500";
            }
            set => _consulServer = value;
        }

        private string _consulToken;

        /// <summary>
        /// Consul令牌
        /// env:MICRO_CONSUL_TOKEN,默认空字符
        /// </summary>
        public string ConsulToken
        {
            get
            {
                var env = "MICRO_CONSUL_TOKEN".Env<string>();
                return env ?? _consulToken;
            }
            set => _consulToken = value;
        }
    }
}
