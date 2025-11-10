using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.DTOs.ProjectMembers;

namespace FGS_BE.Services.Interfaces
{
    public interface IProjectMemberService
    {
        Task<PaginatedList<ProjectMemberDto>> GetPagedAsync(
            int pageIndex,
            int pageSize,
            string? keyword = null,
            int? projectId = null,
            int? userId = null,
            string? sortColumn = "Id",
            string? sortDir = "Asc");

        Task<ProjectMemberDto?> GetByIdAsync(int id);
        Task<ProjectMemberDto> CreateAsync(CreateProjectMemberDto dto);
        Task<ProjectMemberDto?> UpdateAsync(int id, UpdateProjectMemberDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
