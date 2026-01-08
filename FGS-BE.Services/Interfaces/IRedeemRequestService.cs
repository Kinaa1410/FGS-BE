using FGS_BE.Repo.DTOs.RedeemRequests;
using FGS_BE.Repo.DTOs.Pages;

namespace FGS_BE.Service.Interfaces
{
    public interface IRedeemRequestService
    {
        Task<PaginatedList<RedeemRequestDto>> GetPagedAsync(
            int pageIndex,
            int pageSize,
            string? keyword = null,
            string? status = null,
            int? userId = null,
            int? rewardItemId = null,
            bool? collected = null,
            string? sortColumn = "Id",
            string? sortDir = "Asc");

        Task<PaginatedList<RedeemRequestDto>> GetPagedByUserAsync(
            int userId,
            int pageIndex,
            int pageSize,
            string? status = null,
            bool? collected = null,
            string? sortColumn = "Id",
            string? sortDir = "Asc");

        Task<RedeemRequestDto?> GetByIdAsync(int id);

        Task<RedeemRequestDto> CreateAsync(CreateRedeemRequestDto dto);

        Task<RedeemRequestDto?> UpdateStatusAsync(int id, UpdateStatusRedeemRequestDto dto);
        Task<RedeemRequestDto> MarkAsCollectedAsync(int id);

        Task<bool> DeleteAsync(int id);
    }
}