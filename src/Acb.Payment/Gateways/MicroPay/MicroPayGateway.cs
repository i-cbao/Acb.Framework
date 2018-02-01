using Acb.Core;
using Acb.Core.Exceptions;
using Acb.Core.Helper;
using Acb.Core.Serialize;
using Acb.Core.Timing;
using Acb.Payment.Enum;
using Acb.Payment.Interfaces;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using HttpHelper = Acb.Payment.Helper.HttpHelper;

namespace Acb.Payment.Gateways.MicroPay
{
    public class MicroPayGateway : DGateway, IPaymentApp, IPaymentUrl, IPaymentScan, IPaymentPublic, IActionRefund
    {
        #region 私有字段

        private readonly Merchant _merchant;
        private const string UNIFIEDORDERGATEWAYURL = "https://api.mch.weixin.qq.com/pay/unifiedorder";
        private const string QUERYGATEWAYURL = "https://api.mch.weixin.qq.com/pay/orderquery";
        private const string CANCELGATEWAYURL = "https://api.mch.weixin.qq.com/secapi/pay/reverse";
        private const string CLOSEORDERGATEWAYURL = "https://api.mch.weixin.qq.com/pay/closeorder";
        private const string REFUNDGATEWAYURL = "https://api.mch.weixin.qq.com/secapi/pay/refund";
        private const string REFUNDQUERYGATEWAYURL = "https://api.mch.weixin.qq.com/pay/refundquery";
        private const string DOWNLOADBILLGATEWAYURL = "https://api.mch.weixin.qq.com/pay/downloadbill";
        private const string REPORTGATEWAYURL = "https://api.mch.weixin.qq.com/payitil/report";
        private const string BATCHQUERYCOMMENTGATEWAYURL = "https://api.mch.weixin.qq.com/billcommentsp/batchquerycomment";
        private const string BARCODEGATEWAYURL = "https://api.mch.weixin.qq.com/pay/micropay";
        private const string ACCESSTOKENURL = "https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code";
        private const string AUTHCODETOOPENIDURL = "https://api.mch.weixin.qq.com/tools/authcodetoopenid";

        #endregion

        /// <summary>
        /// 初始化微信支付网关
        /// </summary>
        /// <param name="merchant">商户数据</param>
        public MicroPayGateway(Merchant merchant)
            : base(merchant)
        {
            _merchant = merchant;
        }

        #region 属性
        public override string GatewayUrl { get; set; } = UNIFIEDORDERGATEWAYURL;

        public new Merchant Merchant => _merchant;

        public new Order Order
        {
            get => (Order)base.Order;
            set => base.Order = value;
        }

        public new Notify Notify => (Notify)base.Notify;

        protected override bool IsSuccessPay => Notify.TradeState.ToLower() == SUCCESS;

        protected override bool IsWaitPay => Notify.TradeState.ToLower() == Constant.USERPAYING;

        protected internal override string[] NotifyVerifyParameter => new string[]
        { Constant.APPID, Constant.RETURN_CODE, Constant.MCH_ID, Constant.NONCE_STR, Constant.RESULT_CODE };

