using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.Entities;

namespace FGS_BE.Repo.Repositories.Interfaces;

public interface ILevelRepository : IGenericRepository<Level>
{
    Task<PaginatedList<Level>> GetPagedAsync(
        int pageIndex,
        int pageSize,
        string? keyword = null,
        string? sortColumn = "Id",
        string? sortDir = "Asc",
        CancellationToken cancellationToken = default);

    Task<ICollection<Level>> GetEligibleLevelsAsync(int currentPoints, CancellationToken cancellationToken = default);
}