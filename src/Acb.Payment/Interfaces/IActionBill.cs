namespace Acb.Payment.Interfaces
{
    /// <summary> 账单下载 </summary>
    public interface IActionBill : IActionBase
    {
        /// <summary> 生成账单下载参数 </summary>
        /// <param name="dataAction"></param>
        /// <returns></returns>
        IDataNotify BuildBill(IDataAction dataAction);

        /// <summary> 初始化账单下载参数 </summary>
        /// <param name="dataAction"></param>
        void InitBill(IDataAction dataAction);
    }
}
