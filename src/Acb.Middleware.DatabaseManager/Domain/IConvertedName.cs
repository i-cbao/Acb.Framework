namespace Acb.Middleware.DatabaseManager.Domain
{
    /// <summary> 重命名接口 </summary>
    public interface IConvertedName
    {
        string Name { get; set; }
        string ConvertedName { get; set; }
    }
}
