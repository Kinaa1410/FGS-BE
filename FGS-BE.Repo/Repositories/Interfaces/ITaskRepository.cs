using FGS_BE.Repo.DTOs.Pages;
using Task = FGS_BE.Repo.Entities.Task;

namespace FGS_BE.Repo.Repositories.Interfaces
{
    public interface ITaskRepository : IGenericRepository<Task>
    {
        Task<PaginatedList<Task>> GetPagedAsync(
            int pageIndex,
            int pageSize,
            string? keyword = null,
            string? status = null,
            int? milestoneId = null,
            int? assigneeId = null,
            int? parentTaskId = null,
            string? sortColumn = "Id",
            string? sortDir = "Asc",
            CancellationToken cancellationToken = default);
    }
}
