namespace Acb.Payment.Interfaces
{
    /// <summary> 取消支付 </summary>
    public interface IActionCancel : IActionBase
    {
        /// <summary> 生成取消支付参数 </summary>
        /// <param name="dataAction"></param>
        /// <returns></returns>
        IDataNotify BuildCancel(IDataAction dataAction);

        /// <summary> 初始化取消支付参数 </summary>
        /// <param name="dataAction"></param>
        void InitCancel(IDataAction dataAction);
    }
}
