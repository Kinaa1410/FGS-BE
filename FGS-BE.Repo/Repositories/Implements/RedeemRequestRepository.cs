using FGS_BE.Repo.Data;
using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Enums;
using FGS_BE.Repo.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace FGS_BE.Repo.Repositories.Implements
{
    public class RedeemRequestRepository : GenericRepository<RedeemRequest>, IRedeemRequestRepository
    {
        public RedeemRequestRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<PaginatedList<RedeemRequest>> GetPagedAsync(
            int pageIndex,
            int pageSize,
            string? keyword = null,
            string? status = null,
            int? userId = null,
            int? rewardItemId = null,
            string? sortColumn = "Id",
            string? sortDir = "Asc",
            CancellationToken cancellationToken = default)
        {
            IQueryable<RedeemRequest> query = Entities.AsNoTracking()
                .Include(x => x.RewardItem)
                .Include(x => x.User).ThenInclude(u => u.UserWallet);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(x => x.RewardItem.Name.Contains(keyword) || x.RewardItem.Description.Contains(keyword)); // Assuming RewardItem has Name/Description
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                if (Enum.TryParse<RedeemRequestStatus>(status, true, out var parsedStatus))
                {
                    query = query.Where(x => x.Status == parsedStatus);
                }
            }

            if (userId.HasValue)
                query = query.Where(x => x.UserId == userId.Value);

            if (rewardItemId.HasValue)
                query = query.Where(x => x.RewardItemId == rewardItemId.Value);

            var order = $"{sortColumn} {sortDir}";
            query = query.OrderBy(order);

            return await query.PaginatedListAsync(pageIndex, pageSize, cancellationToken);
        }

        public async Task<PaginatedList<RedeemRequest>> GetPagedByUserAsync(
            int userId,
            int pageIndex,
            int pageSize,
            string? status = null,
            string? sortColumn = "Id",
            string? sortDir = "Asc",
            CancellationToken cancellationToken = default)
        {
            IQueryable<RedeemRequest> query = Entities.AsNoTracking()
                .Include(x => x.RewardItem)
                .Include(x => x.User).ThenInclude(u => u.UserWallet)
                .Where(x => x.UserId == userId);

            if (!string.IsNullOrWhiteSpace(status))
            {
                if (Enum.TryParse<RedeemRequestStatus>(status, true, out var parsedStatus))
                {
                    query = query.Where(x => x.Status == parsedStatus);
                }
            }

            var order = $"{sortColumn} {sortDir}";
            query = query.OrderBy(order);

            return await query.PaginatedListAsync(pageIndex, pageSize, cancellationToken);
        }

        public async Task<RedeemRequest?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await Entities
                .Include(x => x.User).ThenInclude(u => u.UserWallet)
                .Include(x => x.RewardItem)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }
    }
}