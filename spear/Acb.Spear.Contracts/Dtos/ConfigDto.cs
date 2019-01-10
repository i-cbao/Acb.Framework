using Acb.Core.Domain.Dtos;
using Newtonsoft.Json;
using System;
using Acb.Spear.Contracts.Enums;

namespace Acb.Spear.Contracts.Dtos
{
    public class ConfigDto : DDto
    {
        ///<summary> Id </summary>
        public Guid Id { get; set; }

        ///<summary> 名称 </summary>
        public string Name { get; set; }

        ///<summary> 模式:Dev,Test,Ready,Prod... </summary>
        public string Mode { get; set; }

        ///<summary> 内容 </summary>
        public string Content { get; set; }

        public object Config => JsonConvert.DeserializeObject(Content);

        ///<summary> 项目编码 </summary>
        public Guid ProjectId { get; set; }

        ///<summary> 配置校验码 </summary>
        public string Md5 { get; set; }

        ///<summary> 描述 </summary>
        public string Desc { get; set; }

        ///<summary> 状态:0,正常;1,历史版本;2,已删除 </summary>
        public ConfigStatus Status { get; set; }

        ///<summary> 创建时间 </summary>
        public DateTime Timestamp { get; set; }
    }
}
