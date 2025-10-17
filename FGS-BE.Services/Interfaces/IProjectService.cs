using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.DTOs.Projects;

namespace FGS_BE.Service.Interfaces
{
    public interface IProjectService
    {
        Task<PaginatedList<ProjectDto>> GetPagedAsync(
            int pageIndex, 
            int pageSize, 
            string? keyword = null,
            string? status = null,
            string? sortColumn = "Id", 
            string? sortDir = "Asc");
        Task<ProjectDto?> GetByIdAsync(int id);
        Task<ProjectDto> CreateAsync(CreateProjectDto dto);
        Task<ProjectDto?> UpdateAsync(int id, UpdateProjectDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
