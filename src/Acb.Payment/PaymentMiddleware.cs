using Acb.Core.Extensions;
using Acb.Payment.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Acb.Payment
{
    public static class PaymentMiddleware
    {
        /// <summary> 添加ICanPay </summary>
        /// <param name="services"></param>
        public static void AddPayment(this IServiceCollection services)
        {
            services.AddTransient<IGateways>(sp =>
            {
                var gateways = new GatewayManager();
                IDataMerchant merchant = "payment:alipay".Config<Gateways.Alipay.Merchant>();
                if (merchant != null && !string.IsNullOrWhiteSpace(merchant.AppId))
                {
                    gateways.Add(new Gateways.Alipay.AlipayGateway((Gateways.Alipay.Merchant)merchant));
                }
                merchant = "payment:micropay".Config<Gateways.MicroPay.Merchant>();
                if (merchant != null)
                {
                    gateways.Add(new Gateways.MicroPay.MicroPayGateway((Gateways.MicroPay.Merchant)merchant));
                }
                return gateways;
            });
        }
    }
}
