using System;
using Acb.Core.Extensions;
using Acb.Spear.Contracts.Enums;

namespace Acb.Spear.ViewModels
{
    public class VDatabase
    {
        public Guid Id { get; set; }
        ///<summary> 名称 </summary>
        public string Name { get; set; }

        ///<summary> 数据提供者,mysql,postgresql,sqlserver等 </summary>
        public ProviderType Provider { get; set; }

        /// <summary> 数据提供者描述 </summary>
        public string ProviderCn => Provider.GetText();

        ///<summary> 数据库连接字符(需要具有权限的数据库连接) </summary>
        public string ConnectionString { get; set; }

        ///<summary> 状态 </summary>
        public CommonStatus Status { get; set; }

        /// <summary> 状态描述 </summary>
        public string StatusCn => Status.GetText();

        ///<summary> 创建时间 </summary>
        public DateTime CreateTime { get; set; }
    }
}
