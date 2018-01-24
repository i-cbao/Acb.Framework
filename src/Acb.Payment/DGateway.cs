using Acb.Payment.Enum;
using Acb.Payment.Helper;
using Acb.Payment.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Acb.Payment
{
    /// <summary>
    /// 支付网关抽象类
    /// </summary>
    public abstract class DGateway
    {
        public const string TRUE = "true";
        public const string FALSE = "false";
        public const string SUCCESS = "success";
        public const string FAILURE = "failure";
        public const string FAIL = "FAIL";
        public const string TIME_FORMAT = "yyyy-MM-dd HH:mm:ss";
        public const string TIMEFORMAT = "yyyyMMddHHmmss";

        private ActionType _actionType;

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        protected DGateway()
            : this(new GatewayData())
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="merchant">商户数据</param>
        protected DGateway(IDataMerchant merchant)
            : this(new GatewayData())
        {
            Merchant = merchant;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="merchant">商户数据</param>
        /// <param name="gatewayData">网关数据</param>
        protected DGateway(IDataMerchant merchant, GatewayData gatewayData)
        {
            Merchant = merchant;
            GatewayData = gatewayData;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="gatewayData">网关数据</param>
        protected DGateway(GatewayData gatewayData)
        {
            GatewayData = gatewayData;
        }

        #endregion

        #region 属性
        /// <summary>
        /// 订单数据
        /// </summary>
        public IDataOrder Order { get; set; }

        /// <summary>
        /// 商户数据
        /// </summary>
        public IDataMerchant Merchant { get; set; }

        /// <summary>
        /// 通知数据
        /// </summary>
        public IDataNotify Notify { get; set; }

        /// <summary>
        /// 网关的地址
        /// </summary>
        public abstract string GatewayUrl { get; set; }

        /// <summary>
        /// 网关的交易类型
        /// </summary>
        public TradeType TradeType { get; set; }

        /// <summary>
        /// 网关数据
        /// </summary>
        public GatewayData GatewayData { get; set; }

        /// <summary>
        /// 是否成功支付
        /// </summary>
        protected abstract bool IsSuccessPay { get; }

        /// <summary>
        /// 是否等待支付
        /// </summary>
        protected abstract bool IsWaitPay { get; }

        /// <summary>
        /// 需要验证的参数名称数组，用于识别不同的网关类型。
        /// 商户号(AppId)必须放第一位
        /// </summary>
        protected internal abstract string[] NotifyVerifyParameter { get; }
        #endregion

        #region 私有方法
        /// <summary>
        /// 验证参数
        /// </summary>
        /// <param name="instance">验证对象</param>
        private void ValidateParameter(object instance)
        {
            ValidateHelper.Validate(instance, new Dictionary<object, object>
            {
                {nameof(TradeType), TradeType },
                {nameof(ActionType), _actionType }
            });
        }

        /// <summary>
        /// 验证辅助
        /// </summary>
        /// <param name="dataAction">辅助参数</param>
        private void ValidateAction(IDataAction dataAction)
        {
            if (dataAction is null)
            {
                throw new ArgumentNullException(nameof(dataAction));
            }

            ValidateParameter(dataAction);
            ValidateParameter(Merchant);
        }

        /// <summary>
        /// 验证订单
        /// </summary>
        /// <param name="order">订单</param>
        private void ValidateOrder(IDataOrder order)
        {
            if (order is null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            ValidateParameter(order);
            ValidateParameter(Merchant);
            Order = order;
        }

        #endregion

        #region 抽象方法
        /// <summary>
        /// 检验网关返回的通知，确认订单是否支付成功
        /// </summary>
        protected internal abstract Task<bool> ValidateNotifyAsync();

        /// <summary>
        /// 当接收到支付网关通知并验证无误时按照支付网关要求格式输出表示成功接收到网关通知的字符串
        /// </summary>
        protected internal virtual void WriteSuccessFlag()
        {
            HttpHelper.Write(SUCCESS);
        }

        /// <summary>
        /// 当接收到支付网关通知并验证有误时按照支付网关要求格式输出表示失败接收到网关通知的字符串
        /// </summary>
        protected internal virtual void WriteFailureFlag()
        {
            HttpHelper.Write(FAILURE);
        }

        #endregion

        #region 公共方法
        /// <summary>
        /// 支付
        /// </summary>
        /// <param name="order">订单</param>
        /// <returns></returns>
        public string Payment(IDataOrder order)
        {
            ValidateOrder(order);

            switch (TradeType)
            {
                case TradeType.App:
                    if (this is IPaymentApp appPayment)
                    {
                        return appPayment.BuildAppPayment();
                    }
                    break;
                case TradeType.Wap:
                    if (this is IPaymentUrl urlPayment)
                    {
                        HttpHelper.Redirect(urlPayment.BuildUrlPayment());
                        return null;
                    }
                    break;
                case TradeType.Web:
                    if (this is IPaymentForm formPayment)
                    {
                        HttpHelper.Write(formPayment.BuildFormPayment());
                        return null;
                    }
                    break;
                case TradeType.Scan:
                    if (this is IPaymentScan scanPayment)
                    {
                        return scanPayment.BuildScanPayment();
                    }
                    break;
                case TradeType.Public:
                    if (this is IPaymentPublic publicPayment)
                    {
                        return publicPayment.BuildPublicPayment();
                    }
                    break;
                case TradeType.Barcode:
                    if (this is IPaymentBarcode barcodePayment)
                    {
                        barcodePayment.BuildBarcodePayment();
                        return null;
                    }
                    break;
                case TradeType.Applet:
                    if (this is IPaymentApplet appletPayment)
                    {
                        return appletPayment.BuildAppletPayment();
                    }
                    break;
                default:
                    break;
            }
            throw new NotSupportedException($"{GetType()} 没有实现 {TradeType} 接口");
        }
        #endregion
    }
}
