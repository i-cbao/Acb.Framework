namespace Acb.Payment.Interfaces
{
    /// <summary>
    /// 订单数据
    /// </summary>
    public interface IDataOrder
    {
        /// <summary>
        /// 商户订单号
        /// </summary>
        string OutTradeNo { get; set; }

        /// <summary>
        /// 金额,单位元
        /// </summary>
        double Amount { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        string Body { get; set; }
    }
}
