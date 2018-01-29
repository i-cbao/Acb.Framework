namespace Acb.Payment.Interfaces
{
    /// <summary> 退款查询接口 </summary>
    public interface IActionRefundQuery : IActionBase
    {
        /// <summary> 生成退款查询参数 </summary>
        /// <param name="dataAction"></param>
        /// <returns></returns>
        IDataNotify BuildRefundQuery(IDataAction dataAction);

        /// <summary> 初始化退款查询参数 </summary>
        /// <param name="dataAction"></param>
        void InitRefundQuery(IDataAction dataAction);
    }
}
