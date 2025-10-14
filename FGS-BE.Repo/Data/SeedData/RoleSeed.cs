using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Enums;

namespace FGS_BE.Repo.Data.SeedData;
internal static class RoleSeed
{
    public static IList<Role> Default => new List<Role>()
    {
        new(RoleEnums.Admin.ToString()),
        new(RoleEnums.User.ToString()),
        new(RoleEnums.Staff.ToString()),
    };
}