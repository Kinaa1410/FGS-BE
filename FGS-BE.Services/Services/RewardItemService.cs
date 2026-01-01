using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.DTOs.RewardItems;
using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Repositories.Implements;
using FGS_BE.Repo.Repositories.Interfaces;
using FGS_BE.Services.Interfaces;
using Mapster;

namespace FGS_BE.Service.Services
{
    public class RewardItemService : IRewardItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        public RewardItemService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginatedList<RewardItemDto>> GetPagedAsync(
            int pageIndex,
            int pageSize,
            string? keyword = null,
            bool? isActive = null,
            string? sortColumn = "Id",
            string? sortDir = "Asc")
        {
            try
            {
                var pagedEntities = await _unitOfWork.RewardItemRepository.GetPagedAsync(
                    pageIndex,
                    pageSize,
                    keyword,
                    isActive,
                    sortColumn,
                    sortDir
                );

                return new PaginatedList<RewardItemDto>(
                    pagedEntities.Select(x => new RewardItemDto(x)).ToList(),
                    pagedEntities.TotalItems,
                    pagedEntities.PageIndex,
                    pagedEntities.PageSize
                );
            }
            catch (Exception ex)
            {
                throw new Exception("Không thể lấy danh sách reward item: " + ex.Message);
            }
        }

        public async Task<RewardItemDto?> GetByIdAsync(int id)
        {
            try
            {
                var entity = await _unitOfWork.RewardItemRepository.FindByIdAsync(id);
                return entity == null ? null : new RewardItemDto(entity);
            }
            catch (Exception ex)
            {
                throw new Exception("Không thể lấy thông tin reward item: " + ex.Message);
            }
        }

        public async Task<RewardItemDto> CreateAsync(CreateRewardItemDto dto)
        {
            try
            {
                var entity = dto.ToEntity();
                await _unitOfWork.RewardItemRepository.CreateAsync(entity);
                await _unitOfWork.CommitAsync();
                return new RewardItemDto(entity);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Không thể tạo reward item: " + ex.Message);
            }
        }

        public async Task<RewardItemDto?> UpdateAsync(int id, UpdateRewardItemDto dto)
        {
            try
            {
                var entity = await _unitOfWork.RewardItemRepository.FindByIdAsync(id);
                if (entity == null) return null;

                dto.ApplyToEntity(entity);
                _unitOfWork.RewardItemRepository.UpdateAsync(entity);
                await _unitOfWork.CommitAsync();

                return new RewardItemDto(entity);
            }
            catch (Exception ex)
            {
                throw new Exception("Không thể cập nhật reward item: " + ex.Message);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var entity = await _unitOfWork.RewardItemRepository.FindByIdAsync(id);
                if (entity == null) return false;

                _unitOfWork.RewardItemRepository.DeleteAsync(entity);
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Không thể xóa reward item: " + ex.Message);
            }
        }
    }
}
