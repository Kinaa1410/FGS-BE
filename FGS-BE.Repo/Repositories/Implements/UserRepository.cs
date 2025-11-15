using FGS_BE.Repo.Data;
using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

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

            // Apply any additional filters from GetUsersQuery if needed (e.g., via expressions)
            // Example: if (request.Status != null) query = query.Where(x => x.Status == request.Status);

            var order = $"{sortColumn} {sortDir}";
            query = query.OrderBy(order);

            return await query.PaginatedListAsync(pageIndex, pageSize, cancellationToken);
        }
    }
}