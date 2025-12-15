using FGS_BE.Repo.Data;
using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Enums; // Add for ProjectStatus
using FGS_BE.Repo.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
namespace FGS_BE.Repo.Repositories.Implements
{
    public class ProjectRepository : GenericRepository<Project>, IProjectRepository
    {
        private readonly ApplicationDbContext _context;

        public ProjectRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<PaginatedList<Project>> GetPagedAsync(
            int pageIndex,
            int pageSize,
            string? keyword = null,
            string? status = null,
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
                // Convert enum to string for comparison
                query = query.Where(x => x.Status.ToString() == status);
            }
            var order = $"{sortColumn} {sortDir}";
            query = query.OrderBy(order);
            return await query.PaginatedListAsync(pageIndex, pageSize, cancellationToken);
        }

        // ============================================================
        // GET PAGED BY MENTOR ID
        // ============================================================
        public async Task<PaginatedList<Project>> GetByMentorIdPagedAsync(
            int mentorId,
            int pageIndex,
            int pageSize,
            string? keyword = null,
            string? status = null,
            string? sortColumn = "Id",
            string? sortDir = "Asc")
        {
            var query = Entities
                .Where(p => p.MentorId == mentorId)
                .AsNoTracking();

            // Apply keyword search (on Title and Description for consistency)
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(p => p.Title!.Contains(keyword) || p.Description!.Contains(keyword));
            }

            // Apply status filter (consistent with GetPagedAsync)
            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(p => p.Status.ToString() == status);
            }

            // Apply sorting (using dynamic for consistency and flexibility)
            var order = $"{sortColumn} {sortDir}";
            query = query.OrderBy(order);

            return await query.PaginatedListAsync(pageIndex, pageSize);
        }

        // ============================================================
        // GET PAGED BY MEMBER ID
        // ============================================================
        public async Task<PaginatedList<Project>> GetByMemberIdPagedAsync(
            int memberId,
            int pageIndex,
            int pageSize,
            string? keyword = null,
            string? status = null,
            string? sortColumn = "Id",
            string? sortDir = "Asc")
        {
            // Base query: Join Projects with ProjectMembers to filter by UserId (MemberId)
            // Assuming ProjectMember is an entity with DbSet in ApplicationDbContext
            var projectMembers = _context.Set<ProjectMember>().AsNoTracking();
            var query = from p in Entities
                        join pm in projectMembers on p.Id equals pm.ProjectId
                        where pm.UserId == memberId
                        select p;

            // Apply Distinct to avoid potential duplicates from join
            query = query.Distinct();

            // Apply keyword search (on Title and Description for consistency)
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(p => p.Title!.Contains(keyword) || p.Description!.Contains(keyword));
            }

            // Apply status filter (consistent with GetPagedAsync)
            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(p => p.Status.ToString() == status);
            }

            // Apply sorting (using dynamic for consistency and flexibility)
            var order = $"{sortColumn} {sortDir}";
            query = query.OrderBy(order);

            return await query.PaginatedListAsync(pageIndex, pageSize);
        }
    }
}