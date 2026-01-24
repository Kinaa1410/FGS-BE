using FGS_BE.Repo.Data;
using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace FGS_BE.Repo.Repositories.Implements
{
    public class SubmissionRepository : GenericRepository<Submission>, ISubmissionRepository
    {
        public SubmissionRepository(ApplicationDbContext context) : base(context) { }

        public async Task<PaginatedList<Submission>> GetPagedAsync(
            int pageIndex,
            int pageSize,
            int? userId = null,
            int? taskId = null,
            string? sortColumn = "Id",
            string? sortDir = "Asc",
            CancellationToken cancellationToken = default)
        {
            var query = Entities.AsNoTracking();
            if (userId.HasValue)
                query = query.Where(x => x.UserId == userId.Value);
            if (taskId.HasValue)
                query = query.Where(x => x.TaskId == taskId.Value);
            var order = $"{sortColumn} {sortDir}";
            query = query.OrderBy(order);
            return await query.PaginatedListAsync(pageIndex, pageSize, cancellationToken);
        }

        public async Task<Submission?> FindByAsync(
            Expression<Func<Submission, bool>> predicate,
            Func<IQueryable<Submission>, IQueryable<Submission>>? includeExpression = null,
            CancellationToken cancellationToken = default)
        {
            var query = Entities.Where(predicate);
            if (includeExpression != null)
            {
                query = includeExpression(query);
            }
            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<int> CountAsync(Expression<Func<Submission, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await Entities.CountAsync(predicate, cancellationToken);
        }
    }
}