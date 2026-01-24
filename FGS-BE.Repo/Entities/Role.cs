using Microsoft.AspNetCore.Identity;

namespace FGS_BE.Repo.Entities;

public class Role : IdentityRole<int>
{
    public Role()
    {
    }

    public Role(string roleName) : this()
    {
        Name = roleName;
    }
    public virtual ICollection<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();
}