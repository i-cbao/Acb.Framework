namespace Acb.Spear.Domain.Enums
{
    public enum AccountRole : short
    {
        Project = 1 << 0,
        Admin = 1 << 8
    }
}
