namespace Acb.Payment.Notify.Events
{
    public class PaymentFailedEventArgs : PaymentEventArgs
    {
        /// <summary>
        /// 初始化支付失败网关事件数据
        /// </summary>
        /// <param name="gateway">支付网关</param>
        public PaymentFailedEventArgs(DGateway gateway)
            : base(gateway)
        {
        }

        /// <summary>
        /// 支付失败信息
        /// </summary>
        public string Message { get; set; }
    }
}
