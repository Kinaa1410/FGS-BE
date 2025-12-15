using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.Entities;

namespace FGS_BE.Repo.Repositories.Interfaces
{
    public interface IProjectRepository : IGenericRepository<Project>
    {
        Task<PaginatedList<Project>> GetPagedAsync(
            int pageIndex,
            int pageSize,
            string? keyword = null,
            string? status = null,
            string? sortColumn = "Id",
            string? sortDir = "Asc",
            CancellationToken cancellationToken = default);

        // GET PAGED BY MENTOR ID
        // ============================================================
        Task<PaginatedList<Project>> GetByMentorIdPagedAsync(
            int mentorId,
            int pageIndex,
            int pageSize,
            string? keyword = null,
            string? status = null,
            string? sortColumn = "Id",
            string? sortDir = "Asc");

        // ============================================================
        // GET PAGED BY MEMBER ID
        // ============================================================
        Task<PaginatedList<Project>> GetByMemberIdPagedAsync(
            int memberId,
            int pageIndex,
            int pageSize,
            string? keyword = null,
            string? status = null,
            string? sortColumn = "Id",
            string? sortDir = "Asc");
    }
}
