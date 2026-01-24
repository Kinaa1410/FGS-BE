using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.Entities;
using System.Linq.Expressions;

namespace FGS_BE.Repo.Repositories.Interfaces
{
    public interface ISemesterRepository : IGenericRepository<Semester>
    {
        Task<PaginatedList<Semester>> GetPagedAsync(
        int pageIndex,
        int pageSize,
        string? keyword = null,
        string? status = null,
        string? sortColumn = "Id",
        string? sortDir = "Asc",
        CancellationToken cancellationToken = default);

        Task<List<Semester>> GetByAsync(
            Expression<Func<Semester, bool>> predicate,
            Func<IQueryable<Semester>, IQueryable<Semester>>? include = null,
            CancellationToken cancellationToken = default);
    }
}
