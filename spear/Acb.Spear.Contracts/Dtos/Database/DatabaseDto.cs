using Acb.Core.Domain.Dtos;
using Acb.Spear.Contracts.Enums;
using System;

namespace Acb.Spear.Contracts.Dtos.Database
{
    public class DatabaseDto : DDto
    {
        public Guid Id { get; set; }
        ///<summary> 名称 </summary>
        public string Name { get; set; }

        ///<summary> 数据提供者,mysql,postgresql,sqlserver等 </summary>
        public ProviderType Provider { get; set; }

        ///<summary> 数据库连接字符(需要具有权限的数据库连接) </summary>
        public string ConnectionString { get; set; }

        ///<summary> 状态 </summary>
        public CommonStatus Status { get; set; }

        ///<summary> 创建时间 </summary>
        public DateTime CreateTime { get; set; }
    }
}
