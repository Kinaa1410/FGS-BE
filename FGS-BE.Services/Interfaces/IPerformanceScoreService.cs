using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.DTOs.PerformanceScores;

namespace FGS_BE.Service.Interfaces
{
    public interface IPerformanceScoreService
    {
        Task<PaginatedList<PerformanceScoreDto>> GetPagedAsync(
            int pageIndex, int pageSize,
            string? keyword = null,
            int? userId = null,
            int? projectId = null,
            int? milestoneId = null,
            int? taskId = null,
            string? sortColumn = "Id",
            string? sortDir = "Asc");

        Task<PerformanceScoreDto?> GetByIdAsync(int id);
        Task<PerformanceScoreDto> CreateAsync(CreatePerformanceScoreDto dto);
        Task<PerformanceScoreDto?> UpdateAsync(int id, UpdatePerformanceScoreDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
