using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.Entities;

namespace FGS_BE.Repo.Repositories.Interfaces
{
    public interface IMilestoneRepository : IGenericRepository<Milestone>
    {
        Task<PaginatedList<Milestone>> GetPagedAsync(
            int pageIndex,
            int pageSize,
            string? keyword = null,
            string? status = null,
            int? projectId = null,
            string? sortColumn = "Id",
            string? sortDir = "Asc",
            CancellationToken cancellationToken = default);
    }
}
