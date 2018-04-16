using Acb.Core.Extensions;
using Acb.Payment.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Acb.Payment
{
    /// <summary> 支付中间件 </summary>
    public static class PaymentMiddleware
    {
        private const string ConfigRegion = "payment:";
        private const string Alipay = "alipay";
        private const string Micropay = "micropay";

        /// <summary> 添加ICanPay </summary>
        /// <param name="services"></param>
        public static void AddPayment(this IServiceCollection services)
        {
            services.AddTransient<IGateways>(sp => Gateways());
        }

        public static GatewayManager Gateways()
        {
            var gateways = new GatewayManager();
            IDataMerchant merchant = (ConfigRegion + Alipay).Config<Gateways.Alipay.Merchant>();
            if (merchant != null && !string.IsNullOrWhiteSpace(merchant.AppId))
            {
                gateways.Add(new Gateways.Alipay.AlipayGateway((Gateways.Alipay.Merchant)merchant));
            }
            merchant = (ConfigRegion + Micropay).Config<Gateways.MicroPay.Merchant>();
            if (merchant != null)
            {
                gateways.Add(new Gateways.MicroPay.MicroPayGateway((Gateways.MicroPay.Merchant)merchant));
            }
            return gateways;
        }
    }
}
