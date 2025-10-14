using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Enums;
using FGS_BE.Repo.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Task = System.Threading.Tasks.Task;

namespace FGS_BE.Repo.Data.SeedData;
public class ApplicationDbContextInitialiser(
   ILogger<ApplicationDbContextInitialiser> logger,
   ApplicationDbContext context,
   UserManager<User> userManager,
   RoleManager<Role> roleManager,
   IUnitOfWork unitOfWork)
{
    public async Task MigrateAsync()
    {
        try
        {
            await context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    private async Task TrySeedAsync()
    {

        if (!await unitOfWork.Repository<Role>().ExistsByAsync())
        {
            foreach (var item in RoleSeed.Default)
            {
                await roleManager.CreateAsync(item);
            }
        }

        if (!await unitOfWork.Repository<User>().ExistsByAsync())
        {
            var user = new User
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com"
            };
            await userManager.CreateAsync(user, "admin@gmail.com");
            await userManager.AddToRolesAsync(user, [RoleEnums.Admin.ToString()]);

            user = new User
            {
                UserName = "user@gmail.com",
                Email = "user@gmail.com"
            };
            await userManager.CreateAsync(user, "user@gmail.com");
            await userManager.AddToRolesAsync(user, [RoleEnums.User.ToString()]);

            user = new User
            {
                UserName = "staff@gmail.com",
                Email = "staff@gmail.com",
            };
            await userManager.CreateAsync(user, "staff@gmail.com");
            await userManager.AddToRolesAsync(user, [RoleEnums.Staff.ToString()]);
        }

        await unitOfWork.CommitAsync();

    }
}