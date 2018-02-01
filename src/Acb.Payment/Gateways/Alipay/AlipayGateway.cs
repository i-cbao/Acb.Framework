using Acb.Core.Exceptions;
using Acb.Core.Helper;
using Acb.Core.Serialize;
using Acb.Payment.Enum;
using Acb.Payment.Interfaces;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Acb.Payment.Gateways.Alipay
{
    /// <summary>
    /// 支付宝网关
    /// </summary>
    public sealed class AlipayGateway : DGateway, IPaymentApp, IPaymentUrl, IPaymentForm, IActionQuery, IActionCancel,
        IActionRefund
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
            Constant.APP_ID, Constant.NOTIFY_TYPE, Constant.NOTIFY_ID,
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

            return EncryptHelper.RsaVerifySign(GatewayData.ToUrl(false), Notify.Sign, Merchant.AlipayPublicKey,
                signType: "RSA2");
        }

        /// <summary>
        /// 初始化辅助接口的参数
        /// </summary>
        /// <param name="actionType">辅助类型</param>
        /// <param name="dataAction">辅助参数</param>
        private void InitActionParameter(ActionType actionType, IDataAction dataAction)
        {
            dataAction.Validate(actionType);
            switch (actionType)
            {
                case ActionType.Query:
                    Merchant.Method = Constant.QUERY;
                    break;
                case ActionType.Close:
                    Merchant.Method = Constant.CLOSE;
                    break;
                case ActionType.Cancel:
                    Merchant.Method = Constant.CANCEL;
                    break;
                case ActionType.Refund:
                    Merchant.Method = Constant.REFUND;
                    break;
                case ActionType.RefundQuery:
                    Merchant.Method = Constant.REFUNDQUERY;
                    break;
            }

            Merchant.BizContent = JsonHelper.ToJson((DataAction)dataAction);
            GatewayData.Add(Merchant, NamingType.UrlCase);
            GatewayData.Add(Constant.SIGN, BuildSign());
        }

        /// <summary>
        /// 提交请求
        /// </summary>
        /// <param name="type">结果类型</param>
        private void Commit(string type)
        {
            string result = null;
            Task.Run(async () => { result = await Helper.HttpHelper.PostAsync(GatewayUrl, GatewayData.ToUrl()); })
                .GetAwaiter()
                .GetResult();

            ReadReturnResult(result, type);
        }

        /// <summary>
        /// 读取返回结果
        /// </summary>
        /// <param name="result">结果</param>
        /// <param name="key">结果的对象名</param>
        private void ReadReturnResult(string result, string key)
        {
            GatewayData.FromJson(result);
            var sign = GatewayData.GetValue<string>(Constant.SIGN);
            result = GatewayData.GetValue<string>(key);
            GatewayData.FromJson(result);
            base.Notify = GatewayData.ToObject<Notify>(NamingType.UrlCase);
            Notify.Sign = sign;
            IsSuccessReturn();
        }

        /// <summary>
        /// 是否是已成功的返回
        /// </summary>
        /// <returns></returns>
        private bool IsSuccessReturn()
        {
            if (Notify.Code != "10000")
            {
                throw new BusiException(Notify.SubMessage);
            }

            return true;
        }

        #endregion

        protected internal override async Task<bool> ValidateNotifyAsync()
        {
            base.Notify = await GatewayData.ToObjectAsync<Notify>(NamingType.UrlCase);
            return IsSuccessResult();
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

        #region 退款

        public IDataNotify BuildRefund(IDataAction dataAction)
        {
            InitRefund(dataAction);

            Commit(Constant.ALIPAY_TRADE_REFUND_RESPONSE);

            return Notify;
        }

        public void InitRefund(IDataAction dataAction)
        {
            InitActionParameter(ActionType.Refund, dataAction);
        }

        #endregion

        #region 查询

        public IDataNotify BuildQuery(IDataAction dataAction)
        {
            InitQuery(dataAction);

            Commit(Constant.ALIPAY_TRADE_QUERY_RESPONSE);

            return Notify;
        }

        public void InitQuery(IDataAction dataAction)
        {
            InitActionParameter(ActionType.Query, dataAction);
        }

        #endregion

        #region 取消支付

        public IDataNotify BuildCancel(IDataAction dataAction)
        {
            InitCancel(dataAction);

            Commit(Constant.ALIPAY_TRADE_CANCEL_RESPONSE);

            return Notify;
        }

        public void InitCancel(IDataAction dataAction)
        {
            InitActionParameter(ActionType.Cancel, dataAction);
        }

        #endregion
    }
}
