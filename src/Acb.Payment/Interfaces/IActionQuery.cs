namespace Acb.Payment.Interfaces
{
    /// <summary> 查询订单接口 </summary>
    public interface IActionQuery : IActionBase
    {
        /// <summary> 生成查询参数 </summary>
        /// <param name="dataAction"></param>
        /// <returns></returns>
        IDataNotify BuildQuery(IDataAction dataAction);

        /// <summary> 初始化查询参数 </summary>
        /// <param name="dataAction"></param>
        void InitQuery(IDataAction dataAction);
    }
}
