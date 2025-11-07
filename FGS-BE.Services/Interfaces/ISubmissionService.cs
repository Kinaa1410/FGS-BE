using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.DTOs.Submissions;
using Microsoft.AspNetCore.Http;

namespace FGS_BE.Service.Interfaces
{
    public interface ISubmissionService
    {
        Task<PaginatedList<SubmissionDto>> GetPagedAsync(
            int pageIndex,
            int pageSize,
            int? userId = null,
            int? taskId = null,
            string? sortColumn = "Id",
            string? sortDir = "Asc");

        Task<SubmissionDto?> GetByIdAsync(int id);
        Task<SubmissionDto> CreateAsync(CreateSubmissionDto dto);
        Task<SubmissionDto?> UpdateAsync(int id, UpdateSubmissionDto dto);
        Task<bool> DeleteAsync(int id);
        Task<SubmissionDto?> GradeSubmissionAsync(int submissionId, GradeSubmissionDto dto);
    }
}
