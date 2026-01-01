using FGS_BE.Repo.DTOs.Milestones;
using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.DTOs.Semesters;
using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Repositories.Interfaces;
using FGS_BE.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FGS_BE.Services.Implements
{
    public class MilestoneService : IMilestoneService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MilestoneService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginatedList<MilestoneDto>> GetPagedAsync(
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
                throw new Exception("Không thể lấy danh sách milestone: " + ex.Message);
            }
        }

        public async Task<MilestoneDto?> GetByIdAsync(int id)
        {
            try
            {
                var entity = await _unitOfWork.MilestoneRepository.FindByIdAsync(id);
                return entity == null ? null : new MilestoneDto(entity);
            }
            catch (Exception ex)
            {
                throw new Exception("Không thể lấy thông tin milestone: " + ex.Message);
            }
        }

        public async Task<MilestoneDto> CreateAsync(CreateMilestoneDto dto)
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

                await _unitOfWork.MilestoneRepository.CreateAsync(entity);
                await _unitOfWork.CommitAsync();

                return new MilestoneDto(entity);
            }
            catch
            {
                throw;
            }
        }


        public async Task<MilestoneDto?> UpdateAsync(int id, UpdateMilestoneDto dto)
        {
            try
            {
                var entity = await _unitOfWork.MilestoneRepository.FindByIdAsync(id);
                if (entity == null) return null;

                if (dto.StartDate >= dto.DueDate)
                    throw new ArgumentException("DueDate must be greater than StartDate.");

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
                entity.Weight = dto.Weight;
                entity.Status = dto.Status ?? entity.Status;

                await _unitOfWork.MilestoneRepository.UpdateAsync(entity);
                await _unitOfWork.CommitAsync();

                return new MilestoneDto(entity);
            }
            catch
            {
                throw;
            }
        }


        public async Task<bool> DeleteAsync(int id)
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
                throw new Exception("Không thể xóa milestone: " + ex.Message);
            }
        }
    }
}
