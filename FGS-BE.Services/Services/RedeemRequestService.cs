using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.DTOs.RedeemRequests;
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

        // ===========================
        // GET PAGED (Updated to support 'collected' filter)
        // ===========================
        public async Task<PaginatedList<RedeemRequestDto>> GetPagedAsync(
            int pageIndex, int pageSize, string? keyword, string? status,
            int? userId, int? rewardItemId, bool? collected, string? sortColumn, string? sortDir)
        {
            var paged = await _unitOfWork.RedeemRequestRepository.GetPagedAsync(
                pageIndex, pageSize, keyword, status, userId, rewardItemId, collected, sortColumn, sortDir);
            return new PaginatedList<RedeemRequestDto>(
                paged.Select(x => new RedeemRequestDto(x)).ToList(),
                paged.TotalItems,
                paged.PageIndex,
                paged.PageSize);
        }

        public async Task<PaginatedList<RedeemRequestDto>> GetPagedByUserAsync(
            int userId, int pageIndex, int pageSize, string? status, bool? collected, string? sortColumn, string? sortDir)
        {
            var paged = await _unitOfWork.RedeemRequestRepository
                .GetPagedByUserAsync(userId, pageIndex, pageSize, status, collected, sortColumn, sortDir);
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

        // ===========================
        // CREATE (No changes)
        // ===========================
        public async Task<RedeemRequestDto> CreateAsync(CreateRedeemRequestDto dto)
        {
            var userWithWallet = await _unitOfWork.Repository<User>()
                .Entities.Include(u => u.UserWallet)
                .FirstOrDefaultAsync(u => u.Id == dto.UserId);
            if (userWithWallet == null || userWithWallet.UserWallet == null)
                throw new KeyNotFoundException("User or wallet not found.");

            var rewardItem = await _unitOfWork.RewardItemRepository.FindByIdAsync(dto.RewardItemId);
            if (rewardItem == null)
                throw new KeyNotFoundException("Reward item not found.");

            if (dto.Quantity <= 0)
                throw new InvalidOperationException("Quantity must be greater than 0.");

            var totalPoints = rewardItem.PriceInPoints * dto.Quantity;
            if (userWithWallet.UserWallet.Balance < totalPoints)
                throw new InvalidOperationException("Not enough points.");

            userWithWallet.UserWallet.Balance -= totalPoints;
            userWithWallet.UserWallet.LastUpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Repository<UserWallet>().UpdateAsync(userWithWallet.UserWallet);

            var entity = new RedeemRequest
            {
                Quantity = dto.Quantity,
                TotalPoints = totalPoints,
                UserId = dto.UserId,
                RewardItemId = dto.RewardItemId,
                Status = RedeemRequestStatus.Pending,
                RequestedAt = DateTime.UtcNow
            };
            await _unitOfWork.RedeemRequestRepository.CreateAsync(entity);
            await _unitOfWork.CommitAsync();
            return new RedeemRequestDto(entity);
        }

        // ===========================
        // UPDATE STATUS (Minor: Allow only Pending → Approved/Rejected)
        // ===========================
        public async Task<RedeemRequestDto> UpdateStatusAsync(int id, UpdateStatusRedeemRequestDto dto)
        {
            var entity = await _unitOfWork.RedeemRequestRepository
                .GetByIdWithDetailsAsync(id);
            if (entity == null)
                throw new KeyNotFoundException("Redeem request not found.");

            if (entity.Status != RedeemRequestStatus.Pending)
                throw new InvalidOperationException("Request already processed.");

            if (dto.Status != RedeemRequestStatus.Approved && dto.Status != RedeemRequestStatus.Rejected)
                throw new InvalidOperationException("Only Approved or Rejected is allowed.");

            entity.Status = dto.Status;
            entity.ProcessedAt = DateTime.UtcNow;

            if (dto.Status == RedeemRequestStatus.Rejected)
            {
                entity.User.UserWallet.Balance += entity.TotalPoints;
                entity.User.UserWallet.LastUpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Repository<UserWallet>()
                    .UpdateAsync(entity.User.UserWallet);
            }

            await _unitOfWork.RedeemRequestRepository.UpdateAsync(entity);
            await _unitOfWork.CommitAsync();
            return new RedeemRequestDto(entity);
        }

        // ===========================
        // NEW: MARK AS COLLECTED (Check if approved and not yet collected)
        // ===========================
        public async Task<RedeemRequestDto> MarkAsCollectedAsync(int id)
        {
            var entity = await _unitOfWork.RedeemRequestRepository
                .GetByIdWithDetailsAsync(id);
            if (entity == null)
                throw new KeyNotFoundException("Redeem request not found.");

            if (entity.Status != RedeemRequestStatus.Approved)
                throw new InvalidOperationException("Request must be approved to collect.");

            if (entity.CollectedAt != null)
                throw new InvalidOperationException("Item already collected.");

            entity.Status = RedeemRequestStatus.PickedUp;
            entity.CollectedAt = DateTime.UtcNow;
            await _unitOfWork.RedeemRequestRepository.UpdateAsync(entity);
            await _unitOfWork.CommitAsync();
            return new RedeemRequestDto(entity);
        }

        // ===========================
        // DELETE (Updated: No refund if already PickedUp)
        // ===========================
        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.RedeemRequestRepository.FindByIdAsync(id);
            if (entity == null)
                throw new KeyNotFoundException("Redeem request not found.");

            if (entity.Status != RedeemRequestStatus.PickedUp)  // Refund only if not collected
            {
                var user = await _unitOfWork.Repository<User>()
                    .Entities.Include(x => x.UserWallet)
                    .FirstOrDefaultAsync(x => x.Id == entity.UserId);
                if (user?.UserWallet != null)
                {
                    user.UserWallet.Balance += entity.TotalPoints;
                    user.UserWallet.LastUpdatedAt = DateTime.UtcNow;
                    await _unitOfWork.Repository<UserWallet>()
                        .UpdateAsync(user.UserWallet);
                }
            }

            await _unitOfWork.RedeemRequestRepository.DeleteAsync(entity);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}