using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace FGS_BE.Repo.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            // Find the web project root by locating FGS-BE.csproj
            var currentDir = AppDomain.CurrentDomain.BaseDirectory;
            var basePath = FindProjectRoot(currentDir);

            // Debug output (remove after confirming it works)
            Console.WriteLine($"Found project root: {basePath}");
            Console.WriteLine($"appsettings.json exists: {File.Exists(Path.Combine(basePath, "appsettings.json"))}");

            // Build configuration from the project root's appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Could not find 'DefaultConnection' in appsettings.json.");

            optionsBuilder.UseSqlServer(connectionString);

            // Enable logging for migration debugging (dev only)
            optionsBuilder.EnableSensitiveDataLogging()
                          .EnableDetailedErrors();

            return new ApplicationDbContext(optionsBuilder.Options);
        }

        private static string FindProjectRoot(string startPath)
        {
            var dir = new DirectoryInfo(startPath);
            while (dir != null)
            {
                var csprojPath = Path.Combine(dir.FullName, "FGS-BE.csproj");
                if (File.Exists(csprojPath))
                {
                    return dir.FullName;
                }
                dir = dir.Parent;
            }
            throw new InvalidOperationException("Could not locate FGS-BE.csproj in any parent directory. Ensure you're running migrations with the correct project structure.");
        }
    }
}