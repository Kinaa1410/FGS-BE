using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.DTOs.Projects;
using FGS_BE.Repo.Enums;
using FGS_BE.Repo.Repositories.Interfaces;
using FGS_BE.Service.Interfaces;
using FGS_BE.Services.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;
using ProjectTask = FGS_BE.Repo.Entities.Task;

namespace FGS_BE.Service.Implements
{
    public class ProjectService : IProjectService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISemesterService _semesterService;

        public ProjectService(IUnitOfWork unitOfWork, ISemesterService semesterService)
        {
            _unitOfWork = unitOfWork;
            _semesterService = semesterService;
        }

        public async Task<PaginatedList<ProjectDto>> GetPagedAsync(
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

        public async Task<ProjectDto?> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.ProjectRepository.FindByIdAsync(id);
            return entity == null ? null : new ProjectDto(entity);
        }

        public async Task<ProjectDto> CreateAsync(CreateProjectDto dto)
        {
            await ValidateProposerAsync(dto.ProposerId);
            await ValidateSemesterAsync(dto.SemesterId);
            var exists = await _unitOfWork.ProjectRepository.ExistsByAsync(p => p.Title == dto.Title);
            if (exists)
                throw new InvalidOperationException($"A project with title '{dto.Title}' already exists.");
            if (dto.MentorId.HasValue)
                await ValidateMentorAsync(dto.MentorId.Value);

            var semesterStatus = await _semesterService.GetSemesterStatusAsync(dto.SemesterId);
            if (semesterStatus != "Upcoming" && semesterStatus != "Active")
                throw new InvalidOperationException($"Cannot create project: Semester is {semesterStatus}.");

            var entity = dto.ToEntity();
            entity.Status = ProjectStatus.Open;
            await _unitOfWork.ProjectRepository.CreateAsync(entity);
            await _unitOfWork.CommitAsync();
            var result = await _unitOfWork.ProjectRepository.FindByIdAsync(entity.Id);
            return new ProjectDto(result);
        }

        public async Task<ProjectDto?> UpdateAsync(int id, UpdateProjectDto dto)
        {
            var entity = await _unitOfWork.ProjectRepository.FindByIdAsync(id);
            if (entity == null) return null;

            var semesterStatus = await _semesterService.GetSemesterStatusAsync(entity.SemesterId);
            if (semesterStatus == "Closed")
                throw new InvalidOperationException($"Cannot update project: Semester ended on " +
                    (await _semesterService.GetByIdAsync(entity.SemesterId))?.EndDate.ToString("yyyy-MM-dd") + ".");

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

                if (newStatus == ProjectStatus.InProcess)
                {
                    var semester = await _semesterService.GetByIdAsync(entity.SemesterId);
                    var now = DateTime.UtcNow;
                    if (entity.CurrentMembers < entity.MinMembers)
                        throw new InvalidOperationException($"Need at least {entity.MinMembers} members to set InProcess.");
                    if (semester?.StartDate > now)
                        throw new InvalidOperationException($"Cannot start project yet: Semester starts on {semester.StartDate:yyyy-MM-dd}.");
                }
                entity.Status = newStatus;
            }
            await _unitOfWork.ProjectRepository.UpdateAsync(entity);
            await _unitOfWork.CommitAsync();
            await _semesterService.SyncSemesterStatusAsync(entity.SemesterId);
            return new ProjectDto(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.ProjectRepository.FindByIdAsync(id);
            if (entity == null) return false;

            var semesterStatus = await _semesterService.GetSemesterStatusAsync(entity.SemesterId);
            if (semesterStatus == "Closed")
                throw new InvalidOperationException("Cannot delete project from a closed semester.");

            await _unitOfWork.ProjectRepository.DeleteAsync(entity);
            await _unitOfWork.CommitAsync();
            return true;
        }

        public async Task<PaginatedList<ProjectDto>> GetByMentorIdPagedAsync(
            int mentorId,
            int pageIndex,
            int pageSize,
            string? keyword = null,
            string? status = null,
            string? sortColumn = "Id",
            string? sortDir = "Asc")
        {
            await ValidateMentorAsync(mentorId);
            var paged = await _unitOfWork.ProjectRepository.GetByMentorIdPagedAsync(
                mentorId, pageIndex, pageSize, keyword, status, sortColumn, sortDir);
            var list = paged.Select(x => new ProjectDto(x)).ToList();
            return new PaginatedList<ProjectDto>(
                list,
                paged.TotalItems,
                paged.PageIndex,
                paged.PageSize
            );
        }

