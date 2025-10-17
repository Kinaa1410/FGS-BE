using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.Entities;

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
    }
}
