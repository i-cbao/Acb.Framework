namespace Acb.Payment.Enum
{
    /// <summary>
    /// 网关操作类型
    /// </summary>
    public enum ActionType
    {
        /// <summary>
        /// 无操作
        /// </summary>
        NoAction,

        /// <summary>
        /// 查询
        /// </summary>
        Query,

        /// <summary>
        /// 关闭
        /// </summary>
        Close,

        /// <summary>
        /// 撤销
        /// </summary>
        Cancel,

        /// <summary>
        /// 退款
        /// </summary>
        Refund,

        /// <summary>
        /// 退款查询
        /// </summary>
        RefundQuery,

        /// <summary>
        /// 对账单下载
        /// </summary>
        BillDownload
    }
}
