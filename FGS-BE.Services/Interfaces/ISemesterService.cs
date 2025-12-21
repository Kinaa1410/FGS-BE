using FGS_BE.Repo.DTOs.Semesters;
using FGS_BE.Repo.DTOs.Pages;
using System;
using System.Threading.Tasks;  // Explicit for Task

namespace FGS_BE.Service.Interfaces
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
        // FIXED: Explicit System.Threading.Tasks.Task to avoid ambiguity
        System.Threading.Tasks.Task<string> GetSemesterStatusAsync(int semesterId);
        System.Threading.Tasks.Task SyncSemesterStatusAsync(int semesterId);
    }
}