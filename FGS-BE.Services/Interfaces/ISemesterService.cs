using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.DTOs.Semesters;
using FGS_BE.Repo.Entities;

namespace FGS_BE.Services.Interfaces
{
    public interface ISemesterService
    {
        Task<PaginatedList<SemesterDto>> GetPagedAsync(
            int pageIndex,
            int pageSize,
            string? keyword = null,
            string? status = null,
            string? sortColumn = "Id",
            string? sortDir = "Asc");

        Task<SemesterDto?> GetByIdAsync(int id);
        Task<SemesterDto> CreateAsync(CreateSemesterDto dto);
        Task<SemesterDto?> UpdateAsync(int id, UpdateSemesterDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
