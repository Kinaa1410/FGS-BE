using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.DTOs.Tasks;

namespace FGS_BE.Service.Interfaces
{
    public interface ITaskService
    {
        Task<PaginatedList<TaskDto>> GetPagedAsync(
            int pageIndex,
            int pageSize,
            string? keyword = null,
            string? status = null,
            int? milestoneId = null,
            int? assigneeId = null,
            int? parentTaskId = null,
            int? projectId = null,                 
            string? sortColumn = "Id",
            string? sortDir = "Asc");

        Task<TaskDto?> GetByIdAsync(int id);
        Task<TaskDto> CreateAsync(CreateTaskDto dto);
        Task<TaskDto?> UpdateAsync(int id, UpdateTaskDto dto);
        Task<bool> DeleteAsync(int id);
    }
}