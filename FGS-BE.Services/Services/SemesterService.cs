using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.DTOs.Semesters;
using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Repositories.Implements;
using FGS_BE.Repo.Repositories.Interfaces;
using FGS_BE.Services.Interfaces;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace FGS_BE.Services.Services
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
            if (dto.EndDate <= dto.StartDate)
            {
                throw new Exception("Ngày kết thúc không thể nhỏ hơn hoặc bằng ngày bắt đầu.");
            }

            var overlappingSemester = await _unitOfWork.SemesterRepository.Entities
                .AsNoTracking()
                .Where(x =>
                    (dto.StartDate >= x.StartDate && dto.StartDate <= x.EndDate) || 
                    (dto.EndDate >= x.StartDate && dto.EndDate <= x.EndDate) ||  
                    (dto.StartDate <= x.StartDate && dto.EndDate >= x.EndDate))     
                .FirstOrDefaultAsync();

            if (overlappingSemester != null)
            {
                throw new Exception("Thời gian của học kỳ bị trùng với một học kỳ đã tồn tại.");
            }

            var entity = dto.ToEntity();

            await _unitOfWork.SemesterRepository.CreateAsync(entity);
            await _unitOfWork.CommitAsync();

            return new SemesterDto(entity);
        }



        public async Task<SemesterDto?> UpdateAsync(int id, UpdateSemesterDto dto)
        {
            var entity = await _unitOfWork.SemesterRepository.FindByIdAsync(id);
            if (entity == null) return null;

            dto.ApplyToEntity(entity);
            _unitOfWork.SemesterRepository.UpdateAsync(entity);
            await _unitOfWork.CommitAsync();

            return new SemesterDto(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.SemesterRepository.FindByIdAsync(id);
            if (entity == null) return false;

            _unitOfWork.SemesterRepository.DeleteAsync(entity);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
