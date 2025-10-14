using FGS_BE.Repo.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FGS_BE.Repo.Data.Configurations;
public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.HasOne(e => e.Role)
               .WithMany(e => e.UserRoles)
               .HasForeignKey(ur => ur.RoleId);

        builder.HasOne(e => e.User)
               .WithMany(e => e.UserRoles)
               .HasForeignKey(ur => ur.UserId);
    }
}