namespace Acb.Payment.Notify.Events
{
    public class PaymentSucceedEventArgs : PaymentEventArgs
    {
        /// <summary>
        /// 初始化支付成功网关事件数据
        /// </summary>
        /// <param name="gateway">支付网关</param>
        public PaymentSucceedEventArgs(DGateway gateway)
            : base(gateway)
        {
        }
    }
}
