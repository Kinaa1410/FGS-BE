using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.Repositories.Interfaces;
using FGS_BE.Repo.Data;
using FGS_BE.Repo.Entities;

namespace FGS_BE.Repo.Repositories.Implements
{
    public class ProjectMemberRepository : GenericRepository<ProjectMember>, IProjectMemberRepository
    {
        public ProjectMemberRepository(ApplicationDbContext context) : base(context) { }

        public async Task<PaginatedList<ProjectMember>> GetPagedAsync(
    int pageIndex,
    int pageSize,
    string? keyword = null,
    int? projectId = null,
    int? userId = null,
    string? sortColumn = "Id",
    string? sortDir = "Asc",
    CancellationToken cancellationToken = default)
        {
            var query = Entities
                .Include(x => x.User)
                .Include(x => x.Project)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(x =>
                    (x.Role != null && x.Role.Contains(keyword)) ||
                    (x.User != null && x.User.FullName.Contains(keyword)) ||
                    (x.Project != null && x.Project.Title.Contains(keyword)));
            }

            if (projectId.HasValue)
            {
                query = query.Where(x => x.ProjectId == projectId.Value);
            }

            if (userId.HasValue)
            {
                query = query.Where(x => x.UserId == userId.Value);
            }

            var order = $"{sortColumn} {sortDir}";
            query = query.OrderBy(order);

            return await query.PaginatedListAsync(pageIndex, pageSize, cancellationToken);
        }

    }
}
