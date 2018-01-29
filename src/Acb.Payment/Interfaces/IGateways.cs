using Acb.Core.Dependency;
using Acb.Payment.Enum;
using System.Collections.Generic;

namespace Acb.Payment.Interfaces
{
    public interface IGateways : ILifetimeDependency
    {
        /// <summary>
        /// 添加网关
        /// </summary>
        /// <param name="gateway">网关</param>
        /// <returns></returns>
        bool Add(DGateway gateway);

        /// <summary>
        /// 获取指定网关
        /// </summary>
        /// <typeparam name="T">网关类型</typeparam>
        /// <returns></returns>
        T Get<T>() where T : DGateway;

        /// <summary>
        /// 通过交易类型获取网关
        /// </summary>
        /// <typeparam name="T">网关类型</typeparam>
        /// <param name="tradeType">网关交易类型</param>
        /// <returns></returns>
        T Get<T>(TradeType tradeType) where T : DGateway;

        /// <summary>
        /// 获取网关列表
        /// </summary>
        /// <returns></returns>
        ICollection<DGateway> GetList();
    }
}
