using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.DTOs.TermKeywords;

namespace FGS_BE.Service.Interfaces
{
    public interface ITermKeywordService
    {
        Task<PaginatedList<TermKeywordDto>> GetPagedAsync(
            int pageIndex,
            int pageSize,
            string? keyword = null,
            string? sortColumn = "Id",
            string? sortDir = "Asc",
            int? semesterId = null);

        Task<TermKeywordDto?> GetByIdAsync(int id);
        Task<TermKeywordDto> CreateAsync(CreateTermKeywordDto dto);
        Task<TermKeywordDto?> UpdateAsync(int id, UpdateTermKeywordDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
