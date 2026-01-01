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
            try
            {
                var paged = await _unitOfWork.PerformanceScoreRepository.GetPagedAsync(
                    pageIndex, pageSize, keyword, userId, projectId, milestoneId, taskId, sortColumn, sortDir);

                return new PaginatedList<PerformanceScoreDto>(
                    paged.Select(x => new PerformanceScoreDto(x)).ToList(),
                    paged.TotalItems,
                    paged.PageIndex,
                    paged.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("Không thể lấy danh sách điểm hiệu suất: " + ex.Message);
            }
        }

        public async Task<PerformanceScoreDto?> GetByIdAsync(int id)
        {
            try
            {
                var entity = await _unitOfWork.PerformanceScoreRepository.FindByIdAsync(id);
                return entity == null ? null : new PerformanceScoreDto(entity);
            }
            catch (Exception ex)
            {
                throw new Exception("Không thể lấy thông tin điểm hiệu suất: " + ex.Message);
            }
        }

        public async Task<PerformanceScoreDto> CreateAsync(CreatePerformanceScoreDto dto)
        {
            try
            {
                if (dto.Score < 0)
                    throw new ArgumentException("Score must be >= 0.");
                if (dto.Score > 100)
                    throw new ArgumentException("Score must be <= 100.");

                var entity = dto.ToEntity();

                await _unitOfWork.PerformanceScoreRepository.CreateAsync(entity);
                await _unitOfWork.CommitAsync();

                return new PerformanceScoreDto(entity);
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
                throw new Exception("Không thể tạo điểm hiệu suất: " + ex.Message);
            }
        }

        public async Task<PerformanceScoreDto?> UpdateAsync(int id, UpdatePerformanceScoreDto dto)
        {
            try
            {
                var entity = await _unitOfWork.PerformanceScoreRepository.FindByIdAsync(id);
                if (entity == null) return null;

                if (dto.Score < 0)
                    throw new ArgumentException("Score must be >= 0.");
                if (dto.Score > 100)
                    throw new ArgumentException("Score must be <= 100.");

                entity.Score = dto.Score;
                entity.Comment = dto.Comment;

                await _unitOfWork.PerformanceScoreRepository.UpdateAsync(entity);
                await _unitOfWork.CommitAsync();

                return new PerformanceScoreDto(entity);
            }
            catch (Exception ex)
            {
                throw new Exception("Không thể cập nhật điểm hiệu suất: " + ex.Message);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var entity = await _unitOfWork.PerformanceScoreRepository.FindByIdAsync(id);
                if (entity == null) return false;

                await _unitOfWork.PerformanceScoreRepository.DeleteAsync(entity);
                await _unitOfWork.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Không thể xóa điểm hiệu suất: " + ex.Message);
            }
        }
    }
}
