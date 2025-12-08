using System;
using System.Linq;
using System.Threading.Tasks; // Explicit use of async Task
using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.DTOs.Projects;
using FGS_BE.Repo.Enums;
using FGS_BE.Repo.Repositories.Interfaces;
using FGS_BE.Service.Interfaces;

// Alias your Task entity to avoid name conflict
using ProjectTask = FGS_BE.Repo.Entities.Task;

namespace FGS_BE.Service.Implements
{
    public class ProjectService : IProjectService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProjectService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ============================================================
        // GET PAGED
        // ============================================================
        public async System.Threading.Tasks.Task<PaginatedList<ProjectDto>> GetPagedAsync(
            int pageIndex,
            int pageSize,
            string? keyword = null,
            string? status = null,
            string? sortColumn = "Id",
            string? sortDir = "Asc")
        {
            var paged = await _unitOfWork.ProjectRepository.GetPagedAsync(
                pageIndex, pageSize, keyword, status, sortColumn, sortDir);

            var list = paged.Select(x => new ProjectDto(x)).ToList();

            return new PaginatedList<ProjectDto>(
                list,
                paged.TotalItems,
                paged.PageIndex,
                paged.PageSize
            );
        }

        // ============================================================
        // GET BY ID
        // ============================================================
        public async System.Threading.Tasks.Task<ProjectDto?> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.ProjectRepository.FindByIdAsync(id);
            return entity == null ? null : new ProjectDto(entity);
        }

        // ============================================================
        // CREATE
        // ============================================================
        public async System.Threading.Tasks.Task<ProjectDto> CreateAsync(CreateProjectDto dto)
        {
            await ValidateProposerAsync(dto.ProposerId);
            await ValidateSemesterAsync(dto.SemesterId);
            var exists = await _unitOfWork.ProjectRepository.ExistsByAsync(p => p.Title == dto.Title);

            if (exists)
                throw new InvalidOperationException($"A project with title '{dto.Title}' already exists.");
            if (dto.MentorId.HasValue)
                await ValidateMentorAsync(dto.MentorId.Value);

            var entity = dto.ToEntity();
            await _unitOfWork.ProjectRepository.CreateAsync(entity);
            await _unitOfWork.CommitAsync();

            var result = await _unitOfWork.ProjectRepository.FindByIdAsync(entity.Id);
            return new ProjectDto(result);
        }

        // ============================================================
        // UPDATE
        // ============================================================
        public async System.Threading.Tasks.Task<ProjectDto?> UpdateAsync(int id, UpdateProjectDto dto)
        {
            var entity = await _unitOfWork.ProjectRepository.FindByIdAsync(id);
            if (entity == null) return null;

            if (dto.Title != null) entity.Title = dto.Title;
            if (dto.Description != null) entity.Description = dto.Description;
            if (dto.TotalPoints.HasValue) entity.TotalPoints = dto.TotalPoints.Value;
            if (dto.MinMembers.HasValue) entity.MinMembers = dto.MinMembers.Value;
            if (dto.MaxMembers.HasValue) entity.MaxMembers = dto.MaxMembers.Value;

            if (dto.MentorId.HasValue)
            {
                await ValidateMentorAsync(dto.MentorId.Value);
                entity.MentorId = dto.MentorId.Value;
            }

            if (dto.Status != null)
            {
                if (!Enum.TryParse(dto.Status, true, out ProjectStatus newStatus))
                    throw new ArgumentException($"Invalid status: {dto.Status}.");

                // Check member requirement for InProcess
                if (newStatus == ProjectStatus.InProcess &&
                    entity.CurrentMembers < entity.MinMembers)
                {
                    throw new InvalidOperationException(
                        $"Need at least {entity.MinMembers} members to set InProcess.");
                }

                entity.Status = newStatus;
            }

            await _unitOfWork.ProjectRepository.UpdateAsync(entity);
            await _unitOfWork.CommitAsync();

            return new ProjectDto(entity);
        }

        // ============================================================
        // DELETE
        // ============================================================
        public async System.Threading.Tasks.Task<bool> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.ProjectRepository.FindByIdAsync(id);
            if (entity == null) return false;

            await _unitOfWork.ProjectRepository.DeleteAsync(entity);
            await _unitOfWork.CommitAsync();
            return true;
        }

        // ============================================================
        // VALIDATION METHODS (NO RETURNS → FIXED)
        // ============================================================
        private async System.Threading.Tasks.Task ValidateProposerAsync(int proposerId)
        {
            var p = await _unitOfWork.UserRepository.FindByIdAsync(proposerId);
            if (p == null)
                throw new ArgumentException("Proposer not found.", nameof(proposerId));
        }

        private async System.Threading.Tasks.Task ValidateSemesterAsync(int semesterId)
        {
            var semester = await _unitOfWork.SemesterRepository.FindByIdAsync(semesterId);
            if (semester == null)
                throw new ArgumentException("Semester not found.", nameof(semesterId));
        }

        private async System.Threading.Tasks.Task ValidateMentorAsync(int mentorId)
        {
            var mentor = await _unitOfWork.UserRepository.FindByIdAsync(mentorId);
            if (mentor == null)
                throw new ArgumentException("Mentor not found.", nameof(mentorId));
        }
    }
}
