using FGS_BE.Repo.Data;
using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace FGS_BE.Repo.Repositories.Implements
{
    public class RewardItemRepository : GenericRepository<RewardItem>, IRewardItemRepository
    {
        public RewardItemRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<PaginatedList<RewardItem>> GetPagedAsync(
        int pageIndex,
        int pageSize,
        string? keyword = null,
        bool? isActive = null,
        string? sortColumn = "Id",
        string? sortDir = "Asc",
        CancellationToken cancellationToken = default)
        {
            var query = Entities.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(x => x.Name!.Contains(keyword) || x.Description!.Contains(keyword));
            }

            if (isActive.HasValue)
            {
                query = query.Where(x => x.IsActive == isActive.Value);
            }

            var order = $"{sortColumn} {sortDir}";
            query = query.OrderBy(order);

            return await query.PaginatedListAsync(pageIndex, pageSize, cancellationToken);
        }
    }
}