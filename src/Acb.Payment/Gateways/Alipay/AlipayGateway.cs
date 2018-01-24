using Acb.Core.Exceptions;
using Acb.Core.Helper;
using Acb.Core.Serialize;
using Acb.Payment.Interfaces;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Acb.Payment.Gateways.Alipay
{
    /// <summary>
    /// 支付宝网关
    /// </summary>
    public sealed class AlipayGateway : DGateway, IPaymentApp, IPaymentUrl, IPaymentForm
    {
        #region 私有字段
#if DEBUG
        private const string GATEWAYURL = "https://openapi.alipaydev.com/gateway.do?charset=UTF-8";
#else
        private const string GATEWAYURL = "https://openapi.alipay.com/gateway.do?charset=UTF-8";
#endif
        private readonly Merchant _merchant;

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化支付宝网关
        /// </summary>
        /// <param name="merchant">商户数据</param>
        public AlipayGateway(Merchant merchant)
            : base(merchant)
        {
            _merchant = merchant;
        }

        #endregion

        #region 属性
        public override string GatewayUrl { get; set; } = GATEWAYURL;

        public new Merchant Merchant => _merchant;

        public new Order Order
        {
            get => (Order)base.Order;
            set => base.Order = value;
        }

        public new Notify Notify => (Notify)base.Notify;

        protected override bool IsSuccessPay => Notify.TradeStatus == Constant.TRADE_SUCCESS;

        protected override bool IsWaitPay => Notify.TradeStatus == Constant.WAIT_BUYER_PAY;

        protected internal override string[] NotifyVerifyParameter => new string[]
        {
            Constant.APP_ID,Constant.NOTIFY_TYPE, Constant.NOTIFY_ID,
            Constant.NOTIFY_TIME, Constant.SIGN, Constant.SIGN_TYPE
        };
        #endregion

        #region 私有方法
        /// <summary>
        /// 生成签名
        /// </summary>
        private string BuildSign()
        {
            return EncryptHelper.RsaSignature(GatewayData.ToUrl(false), Merchant.Privatekey, Merchant.Charset, "RSA2");
        }

        /// <summary>
        /// 初始化订单参数
        /// </summary>
        private void InitOrderParameter()
        {
            Merchant.BizContent = JsonConvert.SerializeObject(Order, new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore
            });
            GatewayData.Add(Merchant, NamingType.UrlCase);
            GatewayData.Add(Constant.SIGN, BuildSign());
        }

        /// <summary>
        /// 是否是已成功支付的支付通知
        /// </summary>
        /// <returns></returns>
        private bool IsSuccessResult()
        {
            if (!ValidateNotifySign())
            {
                throw new BusiException("签名不一致");
            }

            if (IsSuccessPay)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 验证支付宝通知的签名
        /// </summary>
        private bool ValidateNotifySign()
        {
            GatewayData.Remove(Constant.SIGN);
            GatewayData.Remove(Constant.SIGN_TYPE);

            return EncryptHelper.RsaVerifySign(GatewayData.ToUrl(false), Notify.Sign, Merchant.AlipayPublicKey, signType: "RSA2");
        }

        #endregion

        protected internal override async Task<bool> ValidateNotifyAsync()
        {
            base.Notify = await GatewayData.ToObjectAsync<Notify>(NamingType.UrlCase);
            if (IsSuccessResult())
            {
                return true;
            }

            return false;
        }

        #region App支付
        public string BuildAppPayment()
        {
            InitAppPayment();

            return GatewayData.ToUrl();
        }

        public void InitAppPayment()
        {
            Merchant.Method = Constant.APP;
            Order.ProductCode = Constant.QUICK_MSECURITY_PAY;

            InitOrderParameter();
        }

        #endregion

        #region Web支付
        public string BuildFormPayment()
        {
            InitFormPayment();

            return GatewayData.ToForm(GatewayUrl);
        }

        public void InitFormPayment()
        {
            Merchant.Method = Constant.WEB;
            Order.ProductCode = Constant.FAST_INSTANT_TRADE_PAY;
            InitOrderParameter();
        }
        #endregion

        #region Wap支付
        public string BuildUrlPayment()
        {
            InitUrlPayment();
            var param = GatewayData.ToUrl();
            return $"{GatewayUrl}&{param}";
        }

        public void InitUrlPayment()
        {
            Merchant.Method = Constant.WAP;
            Order.ProductCode = Constant.QUICK_WAP_WAY;
            InitOrderParameter();
        }
        #endregion
    }
}
