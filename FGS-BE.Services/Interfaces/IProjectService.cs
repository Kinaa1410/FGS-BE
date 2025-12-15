using FGS_BE.Repo.DTOs.Projects;
using FGS_BE.Repo.DTOs.Pages; // For PaginatedList<ProjectDto> if needed

namespace FGS_BE.Service.Interfaces
{
    public interface IProjectService
    {
        Task<ProjectDto> CreateAsync(CreateProjectDto dto);
        Task<ProjectDto?> UpdateAsync(int id, UpdateProjectDto dto);
        Task<ProjectDto?> GetByIdAsync(int id);
        Task<bool> DeleteAsync(int id);

        // FIXED: Full signature for paged query (matches controller call)
        Task<PaginatedList<ProjectDto>> GetPagedAsync(
            int pageIndex,
            int pageSize,
            string? keyword = null,
            string? status = null,
            string? sortColumn = "Id",
            string? sortDir = "Asc");

        // GET PAGED BY MENTOR ID
        // ============================================================
        Task<PaginatedList<ProjectDto>> GetByMentorIdPagedAsync(
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
        Task<PaginatedList<ProjectDto>> GetByMemberIdPagedAsync(
            int memberId,
            int pageIndex,
            int pageSize,
            string? keyword = null,
            string? status = null,
            string? sortColumn = "Id",
            string? sortDir = "Asc");
    }
}