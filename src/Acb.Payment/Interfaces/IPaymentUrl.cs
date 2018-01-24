namespace Acb.Payment.Interfaces
{
    /// <summary>
    /// Url支付
    /// </summary>
    public interface IPaymentUrl : IPayment
    {
        /// <summary>
        /// 生成Url支付参数
        /// </summary>
        string BuildUrlPayment();

        /// <summary>
        /// 初始化Url支付参数
        /// </summary>
        void InitUrlPayment();
    }
}
