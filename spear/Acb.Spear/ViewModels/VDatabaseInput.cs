using Acb.Spear.Contracts.Enums;

namespace Acb.Spear.ViewModels
{
    public class VDatabaseInput
    {
        /// <summary> 名称 </summary>
        public string Name { get; set; }
        /// <summary> 数据库类型 </summary>
        public ProviderType Provider { get; set; }
        /// <summary> 连接字符串 </summary>
        public string ConnectionString { get; set; }
    }
}
