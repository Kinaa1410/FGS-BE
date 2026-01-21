using System;
using System.Linq;
using System.Threading.Tasks;  // Explicit for Task (fixes ambiguity)
using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.DTOs.Semesters;
using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Repositories.Interfaces;
using FGS_BE.Service.Interfaces;
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
                pageIndex, pageSize, keyword, status, sortColumn, sortDir);
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
            // 1. Validate ngày tháng
            if (dto.StartDate == default || dto.EndDate == default)
                throw new ArgumentException("StartDate and EndDate must be provided and valid.");

            var now = DateTime.UtcNow;

            if (dto.StartDate <= now)
                throw new ArgumentException("StartDate must be in the future.");

            if (dto.EndDate <= dto.StartDate)
                throw new ArgumentException("EndDate must be after StartDate.");

            const int MinDurationDays = 70;
            if ((dto.EndDate - dto.StartDate).TotalDays < MinDurationDays)
                throw new InvalidOperationException($"Semester duration must be at least {MinDurationDays} days.");

            // 2. Validate Name (bắt buộc hoặc unique nếu có)
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Semester name is required.");

            var existingName = await _unitOfWork.SemesterRepository.Entities
                .AsNoTracking()
                .AnyAsync(x => x.Name == dto.Name.Trim());

            if (existingName)
                throw new InvalidOperationException($"Semester name '{dto.Name}' already exists.");

            // 3. Kiểm tra overlap thời gian với bất kỳ semester nào (không phụ thuộc Status DB)
            var overlappingSemester = await _unitOfWork.SemesterRepository.Entities
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    (dto.StartDate >= x.StartDate && dto.StartDate <= x.EndDate) ||
                    (dto.EndDate >= x.StartDate && dto.EndDate <= x.EndDate) ||
                    (dto.StartDate <= x.StartDate && dto.EndDate >= x.EndDate));

            if (overlappingSemester != null)
                throw new InvalidOperationException(
                    $"The new semester overlaps with existing semester '{overlappingSemester.Name}' " +
                    $"(from {overlappingSemester.StartDate:yyyy-MM-dd} to {overlappingSemester.EndDate:yyyy-MM-dd}).");

            // 4. Tạo entity – KHÔNG dùng dto.Status nữa
            var entity = dto.ToEntity();

            entity.Status = "Upcoming";
            entity.CreatedAt = now;

            await _unitOfWork.SemesterRepository.CreateAsync(entity);
            await _unitOfWork.CommitAsync();

            // Trả về DTO với status được tính realtime (Upcoming ngay lập tức vì StartDate > now)
            return new SemesterDto(entity);
        }

        public async Task<SemesterDto?> UpdateAsync(int id, UpdateSemesterDto dto)
        {
            var entity = await _unitOfWork.SemesterRepository.FindByIdAsync(id);
            if (entity == null) return null;

            if (dto.Status != null)
            {
                var validStatuses = new[] { "Upcoming", "Active", "Inactive", "Planned", "Closed" };
                if (!validStatuses.Contains(dto.Status))
                    throw new InvalidOperationException($"Invalid status '{dto.Status}'. Must be one of: {string.Join(", ", validStatuses)}.");
            }

            dto.ApplyToEntity(entity);
            // FIXED: Await to ensure completion
            await SyncSemesterStatusAsync(id);
            await _unitOfWork.SemesterRepository.UpdateAsync(entity);
            await _unitOfWork.CommitAsync();
            return new SemesterDto(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.SemesterRepository.FindByIdAsync(id);
            if (entity == null) return false;

            var hasProjects = await _unitOfWork.ProjectRepository.Entities
                .AnyAsync(p => p.SemesterId == id);
            if (hasProjects)
                throw new InvalidOperationException("Cannot delete semester with associated projects.");

            await _unitOfWork.SemesterRepository.DeleteAsync(entity);
            await _unitOfWork.CommitAsync();
            return true;
        }

        // FIXED: Explicit System.Threading.Tasks.Task<string> to match interface & avoid ambiguity
        public async System.Threading.Tasks.Task<string> GetSemesterStatusAsync(int semesterId)
        {
            var semester = await _unitOfWork.SemesterRepository.FindByIdAsync(semesterId);
            if (semester == null) throw new ArgumentException("Semester not found.", nameof(semesterId));

            var now = DateTime.UtcNow;
            if (now < semester.StartDate) return "Upcoming";
            if (now > semester.EndDate) return "Closed";
            return "Active";
        }

        // FIXED: Explicit System.Threading.Tasks.Task (no return value), all paths complete (no early exit)
        public async System.Threading.Tasks.Task SyncSemesterStatusAsync(int semesterId)
        {
            var computedStatus = await GetSemesterStatusAsync(semesterId);
            var entity = await _unitOfWork.SemesterRepository.FindByIdAsync(semesterId);
            if (entity != null && entity.Status != computedStatus)
            {
                entity.Status = computedStatus;
                await _unitOfWork.SemesterRepository.UpdateAsync(entity);
                await _unitOfWork.CommitAsync();
            }
            // No explicit return - async Task completes here
        }
    }
}