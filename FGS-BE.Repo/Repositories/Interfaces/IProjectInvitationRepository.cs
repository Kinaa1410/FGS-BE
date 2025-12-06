using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.Entities;

namespace FGS_BE.Repo.Repositories.Interfaces
{
    public interface IProjectInvitationRepository : IGenericRepository<ProjectInvitation>
    {
        Task<PaginatedList<ProjectInvitation>> GetPagedAsync(
            int projectId,
            int pageIndex,
            int pageSize,
            string? status = null,
            string? sortColumn = "Id",
            string? sortDir = "Asc",
            CancellationToken cancellationToken = default);
    }
}