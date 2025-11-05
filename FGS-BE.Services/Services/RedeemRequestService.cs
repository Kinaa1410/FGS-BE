using FGS_BE.Repo.DTOs.RedeemRequests;
using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Enums;
using FGS_BE.Repo.Repositories.Interfaces;
using FGS_BE.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FGS_BE.Service.Implements
{
    public class RedeemRequestService : IRedeemRequestService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RedeemRequestService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginatedList<RedeemRequestDto>> GetPagedAsync(
            int pageIndex,
            int pageSize,
            string? keyword = null,
            string? status = null,
            int? userId = null,
            int? rewardItemId = null,
            string? sortColumn = "Id",
            string? sortDir = "Asc")
        {
            var paged = await _unitOfWork.RedeemRequestRepository.GetPagedAsync(
                pageIndex, pageSize, keyword, status, userId, rewardItemId, sortColumn, sortDir);
            return new PaginatedList<RedeemRequestDto>(
                paged.Select(x => new RedeemRequestDto(x)).ToList(),
                paged.TotalItems,
                paged.PageIndex,
                paged.PageSize);
        }

        public async Task<PaginatedList<RedeemRequestDto>> GetPagedByUserAsync(
            int userId,
            int pageIndex,
            int pageSize,
            string? status = null,
            string? sortColumn = "Id",
            string? sortDir = "Asc")
        {
            var paged = await _unitOfWork.RedeemRequestRepository.GetPagedByUserAsync(
                userId, pageIndex, pageSize, status, sortColumn, sortDir);
            return new PaginatedList<RedeemRequestDto>(
                paged.Select(x => new RedeemRequestDto(x)).ToList(),
                paged.TotalItems,
                paged.PageIndex,
                paged.PageSize);
        }

        public async Task<RedeemRequestDto?> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.RedeemRequestRepository.FindByIdAsync(id);
            return entity == null ? null : new RedeemRequestDto(entity);
        }

        public async Task<RedeemRequestDto?> CreateAsync(CreateRedeemRequestDto dto)
        {
            // Fetch user with wallet for validation
            var userWithWallet = await _unitOfWork.Repository<User>()
                .Entities.Include(u => u.UserWallet)
                .FirstOrDefaultAsync(u => u.Id == dto.UserId);

            if (userWithWallet == null || userWithWallet.UserWallet == null)
            {
                return null;  // User or wallet not found
            }

            var rewardItem = await _unitOfWork.RewardItemRepository.FindByIdAsync(dto.RewardItemId);
            if (rewardItem == null)
            {
                return null;  // Reward item not found
            }

            // Validate points: User's wallet balance >= TotalPoints
            if (userWithWallet.UserWallet.Balance < dto.TotalPoints)
            {
                return null;  // Triggers BadRequest in controller
            }

            // Temporarily deduct points on creation (hold for pending request)
            userWithWallet.UserWallet.Balance -= dto.TotalPoints;
            userWithWallet.UserWallet.LastUpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Repository<UserWallet>().UpdateAsync(userWithWallet.UserWallet);

            var entity = dto.ToEntity();
            entity.User = userWithWallet;  // Attach for navigation
            await _unitOfWork.RedeemRequestRepository.CreateAsync(entity);
            await _unitOfWork.CommitAsync();
            return new RedeemRequestDto(entity);
        }

        public async Task<RedeemRequestDto?> UpdateStatusAsync(int id, UpdateStatusRedeemRequestDto dto)
        {
            if (dto.Status != RedeemRequestStatus.Approved && dto.Status != RedeemRequestStatus.Rejected)
            {
                return null;  // Invalid status
            }

            var entity = await _unitOfWork.RedeemRequestRepository.GetByIdWithDetailsAsync(id);
            if (entity == null) return null;

            // Only allow updating if current status is Pending
            if (entity.Status != RedeemRequestStatus.Pending)
            {
                return null;  // Already processed
            }

            entity.Status = dto.Status;
            entity.ProcessedAt = DateTime.UtcNow;

            // If rejected, refund the temporarily deducted points
            if (dto.Status == RedeemRequestStatus.Rejected)
            {
                entity.User.UserWallet.Balance += entity.TotalPoints;
                entity.User.UserWallet.LastUpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Repository<UserWallet>().UpdateAsync(entity.User.UserWallet);
            }
            // If approved, points remain deducted (no action needed)

            await _unitOfWork.RedeemRequestRepository.UpdateAsync(entity);
            await _unitOfWork.CommitAsync();
            return new RedeemRequestDto(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.RedeemRequestRepository.FindByIdAsync(id);
            if (entity == null) return false;

            // Optional: If deleting a Pending request, refund points
            if (entity.Status == RedeemRequestStatus.Pending)
            {
                var userWithWallet = await _unitOfWork.Repository<User>()
                    .Entities.Include(u => u.UserWallet)
                    .FirstOrDefaultAsync(u => u.Id == entity.UserId);
                if (userWithWallet?.UserWallet != null)
                {
                    userWithWallet.UserWallet.Balance += entity.TotalPoints;
                    userWithWallet.UserWallet.LastUpdatedAt = DateTime.UtcNow;
                    await _unitOfWork.Repository<UserWallet>().UpdateAsync(userWithWallet.UserWallet);
                }
            }

            await _unitOfWork.RedeemRequestRepository.DeleteAsync(entity);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}