﻿namespace Acb.Payment.Interfaces
{
    /// <summary>
    /// 扫码支付
    /// </summary>
    public interface IPaymentScan : IPayment
    {
        /// <summary>
        /// 生成扫码支付参数
        /// </summary>
        string BuildScanPayment();

        /// <summary>
        /// 初始化扫码支付参数
        /// </summary>
        void InitScanPayment();
    }
}
