using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.DTOs.Submissions;
using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Repositories.Interfaces;
using FGS_BE.Service.Interfaces;

namespace FGS_BE.Service.Implements
{
    public class SubmissionService : ISubmissionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubmissionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginatedList<SubmissionDto>> GetPagedAsync(
            int pageIndex,
            int pageSize,
            int? userId = null,
            int? taskId = null,
            string? sortColumn = "Id",
            string? sortDir = "Asc")
        {
            var paged = await _unitOfWork.SubmissionRepository.GetPagedAsync(
                pageIndex, pageSize, userId, taskId, sortColumn, sortDir);

            return new PaginatedList<SubmissionDto>(
                paged.Select(x => new SubmissionDto(x)).ToList(),
                paged.TotalItems, paged.PageIndex, paged.PageSize);
        }

        public async Task<SubmissionDto?> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.SubmissionRepository.FindByIdAsync(id);
            return entity == null ? null : new SubmissionDto(entity);
        }

        public async Task<SubmissionDto> CreateAsync(CreateSubmissionDto dto)
        {
            var entity = dto.ToEntity();
            await _unitOfWork.SubmissionRepository.CreateAsync(entity);
            await _unitOfWork.CommitAsync();
            return new SubmissionDto(entity);
        }

        public async Task<SubmissionDto?> UpdateAsync(int id, UpdateSubmissionDto dto)
        {
            var entity = await _unitOfWork.SubmissionRepository.FindByIdAsync(id);
            if (entity == null) return null;

            entity.FileUrl = dto.FileUrl ?? entity.FileUrl;
            entity.Status = dto.Status ?? entity.Status;
            entity.Grade = dto.Grade ?? entity.Grade;
            entity.Feedback = dto.Feedback ?? entity.Feedback;
            entity.IsFinal = dto.IsFinal ?? entity.IsFinal;

            await _unitOfWork.SubmissionRepository.UpdateAsync(entity);
            await _unitOfWork.CommitAsync();
            return new SubmissionDto(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.SubmissionRepository.FindByIdAsync(id);
            if (entity == null) return false;
            await _unitOfWork.SubmissionRepository.DeleteAsync(entity);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
