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

        public async Task<PaginatedList<TermKeywordDto>> GetPagedAsync(
            int pageIndex,
            int pageSize,
            string? keyword,
            string? sortColumn,
            string? sortDir,
            int? semesterId)
        {
            try
            {
                var result = await _unitOfWork.TermKeywordRepository.GetPagedAsync(
                    pageIndex, pageSize, keyword, sortColumn, sortDir, semesterId);

                return new PaginatedList<TermKeywordDto>(
                    result.Select(x => new TermKeywordDto(x)).ToList(),
                    result.TotalItems,
                    result.PageIndex,
                    result.PageSize
                );
            }
            catch (Exception ex)
            {
                throw new Exception("Không thể lấy danh sách term keyword: " + ex.Message);
            }
        }

        public async Task<TermKeywordDto?> GetByIdAsync(int id)
        {
            try
            {
                var entity = await _unitOfWork.TermKeywordRepository.FindByIdAsync(id);
                return entity == null ? null : new TermKeywordDto(entity);
            }
            catch (Exception ex)
            {
                throw new Exception("Không thể lấy thông tin term keyword: " + ex.Message);
            }
        }

        public async Task<TermKeywordDto> CreateAsync(CreateTermKeywordDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Keyword))
                    throw new ArgumentException("Keyword is required.");

                if (dto.BasePoints < 0)
                    throw new ArgumentException("BasePoints must be >= 0.");

                if (dto.RuleBonus < 0)
                    throw new ArgumentException("RuleBonus must be >= 0.");

                // Optional – bật nếu cần validate semester tồn tại
                // var semester = await _unitOfWork.SemesterRepository.FindByIdAsync(dto.SemesterId);
                // if (semester == null) throw new InvalidOperationException("Semester does not exist.");

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
            catch (ArgumentException)
            {
                throw;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Không thể tạo term keyword: " + ex.Message);
            }
        }

        public async Task<TermKeywordDto?> UpdateAsync(int id, UpdateTermKeywordDto dto)
        {
            try
            {
                var entity = await _unitOfWork.TermKeywordRepository.FindByIdAsync(id);
                if (entity == null) return null;

                if (dto.BasePoints < 0)
                    throw new ArgumentException("BasePoints must be >= 0.");

                if (dto.RuleBonus < 0)
                    throw new ArgumentException("RuleBonus must be >= 0.");

                entity.Keyword = dto.Keyword;
                entity.BasePoints = dto.BasePoints;
                entity.RuleBonus = dto.RuleBonus;

                await _unitOfWork.TermKeywordRepository.UpdateAsync(entity);
                await _unitOfWork.CommitAsync();
                return new TermKeywordDto(entity);
            }
            catch (Exception ex)
            {
                throw new Exception("Không thể cập nhật term keyword: " + ex.Message);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var entity = await _unitOfWork.TermKeywordRepository.FindByIdAsync(id);
                if (entity == null) return false;

                await _unitOfWork.TermKeywordRepository.DeleteAsync(entity);
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Không thể xóa term keyword: " + ex.Message);
            }
        }
    }
}
