namespace Asb.Spear.Client
{
    public class SpearOption
    {
        /// <summary> hub地址 </summary>
        public string HubAddress { get; set; }
        /// <summary> 项目编码 </summary>
        public string Code { get; set; }
        /// <summary> 项目密钥 </summary>
        public string Secret { get; set; }
        /// <summary> 配置模块 </summary>
        public string[] ConfigModules { get; set; }
        /// <summary> 开发模式：dev/test/ready/prod </summary>
        public string Mode { get; set; }
    }
}
