using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.DTOs.Semesters;
using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Repositories.Implements;
using FGS_BE.Repo.Repositories.Interfaces;
using FGS_BE.Services.Interfaces;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace FGS_BE.Service.Services
{
    public class SemesterService : ISemesterService
    {
        private readonly IUnitOfWork _unitOfWork;
        public SemesterService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<PaginatedList<SemesterDto>> GetPagedAsync(
            int pageIndex,
            int pageSize,
            string? keyword = null,
            string? status = null,
            string? sortColumn = "Id",
            string? sortDir = "Asc")
        {
            var pagedEntities = await _unitOfWork.SemesterRepository.GetPagedAsync(
                pageIndex,
                pageSize,
                keyword,
                status,
                sortColumn,
                sortDir
            );

            var pagedDtos = new PaginatedList<SemesterDto>(
                pagedEntities.Select(x => new SemesterDto(x)).ToList(),
                pagedEntities.TotalItems,
                pagedEntities.PageIndex,
                pagedEntities.PageSize
            );

            return pagedDtos;
        }

        public async Task<SemesterDto?> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.SemesterRepository.FindByIdAsync(id);
            return entity == null ? null : new SemesterDto(entity);
        }

        public async Task<SemesterDto> CreateAsync(CreateSemesterDto dto)
        {
            // Rule 1: Valid Date Range
            if (dto.StartDate == default || dto.EndDate == default)
                throw new ArgumentException("StartDate and EndDate must be provided and valid.");

            var now = DateTime.UtcNow;
            if (dto.StartDate <= now)
                throw new ArgumentException("StartDate must be in the future.");

            // Rule 2: EndDate > StartDate 
            if (dto.EndDate <= dto.StartDate)
                throw new ArgumentException("EndDate must be after StartDate.");

            // Rule 3: Minimum Duration
            const int MinDurationDays = 70;
            if ((dto.EndDate - dto.StartDate).TotalDays < MinDurationDays)
                throw new InvalidOperationException($"Semester duration must be at least {MinDurationDays} days.");

            // Rule 4: Unique Name/Code 
            if (!string.IsNullOrWhiteSpace(dto.Name))
            {
                var existingName = await _unitOfWork.SemesterRepository.Entities
                    .AsNoTracking()
                    .AnyAsync(x => x.Name == dto.Name);
                if (existingName)
                    throw new InvalidOperationException($"Semester name '{dto.Name}' already exists.");
            }

            // Rule 5: Valid Status 
            var status = dto.Status ?? "Upcoming"; 
            var validStatuses = new[] { "Upcoming", "Active", "Inactive", "Planned" }; 
            if (!validStatuses.Contains(status))
                throw new InvalidOperationException($"Invalid status '{status}'. Must be one of: {string.Join(", ", validStatuses)}.");

            // Rule 6: No Overlaps 
            var overlappingSemester = await _unitOfWork.SemesterRepository.Entities
                .AsNoTracking()
                .Where(x => x.Status == "Active")
                .Where(x =>
                    (dto.StartDate >= x.StartDate && dto.StartDate <= x.EndDate) ||
                    (dto.EndDate >= x.StartDate && dto.EndDate <= x.EndDate) ||
                    (dto.StartDate <= x.StartDate && dto.EndDate >= x.EndDate))
                .FirstOrDefaultAsync();
            if (overlappingSemester != null)
                throw new InvalidOperationException("The duration of this semester overlaps with an active semester.");

            var entity = dto.ToEntity();
            entity.Status = status;

            await _unitOfWork.SemesterRepository.CreateAsync(entity);
            await _unitOfWork.CommitAsync();
            return new SemesterDto(entity);
        }



        public async Task<SemesterDto?> UpdateAsync(int id, UpdateSemesterDto dto)
        {
            var entity = await _unitOfWork.SemesterRepository.FindByIdAsync(id);
            if (entity == null) return null;

            dto.ApplyToEntity(entity);
            await _unitOfWork.SemesterRepository.UpdateAsync(entity);
            await _unitOfWork.CommitAsync();

            return new SemesterDto(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.SemesterRepository.FindByIdAsync(id);
            if (entity == null) return false;

            await _unitOfWork.SemesterRepository.DeleteAsync(entity);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
