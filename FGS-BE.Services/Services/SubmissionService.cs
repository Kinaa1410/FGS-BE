using Azure.Core;
using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.DTOs.Submissions;
using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Enums;
using FGS_BE.Repo.Repositories.Interfaces;
using FGS_BE.Service.Interfaces;
using FGS_BE.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace FGS_BE.Service.Implements
{
    public class SubmissionService : ISubmissionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICloudinaryService _cloudinaryService;

        public SubmissionService(IUnitOfWork unitOfWork, ICloudinaryService cloudinaryService)
        {
            _unitOfWork = unitOfWork;
            _cloudinaryService = cloudinaryService;
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
            if (dto.File == null || dto.File.Length == 0)
            {
                throw new Exception("File is required.");
            }

            string fileExtension = Path.GetExtension(dto.File.FileName);
            string newFileName = $"{Guid.NewGuid()}{fileExtension}";

            var uploadResult = await _cloudinaryService.UploadImage(newFileName, dto.File);

            if (uploadResult == null)
            {
                throw new Exception("Error uploading file. Please try again.");
            }

            var entity = new Submission
            {
                TaskId = dto.TaskId,
                UserId = dto.UserId,
                FileUrl = uploadResult.ImageUrl,
                SubmittedAt = DateTime.UtcNow,
                Status = SubmissionStatus.Pending
            };

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

        //public async Task<SubmissionDto?> GradeSubmissionAsync(int submissionId, GradeSubmissionDto dto)
        //{
        //    var submission = await _unitOfWork.SubmissionRepository.FindByIdAsync(submissionId);
        //    if (submission == null)
        //        return null;

        //    submission.Grade = dto.Grade;
        //    submission.Feedback = dto.Feedback;
        //    submission.Status = SubmissionStatus.Graded;

        //    await _unitOfWork.SubmissionRepository.UpdateAsync(submission);
        //    await _unitOfWork.CommitAsync();

        //    return new SubmissionDto(submission);
        //}

        public async Task<SubmissionDto?> GradeSubmissionAsync(int submissionId, GradeSubmissionDto dto)
        {
            var submission = await _unitOfWork.SubmissionRepository.FindByAsync(
                x => x.Id == submissionId,
                q => q
                    .Include(s => s.Task)
                        .ThenInclude(t => t.Milestone)
                            .ThenInclude(m => m.Project)
            );

            if (submission == null)
                return null;

            submission.Grade = dto.Grade;
            submission.Feedback = dto.Feedback;
            submission.Status = SubmissionStatus.Graded;

            await _unitOfWork.SubmissionRepository.UpdateAsync(submission);

            var milestone = submission.Task.Milestone;
            var project = milestone.Project;

            var scoreEntity = new PerformanceScore
            {
                UserId = submission.UserId,
                TaskId = submission.TaskId,
                MilestoneId = milestone.Id,
                ProjectId = project.Id,
                Score = dto.Grade,
                Comment = dto.Feedback,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.PerformanceScoreRepository.CreateAsync(scoreEntity);

            await _unitOfWork.CommitAsync();

            return new SubmissionDto(submission);
        }

    }
}
