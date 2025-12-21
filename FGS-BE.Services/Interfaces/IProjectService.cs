using FGS_BE.Repo.DTOs.Projects;
using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.Enums;
using System;
using System.Threading.Tasks;

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
        Task<PaginatedList<ProjectDto>> GetByMentorIdPagedAsync(
            int mentorId,
            int pageIndex,
            int pageSize,
            string? keyword = null,
            string? status = null,
            string? sortColumn = "Id",
            string? sortDir = "Asc");
        Task<PaginatedList<ProjectDto>> GetByMemberIdPagedAsync(
            int memberId,
            int pageIndex,
            int pageSize,
            string? keyword = null,
            string? status = null,
            string? sortColumn = "Id",
            string? sortDir = "Asc");
        Task<bool> StartByMentorAsync(int projectId, int mentorId);
        Task<bool> CloseByMentorAsync(int projectId, int mentorId);
    }
}