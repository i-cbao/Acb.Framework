using System;
using Acb.Core.Domain.Entities;
using Acb.Core.Serialize;

namespace Acb.Spear.Business.Domain.Entities
{
    ///<summary> 数据库连接表 </summary>
    [Naming("t_database")]
    public class TDatabase : BaseEntity<Guid>
    {
        ///<summary> Id </summary>
        public override Guid Id { get; set; }

        ///<summary> 帐号ID </summary>
        public Guid AccountId { get; set; }

        ///<summary> 名称 </summary>
        public string Name { get; set; }

        ///<summary> 编码 </summary>
        public string Code { get; set; }

        ///<summary> 数据提供者,mysql,postgresql,sqlserver等 </summary>
        public string Provider { get; set; }

        ///<summary> 数据库连接字符(需要具有权限的数据库连接) </summary>
        public string ConnectionString { get; set; }

        ///<summary> 状态 </summary>
        public byte State { get; set; }

        ///<summary> 创建时间 </summary>
        public DateTime CreateTime { get; set; }
    }
}
