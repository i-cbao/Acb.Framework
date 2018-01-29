namespace Acb.Payment.Interfaces
{
    /// <summary> 关闭支付 </summary>
    public interface IActionClose : IActionBase
    {
        /// <summary> 生成关闭支付参数 </summary>
        /// <param name="dataAction"></param>
        /// <returns></returns>
        IDataNotify BuildClose(IDataAction dataAction);

        /// <summary> 初始化关闭支付参数 </summary>
        /// <param name="dataAction"></param>
        void InitClose(IDataAction dataAction);
    }
}
