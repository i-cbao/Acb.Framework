using Acb.Core;
using Acb.Core.Exceptions;
using Acb.Core.Logging;
using Acb.Payment.Interfaces;
using Acb.Payment.Notify.Events;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Acb.Payment.Notify
{
    public class PaymentNotify
    {
        #region 私有字段

        private readonly IGateways _gateways;
        private readonly ILogger _logger = LogManager.Logger<PaymentNotify>();

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化支付通知
        /// </summary>
        /// <param name="gateways">用于验证支付网关返回数据的网关列表</param>
        public PaymentNotify(IGateways gateways)
        {
            _gateways = gateways;
        }

        #endregion

        #region 事件

        /// <summary>
        /// 网关异步返回的支付通知验证失败时触发
        /// </summary>
        public event Action<object, PaymentFailedEventArgs> PaymentFailed;

        /// <summary>
        /// 网关异步返回的支付通知验证成功时触发
        /// </summary>
        public event Action<object, PaymentSucceedEventArgs> PaymentSucceed;

        /// <summary>
        /// 网关异步返回的支付通知无法识别时触发
        /// </summary>
        public event Action<object, PaymentUnknowEventArgs> UnknownGateway;

        #endregion

        #region 私有方法
        private void OnPaymentFailed(PaymentFailedEventArgs e) => PaymentFailed?.Invoke(this, e);

        private void OnPaymentSucceed(PaymentSucceedEventArgs e) => PaymentSucceed?.Invoke(this, e);

        private void OnUnknownGateway(PaymentUnknowEventArgs e) => UnknownGateway?.Invoke(this, e);

        /// <summary>
        /// 是否是Xml格式数据
        /// </summary>
        /// <returns></returns>
        private static bool IsXmlData => AcbHttpContext.ContentType == "text/xml" || AcbHttpContext.ContentType == "application/xml";

        /// <summary>
        /// 是否是GET请求
        /// </summary>
        /// <returns></returns>
        private static bool IsGetRequest => AcbHttpContext.RequestType == "GET";

        private DGateway GetGateway()
        {
            var gatewayData = ReadNotifyData();
            _logger.Info($"orginal:{gatewayData.OriginalData}");
            _logger.Info($"json:{gatewayData.ToJson()}");
            DGateway gateway = null;

            foreach (var item in _gateways.GetList())
            {
                if (ExistParameter(item.NotifyVerifyParameter, gatewayData))
                {
                    if (item.Merchant.AppId == gatewayData
                        .GetValue<string>(item.NotifyVerifyParameter.FirstOrDefault()))
                    {
                        gateway = item;
                        break;
                    }
                }
            }

            if (gateway is null)
            {
                gateway = new NullGateway();
            }

            gateway.GatewayData = gatewayData;
            return gateway;
        }

        /// <summary>
        /// 读取网关发回的数据
        /// </summary>
        /// <returns></returns>
        public static GatewayData ReadNotifyData()
        {
            var gatewayData = new GatewayData();
            if (IsGetRequest)
            {
                gatewayData.FromUrl(AcbHttpContext.QueryString);
            }
            else
            {
                if (IsXmlData)
                {
                    var reader = new StreamReader(AcbHttpContext.Body);
                    string xmlData = reader.ReadToEnd();
                    reader.Dispose();
                    gatewayData.FromXml(xmlData);
                }
                else
                {
                    try
                    {
                        gatewayData.FromForm(AcbHttpContext.Form);
                    }
                    catch { }
                }
            }

            return gatewayData;
        }

        /// <summary>
        /// 网关参数数据项中是否存在指定的所有参数名
        /// </summary>
        /// <param name="parmaName">参数名数组</param>
        /// <param name="gatewayData">网关数据</param>
        private static bool ExistParameter(string[] parmaName, GatewayData gatewayData)
        {
            int compareCount = 0;
            foreach (var item in parmaName)
            {
                if (gatewayData.Exists(item))
                {
                    compareCount++;
                }
            }

            if (compareCount == parmaName.Length)
            {
                return true;
            }

            return false;
        }

        #endregion

        /// <summary>
        /// 接收并验证网关的支付通知
        /// </summary>
        public async Task ReceivedAsync()
        {
            DGateway gateway = GetGateway();
            if (gateway is NullGateway)
            {
                OnUnknownGateway(new PaymentUnknowEventArgs(gateway));
            }
            else
            {
                try
                {
                    if (await gateway.ValidateNotifyAsync())
                    {
                        OnPaymentSucceed(new PaymentSucceedEventArgs(gateway));
                        gateway.WriteSuccessFlag();
                    }
                    else
                    {
                        OnPaymentFailed(new PaymentFailedEventArgs(gateway));
                        gateway.WriteFailureFlag();
                    }
                }
                catch (BusiException ex)
                {
                    OnPaymentFailed(new PaymentFailedEventArgs(gateway)
                    {
                        Message = ex.Message
                    });
                    gateway.WriteFailureFlag();
                }
            }
        }


    }
}
