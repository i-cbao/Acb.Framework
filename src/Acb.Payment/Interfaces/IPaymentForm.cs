namespace Acb.Payment.Interfaces
{
    /// <summary>
    /// 表单支付
    /// </summary>
    public interface IPaymentForm : IPayment
    {
        /// <summary>
        /// 生成表单支付参数
        /// </summary>
        string BuildFormPayment();

        /// <summary>
        /// 初始化表单支付参数
        /// </summary>
        void InitFormPayment();
    }
}
