using FGS_BE.Repo.Data;
using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace FGS_BE.Repo.Repositories.Implements
{
    public class MilestoneRepository : GenericRepository<Milestone>, IMilestoneRepository
    {
        public MilestoneRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<PaginatedList<Milestone>> GetPagedAsync(
            int pageIndex,
            int pageSize,
            string? keyword = null,
            string? status = null,
            int? projectId = null,
            string? sortColumn = "Id",
            string? sortDir = "Asc",
            CancellationToken cancellationToken = default)
        {
            var query = Entities.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(x => x.Title!.Contains(keyword) || x.Description!.Contains(keyword));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(x => x.Status == status);
            }

            if (projectId.HasValue) 
                query = query.Where(x => x.ProjectId == projectId.Value);

            var order = $"{sortColumn} {sortDir}";
            query = query.OrderBy(order);

            return await query.PaginatedListAsync(pageIndex, pageSize, cancellationToken);
        }
    }
}
