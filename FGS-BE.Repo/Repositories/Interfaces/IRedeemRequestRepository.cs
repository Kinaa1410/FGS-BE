using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.Entities;

namespace FGS_BE.Repo.Repositories.Interfaces
{
    public interface IRedeemRequestRepository : IGenericRepository<RedeemRequest>
    {
        Task<PaginatedList<RedeemRequest>> GetPagedAsync(
            int pageIndex,
            int pageSize,
            string? keyword = null,
            string? status = null,
            int? userId = null,
            int? rewardItemId = null,
            bool? collected = null,
            string? sortColumn = "Id",
            string? sortDir = "Asc",
            CancellationToken cancellationToken = default);

        Task<PaginatedList<RedeemRequest>> GetPagedByUserAsync(
            int userId,
            int pageIndex,
            int pageSize,
            string? status = null,
            bool? collected = null,
            string? sortColumn = "Id",
            string? sortDir = "Asc",
            CancellationToken cancellationToken = default);

        Task<RedeemRequest?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
    }
}