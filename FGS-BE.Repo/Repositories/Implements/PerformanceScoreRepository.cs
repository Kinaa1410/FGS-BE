using FGS_BE.Repo.Data;
using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Repositories.Interfaces;
using FGS_BE.Repo.DTOs.Pages;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace FGS_BE.Repo.Repositories.Implements
{
    public class PerformanceScoreRepository : GenericRepository<PerformanceScore>, IPerformanceScoreRepository
    {
        public PerformanceScoreRepository(ApplicationDbContext context) : base(context) { }

        public async Task<PaginatedList<PerformanceScore>> GetPagedAsync(
            int pageIndex,
            int pageSize,
            string? keyword = null,
            int? userId = null,
            int? projectId = null,
            int? milestoneId = null,
            int? taskId = null,
            string? sortColumn = "Id",
            string? sortDir = "Asc",
            CancellationToken cancellationToken = default)
        {
            var query = Entities.AsNoTracking();

            // Search (comment cho phép tìm theo comment)
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(x => x.Comment!.Contains(keyword));
            }

            if (userId.HasValue)
                query = query.Where(x => x.UserId == userId.Value);

            if (projectId.HasValue)
                query = query.Where(x => x.ProjectId == projectId.Value);

            if (milestoneId.HasValue)
                query = query.Where(x => x.MilestoneId == milestoneId.Value);

            if (taskId.HasValue)
                query = query.Where(x => x.TaskId == taskId.Value);

            var order = $"{sortColumn} {sortDir}";
            query = query.OrderBy(order);

            return await query.PaginatedListAsync(pageIndex, pageSize, cancellationToken);
        }
    }
}
