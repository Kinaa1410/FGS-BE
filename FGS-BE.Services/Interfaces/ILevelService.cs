using FGS_BE.Repo.DTOs.Levels;
using FGS_BE.Repo.DTOs.Pages;

namespace FGS_BE.Service.Interfaces;

public interface ILevelService
{
    Task<PaginatedList<LevelDto>> GetPagedAsync(int pageIndex, int pageSize, string? keyword = null, string? sortColumn = "Id", string? sortDir = "Asc");
    Task<LevelDto?> GetByIdAsync(int id);
    Task<LevelDto> CreateAsync(CreateLevelDto dto);
    Task<LevelDto?> UpdateAsync(int id, UpdateLevelDto dto);
    Task<bool> DeleteAsync(int id);

    // Auto-update user level
    Task<UserLevelDto?> CheckAndAssignUserLevelAsync(int userId, CheckLevelUpRequest request);
}