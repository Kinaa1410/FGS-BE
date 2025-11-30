using FGS_BE.Repo.Repositories.Interfaces;
using FGS_BE.Service.Interfaces;
using FGS_BE.Repo.DTOs.PerformanceScores;
using FGS_BE.Repo.DTOs.Pages;

namespace FGS_BE.Service.Implements
{
    public class PerformanceScoreService : IPerformanceScoreService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PerformanceScoreService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginatedList<PerformanceScoreDto>> GetPagedAsync(
            int pageIndex, int pageSize,
            string? keyword = null,
            int? userId = null,
            int? projectId = null,
            int? milestoneId = null,
            int? taskId = null,
            string? sortColumn = "Id",
            string? sortDir = "Asc")
        {
            var paged = await _unitOfWork.PerformanceScoreRepository.GetPagedAsync(
                pageIndex, pageSize, keyword, userId, projectId, milestoneId, taskId, sortColumn, sortDir);

            return new PaginatedList<PerformanceScoreDto>(
                paged.Select(x => new PerformanceScoreDto(x)).ToList(),
                paged.TotalItems,
                paged.PageIndex,
                paged.PageSize);
        }

        public async Task<PerformanceScoreDto?> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.PerformanceScoreRepository.FindByIdAsync(id);
            return entity == null ? null : new PerformanceScoreDto(entity);
        }

        public async Task<PerformanceScoreDto> CreateAsync(CreatePerformanceScoreDto dto)
        {
            var entity = dto.ToEntity();

            await _unitOfWork.PerformanceScoreRepository.CreateAsync(entity);
            await _unitOfWork.CommitAsync();

            return new PerformanceScoreDto(entity);
        }

        public async Task<PerformanceScoreDto?> UpdateAsync(int id, UpdatePerformanceScoreDto dto)
        {
            var entity = await _unitOfWork.PerformanceScoreRepository.FindByIdAsync(id);

            if (entity == null)
                return null;

            entity.Score = dto.Score;
            entity.Comment = dto.Comment;

            await _unitOfWork.PerformanceScoreRepository.UpdateAsync(entity);
            await _unitOfWork.CommitAsync();

            return new PerformanceScoreDto(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.PerformanceScoreRepository.FindByIdAsync(id);

            if (entity == null)
                return false;

            await _unitOfWork.PerformanceScoreRepository.DeleteAsync(entity);
            await _unitOfWork.CommitAsync();

            return true;
        }
    }
}
