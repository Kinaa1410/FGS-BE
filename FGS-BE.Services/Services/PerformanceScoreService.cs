using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.DTOs.PerformanceScores;
using FGS_BE.Repo.Repositories.Interfaces;
using FGS_BE.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

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
                throw new Exception("Fail to get list of performance score: " + ex.Message);
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
                throw new Exception("Fail to get this performance score: " + ex.Message);
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
                throw new Exception("Create failed: " + ex.Message);
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
                throw new Exception("Update failed: " + ex.Message);
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
                throw new Exception("Delete failed: " + ex.Message);
            }
        }

        public async Task<UserProjectScoreDto> GetUserProjectScoreAsync(
    int projectId,
    int userId)
        {
            if (projectId <= 0 || userId <= 0)
                throw new ArgumentException("Invalid projectId or userId.");

            var rows = await _unitOfWork.PerformanceScoreRepository.Entities
                .AsNoTracking()
                .Where(ps =>
                    ps.ProjectId == projectId &&
                    ps.UserId == userId
                )
                .Select(ps => new
                {
                    ps.Score,
                    ps.MilestoneId,
                    MilestoneWeight = ps.Milestone.Weight,
                    TaskWeight = ps.Task.Weight
                })
                .ToListAsync();

            const decimal projectMaxScore = 100m;

            var milestoneScores = rows
                .GroupBy(x => new { x.MilestoneId, x.MilestoneWeight })
                .Select(g =>
                {
                    var earned = g.Sum(x =>
                        projectMaxScore *
                        x.MilestoneWeight *
                        x.TaskWeight *
                        (x.Score / 10m)  
                    );

                    return new MilestoneScoreDto
                    {
                        MilestoneId = g.Key.MilestoneId,
                        MilestoneWeight = g.Key.MilestoneWeight,
                        MilestoneMaxScore = g.Key.MilestoneWeight * projectMaxScore,
                        EarnedScore = Math.Round(earned, 2)
                    };
                })
                .ToList();

            return new UserProjectScoreDto
            {
                ProjectId = projectId,
                UserId = userId,
                TotalScore = Math.Round(milestoneScores.Sum(x => x.EarnedScore), 2),
                MaxScore = projectMaxScore,
                Milestones = milestoneScores
            };
        }

    }
}
