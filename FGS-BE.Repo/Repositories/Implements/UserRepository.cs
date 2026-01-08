using FGS_BE.Repo.Data;
using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace FGS_BE.Repo.Repositories.Implements
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<PaginatedList<User>> GetPagedAsync(
            int pageIndex,
            int pageSize,
            string? keyword = null,
            string? sortColumn = "Id",
            string? sortDir = "Asc",
            CancellationToken cancellationToken = default)
        {
            var query = Entities.AsNoTracking(); // Read-only for efficiency

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(x => x.FullName!.Contains(keyword) || x.Email!.Contains(keyword) || x.StudentCode!.Contains(keyword));
            }

            var order = $"{sortColumn} {sortDir}";
            query = query.OrderBy(order);

            return await query.PaginatedListAsync(pageIndex, pageSize, cancellationToken);
        }

        // For role checks (e.g., in project creation)
        public async Task<User?> FindByIdAsync(int id)
        {
            return await Entities
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User> Verify(string token)
        {
            return await Entities.FirstOrDefaultAsync(t => t.VerificationToken == token);
        }

    }
}