        public async Task<PaginatedList<ProjectDto>> GetByMemberIdPagedAsync(
            int memberId,
            int pageIndex,
            int pageSize,
            string? keyword = null,
            string? status = null,
            string? sortColumn = "Id",
            string? sortDir = "Asc")
        {
            var paged = await _unitOfWork.ProjectRepository.GetByMemberIdPagedAsync(
                memberId, pageIndex, pageSize, keyword, status, sortColumn, sortDir);
            var list = paged.Select(x => new ProjectDto(x)).ToList();
            return new PaginatedList<ProjectDto>(
                list,
                paged.TotalItems,
                paged.PageIndex,
                paged.PageSize
            );
        }

        public async Task<bool> StartByMentorAsync(int projectId, int mentorId)
        {
            var entity = await _unitOfWork.ProjectRepository.FindByIdAsync(projectId);
            if (entity == null)
                throw new ArgumentException("Project not found.", nameof(projectId));

            if (entity.MentorId != mentorId)
                throw new InvalidOperationException("You are not the mentor of this project.");

            var semesterStatus = await _semesterService.GetSemesterStatusAsync(entity.SemesterId);
            if (semesterStatus == "Closed")
                throw new InvalidOperationException($"Cannot start project: Semester already ended on " +
                    (await _semesterService.GetByIdAsync(entity.SemesterId))?.EndDate.ToString("yyyy-MM-dd") + ".");
            if (semesterStatus != "Active")
                throw new InvalidOperationException($"Cannot start project: Semester is {semesterStatus}. Must be active.");

            if (entity.Status != ProjectStatus.Open)
                throw new InvalidOperationException($"Project must be Open to start. Current status: {entity.Status}.");

            if (entity.CurrentMembers <= entity.MinMembers)
                throw new InvalidOperationException(
                    $"Need more than {entity.MinMembers} members to start project. Current: {entity.CurrentMembers}.");

            entity.Status = ProjectStatus.InProcess;
            await _unitOfWork.ProjectRepository.UpdateAsync(entity);
            await _unitOfWork.CommitAsync();
            await _semesterService.SyncSemesterStatusAsync(entity.SemesterId);
            return true;
        }

        public async Task<bool> CloseByMentorAsync(int projectId, int mentorId)
        {
            var entity = await _unitOfWork.ProjectRepository.FindByIdAsync(projectId);
            if (entity == null)
                throw new ArgumentException("Project not found.", nameof(projectId));

            if (entity.MentorId != mentorId)
                throw new InvalidOperationException("You are not the mentor of this project.");

            var semesterStatus = await _semesterService.GetSemesterStatusAsync(entity.SemesterId);
            if (semesterStatus == "Closed")
                throw new InvalidOperationException($"Cannot close project: Semester already ended on " +
                    (await _semesterService.GetByIdAsync(entity.SemesterId))?.EndDate.ToString("yyyy-MM-dd") + ".");

            if (entity.Status != ProjectStatus.InProcess)
                throw new InvalidOperationException($"Project must be InProcess to close. Current status: {entity.Status}.");

            entity.Status = ProjectStatus.Close;
            await _unitOfWork.ProjectRepository.UpdateAsync(entity);
            await _unitOfWork.CommitAsync();
            await _semesterService.SyncSemesterStatusAsync(entity.SemesterId);
            return true;
        }

        private async Task ValidateProposerAsync(int proposerId)
        {
            var p = await _unitOfWork.UserRepository.FindByIdAsync(proposerId);
            if (p == null)
                throw new ArgumentException("Proposer not found.", nameof(proposerId));
        }

        private async Task ValidateSemesterAsync(int semesterId)
        {
            var semester = await _unitOfWork.SemesterRepository.FindByIdAsync(semesterId);
            if (semester == null)
                throw new ArgumentException("Semester not found.", nameof(semesterId));
        }

        private async Task ValidateMentorAsync(int mentorId)
        {
            var mentor = await _unitOfWork.UserRepository.FindByIdAsync(mentorId);
            if (mentor == null)
                throw new ArgumentException("Mentor not found.", nameof(mentorId));
        }
    }
}