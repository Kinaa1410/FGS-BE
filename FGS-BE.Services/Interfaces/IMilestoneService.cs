using FGS_BE.Repo.DTOs.Milestones;
using FGS_BE.Repo.DTOs.Pages;

namespace FGS_BE.Service.Interfaces
{
    public interface IMilestoneService
    {
        Task<PaginatedList<MilestoneDto>> GetPagedAsync(
            int pageIndex,
            int pageSize,
            string? keyword = null,
            string? status = null,
            int? projectId = null,
            string? sortColumn = "Id",
            string? sortDir = "Asc");
        Task<MilestoneDto?> GetByIdAsync(int id);
        Task<MilestoneDto> CreateAsync(CreateMilestoneDto dto);
        Task<MilestoneDto?> UpdateAsync(int id, UpdateMilestoneDto dto);
        Task<bool> DeleteAsync(int id);
        Task ApplyDelayBufferAsync(int milestoneId, int hours = 36);
        Task ResetDelayAsync(int milestoneId);
    }
}