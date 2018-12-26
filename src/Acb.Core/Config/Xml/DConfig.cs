using System;
using System.Xml.Serialization;

namespace Acb.Core.Config.Xml
{
    /// <summary> 产品模式 </summary>
    public enum ProductMode
    {
        /// <summary> 本地 </summary>
        Local,

        /// <summary> 测试服务器 </summary>
        Test,

        /// <summary> 预发布 </summary>
        Ready,

        /// <summary> 正式 </summary>
        Production
    }

    /// <summary> 通用配置 </summary>
    [Serializable]
    [XmlRoot("root")]
    [FileName("base.config")]
    public class DConfig : ConfigBase
    {
        /// <summary> 日志服务器地址 </summary>
        [XmlElement("log-server")]
        public string LogServerUrl { get; set; }

        /// <summary> 日志服务器ApiKey </summary>
        [XmlElement("log-key")]
        public string LogApiKey { get; set; }

        /// <summary> 产品模式 </summary>
        [XmlAttribute("mode")]
        public ProductMode Mode { get; set; }
    }
}
