using Acb.Core.Exceptions;
using Acb.Payment.Enum;
using Acb.Payment.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Acb.Payment
{
    public class GatewayManager : IGateways
    {
        private readonly ICollection<DGateway> _list;

        /// <summary>
        /// 网关数量
        /// </summary>
        public int Count => _list.Count;

        public GatewayManager()
        {
            _list = new List<DGateway>();
        }

        /// <summary> 添加支付网关 </summary>
        /// <param name="gateway"></param>
        /// <returns></returns>
        public bool Add(DGateway gateway)
        {
            if (gateway != null)
            {
                if (!Exist(gateway.Merchant.AppId))
                {
                    _list.Add(gateway);
                    return true;
                }
                else
                {
                    throw new BusiException("该商户数据已存在");
                }
            }

            return false;
        }

        /// <inheritdoc />
        /// <summary> 获取支付网关 </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>() where T : DGateway
        {
            var current = _list.FirstOrDefault(g => g is T);
            if (current == null)
                throw new BusiException("找不到支付网关");
            return (T)current;
        }

        /// <inheritdoc />
        /// <summary> 获取支付网关 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tradeType"></param>
        /// <returns></returns>
        public T Get<T>(TradeType tradeType) where T : DGateway
        {
            var gatewayList = _list
                .Where(a => a is T && a.TradeType == tradeType)
                .ToList();

            var gateway = gatewayList.Count > 0 ? gatewayList[0] : Get<T>();
            gateway.TradeType = tradeType;
            return (T)gateway;
        }

        /// <summary> 指定AppId是否存在 </summary>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        private bool Exist(string appId) => _list.Any(a => a.Merchant.AppId == appId);

        /// <summary> 支付网关列表 </summary>
        /// <returns></returns>
        public ICollection<DGateway> GetList()
        {
            return _list;
        }
    }
}
