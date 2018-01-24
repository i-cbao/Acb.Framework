using System.Threading.Tasks;

namespace Acb.Payment
{
    /// <summary>
    /// 空支付网关
    /// </summary>
    public class NullGateway : DGateway
    {
        public override string GatewayUrl { get; set; } = string.Empty;

        protected override bool IsSuccessPay => false;

        protected override bool IsWaitPay => false;

        protected internal override string[] NotifyVerifyParameter => new string[0];

        protected internal override async Task<bool> ValidateNotifyAsync()
        {
            return await Task.Run(() => { return false; });
        }
    }
}
