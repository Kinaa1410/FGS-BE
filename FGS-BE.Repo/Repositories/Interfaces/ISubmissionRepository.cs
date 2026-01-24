using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.Entities;
using System.Linq.Expressions;

namespace FGS_BE.Repo.Repositories.Interfaces
{
    public interface ISubmissionRepository : IGenericRepository<Submission>
    {
        Task<PaginatedList<Submission>> GetPagedAsync(
            int pageIndex,
            int pageSize,
            int? userId = null,
            int? taskId = null,
            string? sortColumn = "Id",
            string? sortDir = "Asc",
            CancellationToken cancellationToken = default);

        Task<Submission?> FindByAsync(
            Expression<Func<Submission, bool>> predicate,
            Func<IQueryable<Submission>, IQueryable<Submission>>? includeExpression = null,
            CancellationToken cancellationToken = default);

        Task<int> CountAsync(Expression<Func<Submission, bool>> predicate, CancellationToken cancellationToken = default);
    }
}
