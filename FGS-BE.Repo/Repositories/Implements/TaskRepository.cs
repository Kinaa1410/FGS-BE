using FGS_BE.Repo.Data;
using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Task = FGS_BE.Repo.Entities.Task;

namespace FGS_BE.Repo.Repositories.Implements
{
    public class TaskRepository : GenericRepository<Task>, ITaskRepository
    {
        public TaskRepository(ApplicationDbContext context) : base(context) { }

        public async Task<PaginatedList<Task>> GetPagedAsync(
            int pageIndex,
            int pageSize,
            string? keyword = null,
            string? status = null,
            int? milestoneId = null,
            int? assigneeId = null,
            int? parentTaskId = null,
            string? sortColumn = "Id",
            string? sortDir = "Asc",
            CancellationToken cancellationToken = default)
        {
            var query = Entities.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(x => x.Label!.Contains(keyword) || x.Description!.Contains(keyword));

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(x => x.Status == status);

            if (milestoneId.HasValue)
                query = query.Where(x => x.MilestoneId == milestoneId.Value);

            if (assigneeId.HasValue)
                query = query.Where(x => x.AssigneeId == assigneeId.Value);

            if (parentTaskId.HasValue)
                query = query.Where(x => x.ParentTaskId == parentTaskId.Value);

            query = query.OrderBy($"{sortColumn} {sortDir}");
            return await query.PaginatedListAsync(pageIndex, pageSize, cancellationToken);
        }
    }
}
