using FGS_BE.Repo.DTOs.Milestones;
using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.DTOs.Semesters;
using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Repositories.Interfaces;
using FGS_BE.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;  // Explicit for Task to avoid ambiguity

namespace FGS_BE.Services.Implements
{
    public class MilestoneService : IMilestoneService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MilestoneService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async System.Threading.Tasks.Task<PaginatedList<MilestoneDto>> GetPagedAsync(
            int pageIndex,
            int pageSize,
            string? keyword = null,
            string? status = null,
            int? projectId = null,
            string? sortColumn = "Id",
            string? sortDir = "Asc")
        {
            try
            {
                var paged = await _unitOfWork.MilestoneRepository.GetPagedAsync(
                    pageIndex, pageSize, keyword, status, projectId, sortColumn, sortDir);
                return new PaginatedList<MilestoneDto>(
                    paged.Select(x => new MilestoneDto(x)).ToList(),
                    paged.TotalItems,
                    paged.PageIndex,
                    paged.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to retrieve milestone list: " + ex.Message);
            }
        }

        public async System.Threading.Tasks.Task<MilestoneDto?> GetByIdAsync(int id)
        {
            try
            {
                var entity = await _unitOfWork.MilestoneRepository.FindByIdAsync(id);
                return entity == null ? null : new MilestoneDto(entity);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to retrieve milestone information: " + ex.Message);
            }
        }

        public async System.Threading.Tasks.Task<MilestoneDto> CreateAsync(CreateMilestoneDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Title))
                    throw new ArgumentException("Title is required.");
                if (dto.Weight < 0)
                    throw new ArgumentException("Weight must be >= 0.");
                if (dto.DueDate <= dto.StartDate)
                    throw new ArgumentException("The DueDate must be greater than the StartDate.");

                var project = await _unitOfWork.ProjectRepository.FindByIdAsync(dto.ProjectId);
                if (project == null)
                    throw new InvalidOperationException("Project does not exist.");

                var overlappingMilestone = await _unitOfWork.MilestoneRepository.Entities
                    .AsNoTracking()
                    .Where(x =>
                        x.ProjectId == dto.ProjectId &&
                        (
                            (dto.StartDate >= x.StartDate && dto.StartDate <= x.DueDate) ||
                            (dto.DueDate >= x.StartDate && dto.DueDate <= x.DueDate) ||
                            (dto.StartDate <= x.StartDate && dto.DueDate >= x.DueDate)
                        )
                    )
                    .FirstOrDefaultAsync();
                if (overlappingMilestone != null)
                    throw new InvalidOperationException("The milestone duration overlaps with existing milestone.");

                var entity = dto.ToEntity();
                entity.IsDelayed = false;  // Default for new milestones
                entity.OriginalDueDate = null;  // No original on create

                await _unitOfWork.MilestoneRepository.CreateAsync(entity);
                await _unitOfWork.CommitAsync();
                return new MilestoneDto(entity);
            }
            catch
            {
                throw;
            }
        }

        public async System.Threading.Tasks.Task<MilestoneDto?> UpdateAsync(int id, UpdateMilestoneDto dto)
        {
            try
            {
                var entity = await _unitOfWork.MilestoneRepository.FindByIdAsync(id);
                if (entity == null) return null;
                if (dto.StartDate >= dto.DueDate)
                    throw new ArgumentException("DueDate must be greater than StartDate.");
                if (dto.Weight.GetValueOrDefault() < 0)
                    throw new ArgumentException("Weight must be >= 0.");

                var overlappingMilestone = await _unitOfWork.MilestoneRepository.Entities
                    .AsNoTracking()
                    .Where(x =>
                        x.ProjectId == entity.ProjectId &&
                        x.Id != id &&
                        (
                            (dto.StartDate >= x.StartDate && dto.StartDate <= x.DueDate) ||
                            (dto.DueDate >= x.StartDate && dto.DueDate <= x.DueDate) ||
                            (dto.StartDate <= x.StartDate && dto.DueDate >= x.DueDate)
                        )
                    )
                    .FirstOrDefaultAsync();
                if (overlappingMilestone != null)
                    throw new InvalidOperationException("The milestone duration overlaps with another milestone.");

                entity.Title = dto.Title ?? entity.Title;
                entity.Description = dto.Description ?? entity.Description;
                entity.StartDate = dto.StartDate;
                entity.DueDate = dto.DueDate;
                entity.Weight = dto.Weight.GetValueOrDefault(entity.Weight);  // Handle nullable decimal
                entity.Status = dto.Status ?? entity.Status;
                entity.IsDelayed = dto.IsDelayed ?? entity.IsDelayed;  // New: Update if provided
                entity.OriginalDueDate = dto.OriginalDueDate ?? entity.OriginalDueDate;  // New: Allow update

                await _unitOfWork.MilestoneRepository.UpdateAsync(entity);
                await _unitOfWork.CommitAsync();
                return new MilestoneDto(entity);
            }
            catch
            {
                throw;
            }
        }

        public async System.Threading.Tasks.Task<bool> DeleteAsync(int id)
        {
            try
            {
                var entity = await _unitOfWork.MilestoneRepository.FindByIdAsync(id);
                if (entity == null) return false;
                await _unitOfWork.MilestoneRepository.DeleteAsync(entity);
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to delete milestone: " + ex.Message);
            }
        }

        // New: Manual delay buffer application (for mentor/admin, e.g., via API endpoint)
        public async System.Threading.Tasks.Task ApplyDelayBufferAsync(int milestoneId, int hours = 36)
        {
            try
            {
                var entity = await _unitOfWork.MilestoneRepository.FindByIdAsync(milestoneId);
                if (entity == null || entity.IsDelayed)
                    return;  // Already delayed or not found

                entity.OriginalDueDate ??= entity.DueDate;
                entity.DueDate = entity.DueDate.AddHours(hours);
                entity.IsDelayed = true;

                await _unitOfWork.MilestoneRepository.UpdateAsync(entity);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to apply delay buffer to milestone: " + ex.Message);
            }
        }

        // New: Reset delay (for appeals, testing, or manual override)
        public async System.Threading.Tasks.Task ResetDelayAsync(int milestoneId)
        {
            try
            {
                var entity = await _unitOfWork.MilestoneRepository.FindByIdAsync(milestoneId);
                if (entity == null)
                    return;

                entity.DueDate = entity.OriginalDueDate ?? entity.DueDate;
                entity.IsDelayed = false;

                await _unitOfWork.MilestoneRepository.UpdateAsync(entity);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to reset delay for milestone: " + ex.Message);
            }
        }
    }
}