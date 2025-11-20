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
            var pagedEntities = await _unitOfWork.RewardItemRepository.GetPagedAsync(
                pageIndex,
                pageSize,
                keyword,
                isActive,
                sortColumn,
                sortDir
            );

            var pagedDtos = new PaginatedList<RewardItemDto>(
                pagedEntities.Select(x => new RewardItemDto(x)).ToList(),
                pagedEntities.TotalItems,
                pagedEntities.PageIndex,
                pagedEntities.PageSize
            );

            return pagedDtos;
        }

        public async Task<RewardItemDto?> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.RewardItemRepository.FindByIdAsync(id);
            return entity == null ? null : new RewardItemDto(entity);
        }

        public async Task<RewardItemDto> CreateAsync(CreateRewardItemDto dto)
        {
            var entity = dto.ToEntity();
            await _unitOfWork.RewardItemRepository.CreateAsync(entity);
            await _unitOfWork.CommitAsync();
            return new RewardItemDto(entity);
        }

        public async Task<RewardItemDto?> UpdateAsync(int id, UpdateRewardItemDto dto)
        {
            var entity = await _unitOfWork.RewardItemRepository.FindByIdAsync(id);
            if (entity == null) return null;

            dto.ApplyToEntity(entity);
            _unitOfWork.RewardItemRepository.UpdateAsync(entity);
            await _unitOfWork.CommitAsync();

            return new RewardItemDto(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.RewardItemRepository.FindByIdAsync(id);
            if (entity == null) return false;

            _unitOfWork.RewardItemRepository.DeleteAsync(entity);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}