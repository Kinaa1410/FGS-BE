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
using System.Linq.Expressions;

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
            try
            {
                var paged = await _unitOfWork.SubmissionRepository.GetPagedAsync(
                    pageIndex, pageSize, userId, taskId, sortColumn, sortDir);
                return new PaginatedList<SubmissionDto>(
                    paged.Select(x => new SubmissionDto(x)).ToList(),
                    paged.TotalItems, paged.PageIndex, paged.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("Không thể lấy danh sách submissions: " + ex.Message);
            }
        }

        public async Task<SubmissionDto?> GetByIdAsync(int id)
        {
            try
            {
                if (id <= 0) throw new ArgumentException("Invalid submission ID.");
                var entity = await _unitOfWork.SubmissionRepository.FindByIdAsync(id);
                return entity == null ? null : new SubmissionDto(entity);
            }
            catch (Exception ex)
            {
                throw new Exception("Không thể lấy thông tin submission: " + ex.Message);
            }
        }

        public async Task<SubmissionDto> CreateAsync(CreateSubmissionDto dto)
        {
            try
            {
                if (dto.File == null || dto.File.Length == 0)
                    throw new ArgumentException("File is required.");
                if (dto.TaskId <= 0)
                    throw new ArgumentException("TaskId is invalid.");
                if (dto.UserId <= 0)
                    throw new ArgumentException("UserId is invalid.");

                // Rule 1: Grace Period for Resubmission (72 hours, 1 attempt)
                var latestRejection = await _unitOfWork.SubmissionRepository.FindByAsync(
                    predicate: s => s.UserId == dto.UserId && s.TaskId == dto.TaskId && s.Status == SubmissionStatus.Rejected,
                    includeExpression: q => q.OrderByDescending(s => s.SubmittedAt).Take(1)
                );

                bool isResubmission = false;
                int version = 1;
                if (latestRejection != null)
                {
                    var hoursSinceRejection = (DateTime.UtcNow - latestRejection.RejectionDate!.Value).TotalHours;
                    if (hoursSinceRejection > 72)
                        throw new ArgumentException("Đã hết thời hạn 72 giờ để nộp lại submission. Vui lòng liên hệ quản lý dự án.");

                    var resubCount = await _unitOfWork.SubmissionRepository.CountAsync(
                        predicate: s => s.UserId == dto.UserId && s.TaskId == dto.TaskId && s.IsResubmission);
                    if (resubCount >= 1)
                        throw new ArgumentException("Chỉ được phép nộp lại 1 lần cho task này.");

                    isResubmission = true;
                    version = latestRejection.Version + 1;
                }

                string fileExtension = Path.GetExtension(dto.File.FileName);
                string newFileName = $"{Guid.NewGuid()}{fileExtension}";
                var uploadResult = await _cloudinaryService.UploadImage(newFileName, dto.File);
                if (uploadResult == null)
                    throw new Exception("Error uploading file. Please try again.");

                var entity = new Submission
                {
                    TaskId = dto.TaskId,
                    UserId = dto.UserId,
                    FileUrl = uploadResult.ImageUrl,
                    SubmittedAt = DateTime.UtcNow,
                    Status = SubmissionStatus.Pending,
                    IsResubmission = isResubmission,
                    Version = version
                };

                await _unitOfWork.SubmissionRepository.CreateAsync(entity);
                await _unitOfWork.CommitAsync();
                return new SubmissionDto(entity);
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
                throw new Exception("Không thể tạo submission: " + ex.Message);
            }
        }

        public async Task<SubmissionDto?> UpdateAsync(int id, UpdateSubmissionDto dto)
        {
            try
            {
                if (id <= 0) throw new ArgumentException("Invalid submission ID.");
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
            catch (Exception ex)
            {
                throw new Exception("Không thể cập nhật submission: " + ex.Message);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                if (id <= 0) throw new ArgumentException("Invalid submission ID.");
                var entity = await _unitOfWork.SubmissionRepository.FindByIdAsync(id);
                if (entity == null) return false;
                await _unitOfWork.SubmissionRepository.DeleteAsync(entity);
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Không thể xóa submission: " + ex.Message);
            }
        }

        public async Task<SubmissionDto?> ReviewSubmissionAsync(int submissionId, ReviewSubmissionDto dto)
        {
            try
            {
                if (submissionId <= 0)
                    throw new ArgumentException("Invalid submission ID.");
                var decision = dto.Decision?.ToLower(); 
                if (decision != "approve" && decision != "reject")
                    throw new ArgumentException("Decision must be 'approve' or 'reject'.");
                if (!string.IsNullOrEmpty(dto.Feedback) && dto.Feedback.Length > 500)
                    throw new ArgumentException("Feedback must be less than 500 characters.");
                if (decision == "approve" && (!dto.Score.HasValue || dto.Score < 0 || dto.Score > 10))
                    throw new ArgumentException("Score must be between 0 and 10 when approving.");
                var submission = await _unitOfWork.SubmissionRepository.FindByAsync(
                    predicate: x => x.Id == submissionId,
                    includeExpression: q => q
                        .Include(s => s.Task)!
                        .ThenInclude(t => t.Milestone)!
                        .ThenInclude(m => m.Project)!
                        .ThenInclude(p => p.ProjectMembers)
                );
                if (submission == null)
                    return null;
                submission.Feedback = dto.Feedback;
                await using var transaction = await _unitOfWork.BeginTransactionAsync();
                if (decision == "approve")
                {
                    submission.Status = SubmissionStatus.Approved;
                    submission.Grade = dto.Score.Value;
                    submission.IsFinal = true;
                    var milestone = submission.Task!.Milestone;
                    var project = milestone.Project;
                    var scoreEntity = new PerformanceScore
                    {
                        UserId = submission.UserId,
                        TaskId = submission.TaskId,
                        MilestoneId = milestone.Id,
                        ProjectId = project.Id,
                        Score = dto.Score.Value,
                        Comment = dto.Feedback,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _unitOfWork.PerformanceScoreRepository.CreateAsync(scoreEntity);
                }
                else
                {
                    submission.Status = SubmissionStatus.Rejected;
                    submission.Grade = null;
                    submission.RejectionDate = DateTime.UtcNow;  // Starts grace period clock

                    // Rule 2: Team Milestone Delay Buffer (conditional on mentor choice)
                    var project = submission.Task!.Milestone.Project;
                    var shouldExtend = dto.ExtendDeadline ?? true;
                    if (shouldExtend && project.ProjectMembers.Count == 4 && !submission.Task.Milestone.IsDelayed)
                    {
                        var milestone = submission.Task.Milestone;
                        milestone.OriginalDueDate ??= milestone.DueDate;
                        milestone.DueDate = milestone.DueDate.AddHours(36);
                        milestone.IsDelayed = true;
                        await _unitOfWork.MilestoneRepository.UpdateAsync(milestone);
                    }
                    // Rule 3: Escalation Threshold
                    var projectId = project.Id;
                    var stats = await _unitOfWork.UserProjectStatsRepository.FindByAsync(
                        predicate: s => s.UserId == submission.UserId && s.ProjectId == projectId
                    );
                    if (stats == null)
                    {
                        stats = new UserProjectStats
                        {
                            UserId = submission.UserId,
                            ProjectId = projectId,
                            FailureCount = 1
                        };
                        await _unitOfWork.UserProjectStatsRepository.CreateAsync(stats);
                    }
                    else
                    {
                        stats.FailureCount++;
                        stats.UpdatedAt = DateTime.UtcNow;
                        await _unitOfWork.UserProjectStatsRepository.UpdateAsync(stats);
                    }
                    if (stats.FailureCount >= 2)
                    {
                        submission.Feedback += $"\n[WARNING: Reached failure threshold {stats.FailureCount}/2. Performance review required with project mentor.]";
                    }
                }
                await _unitOfWork.SubmissionRepository.UpdateAsync(submission);
                await _unitOfWork.CommitAsync();
                await transaction.CommitAsync();
                return new SubmissionDto(submission);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to review submission: " + ex.Message);
            }
        }
    }
}