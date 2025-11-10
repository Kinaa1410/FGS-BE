using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.Entities;

namespace FGS_BE.Repo.Repositories.Interfaces
{
    public interface IProjectMemberRepository : IGenericRepository<ProjectMember>
    {
        Task<PaginatedList<ProjectMember>> GetPagedAsync(
            int pageIndex,
            int pageSize,
            string? keyword = null,
            int? projectId = null,
            int? userId = null,
            string? sortColumn = "Id",
            string? sortDir = "Asc",
            CancellationToken cancellationToken = default);
    }
}
