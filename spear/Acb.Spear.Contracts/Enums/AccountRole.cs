namespace Acb.Spear.Contracts.Enums
{
    public enum AccountRole : byte
    {
        Project = 1 << 0,
        Admin = 1 << 8 - 1
    }
}
