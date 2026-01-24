using FGS_BE.Repo.Data;
using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace FGS_BE.Repo.Repositories.Implements
{
    public class ProjectInvitationRepository : GenericRepository<ProjectInvitation>, IProjectInvitationRepository
    {
        public ProjectInvitationRepository(ApplicationDbContext context) : base(context) { }

        public async Task<PaginatedList<ProjectInvitation>> GetPagedAsync(
            int projectId,
            int pageIndex,
            int pageSize,
            string? status = null,
            string? sortColumn = "Id",
            string? sortDir = "Asc",
            CancellationToken cancellationToken = default)
        {
            var query = Entities
                .Where(pi => pi.ProjectId == projectId)
                .AsNoTracking();
            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(pi => pi.Status == status);
            }
            var order = $"{sortColumn} {sortDir}";
            query = query.OrderBy(order);
            return await query.PaginatedListAsync(pageIndex, pageSize, cancellationToken);
        }
    }
}