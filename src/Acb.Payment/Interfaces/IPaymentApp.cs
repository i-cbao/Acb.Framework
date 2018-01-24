namespace Acb.Payment.Interfaces
{
    /// <summary>
    /// App支付
    /// </summary>
    public interface IPaymentApp : IPayment
    {
        /// <summary>
        /// 生成App支付参数
        /// </summary>
        string BuildAppPayment();

        /// <summary>
        /// 初始化App支付参数
        /// </summary>
        void InitAppPayment();
    }
}
