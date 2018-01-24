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

        public DGateway Get<T>()
        {
            var current = _list.Where(g => g is T).FirstOrDefault();
            if (current == null)
                throw new BusiException("找不到支付网关");
            return current;
        }

        public DGateway Get<T>(TradeType tradeType)
        {
            var gatewayList = _list
                .Where(a => a is T && a.TradeType == tradeType)
                .ToList();

            var gateway = gatewayList.Count > 0 ? gatewayList[0] : Get<T>();
            gateway.TradeType = tradeType;
            return gateway;
        }

        /// <summary>
        /// 指定AppId是否存在
        /// </summary>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        private bool Exist(string appId) => _list.Any(a => a.Merchant.AppId == appId);

        public ICollection<DGateway> GetList()
        {
            return _list;
        }
    }
}
