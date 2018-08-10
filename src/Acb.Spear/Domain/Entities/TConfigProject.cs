using Acb.Core.Domain.Entities;
using Acb.Core.Serialize;

namespace Acb.Spear.Domain.Entities
{
    /// <summary> 配置项目表 </summary>
    [Naming("t_config_project")]
    public class TConfigProject : BaseEntity<string>
    {
        /// <summary> 名称 </summary>
        public string Name { get; set; }
        /// <summary> 编码 </summary>
        public string Code { get; set; }
        /// <summary> 安全等级 </summary>
        public byte Security { get; set; }
        /// <summary> 帐号 </summary>
        public string Account { get; set; }
        /// <summary> 密码 </summary>
        public string Password { get; set; }
        /// <summary> 描述 </summary>
        public string Desc { get; set; }
    }
}