        protected internal override async Task<bool> ValidateNotifyAsync()
        {
            base.Notify = await GatewayData.ToObjectAsync<Notify>(NamingType.UrlCase);

            if (IsSuccessResult())
            {
                return true;
            }

            return false;
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 获得签名
        /// </summary>
        /// <returns></returns>
        private string BuildSign()
        {
            GatewayData.Remove(Constant.SIGN);
            string data = $"{GatewayData.ToUrl(false)}&key={Merchant.Key}";
            return EncryptHelper.MD5(data);
        }

        /// <summary>
        /// 初始化订单参数
        /// </summary>
        private void InitOrderParameter()
        {
            GatewayData.Clear();
            Merchant.NonceStr = IdentityHelper.Guid32;
            Merchant.DeviceInfo = Constant.WEB;
            GatewayData.Add(Merchant);
            GatewayData.Add(Order);
            GatewayData.Add(Constant.SIGN, BuildSign());
        }

        /// <summary>
        /// 统一下单
        /// </summary>
        /// <returns></returns>
        private void UnifiedOrder()
        {
            GatewayUrl = UNIFIEDORDERGATEWAYURL;
            InitOrderParameter();
            Commit();
        }

        /// <summary>
        /// 提交请求
        /// </summary>
        /// <param name="isCert">是否添加证书</param>
        private void Commit(bool isCert = false)
        {
            var cert = isCert ? new X509Certificate2(Merchant.SslCertPath, Merchant.SslCertPassword) : null;

            string result = null;
            Task.Run(async () =>
            {
                result = await HttpHelper.PostAsync(GatewayUrl, GatewayData.ToXml(), cert);
            })
            .GetAwaiter()
            .GetResult();
            ReadReturnResult(result);
        }

        /// <summary>
        /// 读取返回结果
        /// </summary>
        /// <param name="result"></param>
        private void ReadReturnResult(string result)
        {
            GatewayData.FromXml(result);
            base.Notify = GatewayData.ToObject<Notify>(NamingType.UrlCase);
            IsSuccessReturn();
        }

        /// <summary>
        /// 是否是已成功的返回
        /// </summary>
        /// <returns></returns>
        private bool IsSuccessReturn()
        {
            if (Notify.ReturnCode == FAIL)
            {
                throw new BusiException(Notify.ReturnMsg);
            }
            return true;
        }

        /// <summary>
        /// 通过code获取AccessToken
        /// </summary>
        /// <param name="code"></param>
        private OAuth GetAccessTokenByCode(string code)
        {
            string result = null;
            Task.Run(async () =>
            {
                result = await HttpHelper.GetAsync(string.Format(ACCESSTOKENURL, Merchant.AppId, Merchant.AppSecret, code));
            })
            .GetAwaiter()
            .GetResult();
            GatewayData.FromJson(result);

            int _code = GatewayData.GetValue<int>(Constant.ERRCODE);
            int _msg = GatewayData.GetValue<int>(Constant.ERRMSG);
            if (_code == 40029)
            {
                throw new BusiException($"{_code} {_msg}");
            }

            return GatewayData.ToObject<OAuth>(NamingType.UrlCase);
        }

        /// <summary>
        /// 是否是已成功支付的支付通知
        /// </summary>
        /// <returns></returns>
        private bool IsSuccessResult()
        {
            if (Notify.ReturnCode.ToLower() != SUCCESS)
            {
                throw new BusiException("不是成功的返回码");
            }

            if (Notify.Sign != BuildSign())
            {
                throw new BusiException("签名不一致");
            }

            if (Notify.ResultCode.ToLower() == SUCCESS)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 初始化辅助参数
        /// </summary>
        /// <param name="actionType">辅助类型</param>
        /// <param name="dataAction">辅助参数</param>
        private void InitActionParameter(ActionType actionType, IDataAction dataAction)
        {
            dataAction.Validate(actionType);
            Merchant.NonceStr = IdentityHelper.Guid32;
            GatewayData.Add(Merchant, NamingType.UrlCase);
            GatewayData.Add(dataAction, NamingType.UrlCase);
            GatewayData.Add(Constant.SIGN, BuildSign());
        }

        #endregion

        #region App支付
        /// <summary>
        /// 初始化APP端调起支付的参数
        /// </summary>
        private void InitAppParameter()
        {
            GatewayData.Clear();
            Merchant.NonceStr = IdentityHelper.Guid32;
            GatewayData.Add(Constant.APPID, Merchant.AppId);
            GatewayData.Add(Constant.PARTNERID, Merchant.MchId);
            GatewayData.Add(Constant.PREPAYID, Notify.PrepayId);
            GatewayData.Add(Constant.PACKAGE, "Sign=WXPay");
            GatewayData.Add(Constant.NONCE_STR, Merchant.NonceStr);
            GatewayData.Add(Constant.TIMESTAMP, Clock.Now.ToTimestamp());
            GatewayData.Add(Constant.SIGN, BuildSign());
        }

        public string BuildAppPayment()
        {
            InitAppPayment();
            InitAppParameter();
            return GatewayData.ToJson();
        }

        public void InitAppPayment()
        {
            Order.TradeType = Constant.APP;
            Order.SpbillCreateIp = AcbHttpContext.RemoteIpAddress;
            UnifiedOrder();
        }
        #endregion

        #region Url支付
        public string BuildUrlPayment()
        {
            InitUrlPayment();
            return Notify.MWebUrl;
        }

        public void InitUrlPayment()
        {
            Order.TradeType = Constant.MWEB;
            Order.SpbillCreateIp = AcbHttpContext.RemoteIpAddress;
            UnifiedOrder();
        }
        #endregion

        #region 公众号支付

        public string BuildPublicPayment()
        {
            InitPublicPayment();
            InitPublicParameter();
            return GatewayData.ToJson();
        }

        public void InitPublicPayment()
        {
            OAuth oAuth = GetAccessTokenByCode(Order.Code);
            Order.OpenId = oAuth.OpenId;
            Order.TradeType = Constant.JSAPI;
            Order.SpbillCreateIp = AcbHttpContext.RemoteIpAddress;
            UnifiedOrder();
        }

        /// <summary>
        /// 初始化公众号调起支付的参数
        /// </summary>
        private void InitPublicParameter()
        {
            GatewayData.Clear();
            Merchant.NonceStr = IdentityHelper.Guid32;
            GatewayData.Add(Constant.APPID, Merchant.AppId);
            GatewayData.Add(Constant.TIMESTAMP, Clock.Now.ToTimestamp());
            GatewayData.Add(Constant.NONCE_STR, Merchant.NonceStr);
            GatewayData.Add(Constant.PACKAGE, $"{Constant.PREPAY_ID}={Notify.PrepayId}");
            GatewayData.Add(Constant.SIGN_TYPE, "MD5");
            GatewayData.Add(Constant.PAYSIGN, BuildSign());
        }

        #endregion

        #region 扫码支付

        public string BuildScanPayment()
        {
            InitScanPayment();
            UnifiedOrder();
            return Notify.CodeUrl;
        }

        public void InitScanPayment()
        {
            Order.TradeType = Constant.NATIVE;
            Order.SpbillCreateIp = AcbHttpContext.LocalIpAddress;
        }

        #endregion


        #region 退款
        public IDataNotify BuildRefund(IDataAction dataAction)
        {
            InitRefund(dataAction);
            Commit(true);
            return Notify;
        }

        public void InitRefund(IDataAction dataAction)
        {
            GatewayUrl = REFUNDGATEWAYURL;
            InitActionParameter(ActionType.Refund, dataAction);
        }

        #endregion
    }
}
