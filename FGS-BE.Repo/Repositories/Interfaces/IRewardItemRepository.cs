using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.Entities;

namespace FGS_BE.Repo.Repositories.Interfaces
{
    public interface IRewardItemRepository : IGenericRepository<RewardItem>
    {
        Task<PaginatedList<RewardItem>> GetPagedAsync(
            int pageIndex,
            int pageSize,
            string? keyword = null,
            bool? isActive = null,
            string? sortColumn = "Id",
            string? sortDir = "Asc",
            CancellationToken cancellationToken = default);
    }
}