namespace Acb.Payment.Interfaces
{
    /// <summary> 退款接口 </summary>
    public interface IActionRefund : IActionBase
    {
        /// <summary> 生成退款参数 </summary>
        /// <param name="dataAction"></param>
        /// <returns></returns>
        IDataNotify BuildRefund(IDataAction dataAction);

        /// <summary> 初始化退款参数 </summary>
        /// <param name="dataAction"></param>
        void InitRefund(IDataAction dataAction);
    }
}
