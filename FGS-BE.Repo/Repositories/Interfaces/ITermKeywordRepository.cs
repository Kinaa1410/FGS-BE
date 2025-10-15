using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.Entities;

namespace FGS_BE.Repo.Repositories.Interfaces
{
    public interface ITermKeywordRepository : IGenericRepository<TermKeyword>
    {
        Task<PaginatedList<TermKeyword>> GetPagedAsync(
            int pageIndex,
            int pageSize,
            string? keyword = null,
            string? sortColumn = "Id",
            string? sortDir = "Asc",
            int? semesterId = null,
            CancellationToken cancellationToken = default);
    }
}
