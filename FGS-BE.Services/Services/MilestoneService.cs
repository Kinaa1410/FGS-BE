using FGS_BE.Repo.DTOs.Milestones;
using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.DTOs.Semesters;
using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Repositories.Interfaces;
using FGS_BE.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FGS_BE.Service.Implements
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
            var paged = await _unitOfWork.MilestoneRepository.GetPagedAsync(
                pageIndex, pageSize, keyword, status, projectId, sortColumn, sortDir);

            return new PaginatedList<MilestoneDto>(
                paged.Select(x => new MilestoneDto(x)).ToList(),
                paged.TotalItems,
                paged.PageIndex,
                paged.PageSize);
        }


        public async Task<MilestoneDto?> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.MilestoneRepository.FindByIdAsync(id);
            return entity == null ? null : new MilestoneDto(entity);
        }

        public async Task<MilestoneDto> CreateAsync(CreateMilestoneDto dto)
        {
            if (dto.DueDate <= dto.StartDate)
            {
                throw new Exception("The DueDate cannot be less than or equal to the StartDate.");
            }

            var overlappingMilestone = await _unitOfWork.MilestoneRepository.Entities
                .AsNoTracking()
                .Where(x =>
                    (dto.StartDate >= x.StartDate && dto.StartDate <= x.DueDate) ||
                    (dto.DueDate >= x.StartDate && dto.DueDate <= x.DueDate) ||
                    (dto.StartDate <= x.StartDate && dto.DueDate >= x.DueDate))
                .FirstOrDefaultAsync();

            if (overlappingMilestone != null)
            {
                throw new Exception("The duration of this semester coincides with the current semester.");
            }

            var entity = dto.ToEntity();

            await _unitOfWork.MilestoneRepository.CreateAsync(entity);
            await _unitOfWork.CommitAsync();

            return new MilestoneDto(entity);
        }

        public async Task<MilestoneDto?> UpdateAsync(int id, UpdateMilestoneDto dto)
        {
            var entity = await _unitOfWork.MilestoneRepository.FindByIdAsync(id);
            if (entity == null) return null;

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

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.MilestoneRepository.FindByIdAsync(id);
            if (entity == null) return false;

            await _unitOfWork.MilestoneRepository.DeleteAsync(entity);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
