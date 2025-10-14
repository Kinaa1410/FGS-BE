using Microsoft.AspNetCore.Identity;

namespace FGS_BE.Repo.Entities;
public class UserRole : IdentityUserRole<int>
{
    public virtual Role Role { get; set; } = default!;

    public virtual User User { get; set; } = default!;

}