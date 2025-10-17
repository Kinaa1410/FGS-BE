using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.DTOs.TermKeywords;
using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Repositories.Interfaces;
using FGS_BE.Service.Interfaces;

namespace FGS_BE.Service.Implements
{
    public class TermKeywordService : ITermKeywordService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TermKeywordService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginatedList<TermKeywordDto>> GetPagedAsync(int pageIndex, int pageSize, string? keyword, string? sortColumn, string? sortDir, int? semesterId)
        {
            var result = await _unitOfWork.TermKeywordRepository.GetPagedAsync(pageIndex, pageSize, keyword, sortColumn, sortDir, semesterId);
            return new PaginatedList<TermKeywordDto>(
                result.Select(x => new TermKeywordDto(x)).ToList(),
                result.TotalItems,
                result.PageIndex,
                result.PageSize
            );
        }

        public async Task<TermKeywordDto?> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.TermKeywordRepository.FindByIdAsync(id);
            return entity == null ? null : new TermKeywordDto(entity);
        }

        public async Task<TermKeywordDto> CreateAsync(CreateTermKeywordDto dto)
        {
            var entity = new TermKeyword
            {
                Keyword = dto.Keyword,
                BasePoints = dto.BasePoints,
                RuleBonus = dto.RuleBonus,
                SemesterId = dto.SemesterId
            };
            await _unitOfWork.TermKeywordRepository.CreateAsync(entity);
            await _unitOfWork.CommitAsync();
            return new TermKeywordDto(entity);
        }

        public async Task<TermKeywordDto?> UpdateAsync(int id, UpdateTermKeywordDto dto)
        {
            var entity = await _unitOfWork.TermKeywordRepository.FindByIdAsync(id);
            if (entity == null) return null;

            entity.Keyword = dto.Keyword;
            entity.BasePoints = dto.BasePoints;
            entity.RuleBonus = dto.RuleBonus;

            await _unitOfWork.TermKeywordRepository.UpdateAsync(entity);
            await _unitOfWork.CommitAsync();
            return new TermKeywordDto(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.TermKeywordRepository.FindByIdAsync(id);
            if (entity == null) return false;
            await _unitOfWork.TermKeywordRepository.DeleteAsync(entity);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
