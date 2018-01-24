﻿namespace Acb.Payment.Interfaces
{
    /// <summary>
    /// 商家数据
    /// </summary>
    public interface IDataMerchant
    {
        /// <summary>
        /// 应用ID
        /// </summary>
        string AppId { get; set; }

        /// <summary>
        /// 签名类型
        /// </summary>
        string SignType { get; }

        /// <summary>
        /// 网关回发通知URL
        /// </summary>
        string NotifyUrl { get; set; }
    }
}
