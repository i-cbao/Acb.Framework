using Acb.Core.Helper;
using Acb.Core.Serialize;
using Acb.Core.Timing;
using Acb.Payment.Enum;
using Acb.Payment.Gateways.Alipay;
using Acb.Payment.Interfaces;
using Acb.Payment.Notify;
using Acb.Payment.Notify.Events;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Acb.WebApi.Test.Controllers
{
    /// <summary> 支付相关接口 </summary>
    [Route("api/payment")]
    public class PaymentController : DController
    {
        private readonly IGateways _gateways;
        private readonly string _outTradeNo = Clock.Now.ToString("yyyyMMddhhmmss");

        public PaymentController(IGateways gateways)
        {
            _gateways = gateways;
        }

        /// <summary>
        /// 支付宝                 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet, Route("alipay")]
        public IActionResult Alipay(TradeType type = TradeType.Web)
        {
            var order = new Order
            {
                Amount = 0.01,
                OutTradeNo = _outTradeNo,
                Subject = "测测看支付宝",
                //AuthCode = "12323",
                //Scene = Alipay.Constant.BAR_CODE
                //Body = "1234",
                //ExtendParams = new ExtendParam()
                //{
                //    HbFqNum = "3"
                //},
                //GoodsDetail = new Goods[] {
                //    new Goods()
                //    {
                //        Id = "12"
                //    }
                //}
            };

            var gateway = _gateways.Get<AlipayGateway>(type);
            //(gateway as AlipayGateway).BuildRefund(new DataAction());

            //gateway.PaymentFailed += Gateway_BarcodePaymentFailed;
            var result = gateway.Payment(order);
            return Content(result);
        }

        /// <summary> 微信支付 </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet, Route("micropay")]
        public IActionResult MicroPay(TradeType type = TradeType.Wap)
        {
            var sceneInfo = new
            {
                h5_info = new
                {
                    type = "h5",
                    wap_url = "http://localhost:5000",
                    wap_name = "h5支付"
                }
            };
            var order = new Payment.Gateways.MicroPay.Order()
            {
                Amount = 0.01,
                OutTradeNo = _outTradeNo,
                Body = "测测看微信支付",
                SceneInfo = JsonHelper.ToJson(sceneInfo),
                ProductId = IdentityHelper.Guid32
                //AuthCode = "123"
            };

            var gateway = _gateways.Get<Payment.Gateways.MicroPay.MicroPayGateway>(type);

            //gateway.PaymentFailed += Gateway_BarcodePaymentFailed;

            return Content(gateway.Payment(order));
        }

        /// <summary> 支付通知 </summary>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public async Task Notify()
        {
            // 订阅支付通知事件
            var notify = new PaymentNotify(_gateways);
            notify.PaymentSucceed += Notify_PaymentSucceed;
            notify.PaymentFailed += Notify_PaymentFailed;
            notify.UnknownGateway += Notify_UnknownGateway;

            // 接收并处理支付通知
            await notify.ReceivedAsync();
        }

        private void Notify_UnknownGateway(object arg1, PaymentUnknowEventArgs arg2)
        {
            throw new NotImplementedException();
        }

        private void Notify_PaymentFailed(object arg1, PaymentFailedEventArgs arg2)
        {
            throw new NotImplementedException();
        }

        private void Notify_PaymentSucceed(object arg1, PaymentSucceedEventArgs arg2)
        {
            throw new NotImplementedException();
        }
    }
}