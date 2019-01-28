namespace Acb.Spear.Contracts.Dtos.Database
{
    /// <summary> 重命名接口 </summary>
    public interface IConvertedName
    {
        string Name { get; set; }
        string ConvertedName { get; }
    }
}
