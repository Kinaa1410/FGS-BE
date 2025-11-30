using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.Entities;

namespace FGS_BE.Repo.Repositories.Interfaces
{
    public interface IPerformanceScoreRepository : IGenericRepository<PerformanceScore>
    {
        Task<PaginatedList<PerformanceScore>> GetPagedAsync(
            int pageIndex,
            int pageSize,
            string? keyword = null,
            int? userId = null,
            int? projectId = null,
            int? milestoneId = null,
            int? taskId = null,
            string? sortColumn = "Id",
            string? sortDir = "Asc",
            CancellationToken cancellationToken = default
        );
    }
}
