using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.DTOs.RewardItems;
using FGS_BE.Repo.Entities;

namespace FGS_BE.Services.Interfaces
{
    public interface IRewardItemService
    {
        Task<PaginatedList<RewardItemDto>> GetPagedAsync(
            int pageIndex,
            int pageSize,
            string? keyword = null,
            bool? isActive = null,
            string? sortColumn = "Id",
            string? sortDir = "Asc");

        Task<RewardItemDto?> GetByIdAsync(int id);
        Task<RewardItemDto> CreateAsync(CreateRewardItemDto dto);
        Task<RewardItemDto?> UpdateAsync(int id, UpdateRewardItemDto dto);
        Task<bool> DeleteAsync(int id);
    }
}