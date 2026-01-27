using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.DTOs.ProjectMembers;
using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Enums;
using FGS_BE.Repo.Exceptions; // Recommended: use proper exceptions
using FGS_BE.Repo.Repositories.Interfaces;
using FGS_BE.Service.Interfaces; // Assuming ISemesterService is here or add using
using FGS_BE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace FGS_BE.Service.Implements
{
    public class ProjectMemberService : IProjectMemberService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISemesterService _semesterService; // Add this dependency

        public ProjectMemberService(IUnitOfWork unitOfWork, ISemesterService semesterService)
        {
            _unitOfWork = unitOfWork;
            _semesterService = semesterService;
        }

        public async Task<PaginatedList<ProjectMemberDto>> GetPagedAsync(
            int pageIndex,
            int pageSize,
            string? keyword = null,
            int? projectId = null,
            int? userId = null,
            string? sortColumn = "Id",
            string? sortDir = "Asc")
        {
            var paged = await _unitOfWork.ProjectMemberRepository.GetPagedAsync(
                pageIndex, pageSize, keyword, projectId, userId, sortColumn, sortDir);

            return new PaginatedList<ProjectMemberDto>(
                paged.Select(x => new ProjectMemberDto(x)).ToList(),
                paged.TotalItems,
                paged.PageIndex,
                paged.PageSize);
        }

        public async Task<ProjectMemberDto?> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.ProjectMemberRepository.FindByIdAsync(id);
            return entity == null ? null : new ProjectMemberDto(entity);
        }

        public async Task<ProjectMemberDto> CreateAsync(CreateProjectMemberDto dto)
        {
            // Load project with semester
            var project = await _unitOfWork.ProjectRepository.Entities
                .Include(p => p.Semester)
                .FirstOrDefaultAsync(p => p.Id == dto.ProjectId);

            if (project == null)
                throw new ArgumentException("Project not found!");

            if (project.Semester == null)
                throw new ArgumentException("Project has no associated semester!");

            // CRITICAL: Get semester status
            var semesterStatus = await _semesterService.GetSemesterStatusAsync(project.SemesterId);

            // Block if semester is Closed
            if (semesterStatus == "Closed")
                throw new InvalidOperationException(
                    $"Cannot join project: The semester has already ended on {project.Semester.EndDate:yyyy-MM-dd}.");

            // Block if semester hasn't started yet
            //if (semesterStatus == "Upcoming")
            //    throw new InvalidOperationException(
            //        $"Cannot join project yet: The semester starts on {project.Semester.StartDate:yyyy-MM-dd}.");

            // Extra safety: Only allow joining Open projects
            if (project.Status != ProjectStatus.Open)
                throw new InvalidOperationException(
                    $"Cannot join project: Project is no longer open for joining (current status: {project.Status}).");

            // Role check: Only User or Mentor can join
            var user = await _unitOfWork.UserRepository.Entities
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == dto.UserId);

            if (user == null)
                throw new ArgumentException("User not found!");

            var allowedRoles = new[] { RoleEnums.User.ToString(), RoleEnums.Mentor.ToString() };
            if (!user.UserRoles.Any(ur => allowedRoles.Contains(ur.Role.Name)))
                throw new InvalidOperationException("Only users with 'User' or 'Mentor' role can join projects.");

            // Check project capacity
            if (project.CurrentMembers + project.ReservedMembers >= project.MaxMembers)
                throw new InvalidOperationException(
                    $"Project is full (Max members: {project.MaxMembers}). Cannot join.");

            // Check if already in this project
            bool alreadyInThisProject = await _unitOfWork.ProjectMemberRepository.Entities
                .AnyAsync(pm => pm.UserId == dto.UserId && pm.ProjectId == dto.ProjectId);

            if (alreadyInThisProject)
                throw new InvalidOperationException("You have already joined this project!");

            // One project per user per semester
            bool alreadyJoinedOtherProject = await _unitOfWork.ProjectMemberRepository.Entities
                .Include(pm => pm.Project)
                .AnyAsync(pm => pm.UserId == dto.UserId &&
                               pm.Project.SemesterId == project.SemesterId &&
                               pm.ProjectId != dto.ProjectId);

            if (alreadyJoinedOtherProject)
                throw new InvalidOperationException(
                    "You are already a member of another project in this semester. Leave it first before joining a new one.");

            var entity = dto.ToEntity();
            entity.JoinAt = DateTime.UtcNow;

            await using var transaction = await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted);
            try
            {
                await _unitOfWork.ProjectMemberRepository.CreateAsync(entity);

                project.CurrentMembers += 1;
                await _unitOfWork.ProjectRepository.UpdateAsync(project);

                await _unitOfWork.CommitAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

            return new ProjectMemberDto(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.ProjectMemberRepository.FindByIdAsync(id);
            if (entity == null) return false;

            var project = await _unitOfWork.ProjectRepository.FindByIdAsync(entity.ProjectId);
            if (project != null)
            {
                project.CurrentMembers = Math.Max(0, project.CurrentMembers - 1);
                await _unitOfWork.ProjectRepository.UpdateAsync(project);
            }

            await _unitOfWork.ProjectMemberRepository.DeleteAsync(entity);
            await _unitOfWork.CommitAsync();

            return true;
        }

        public async Task<ProjectMemberDto?> UpdateAsync(int id, UpdateProjectMemberDto dto)
        {
            var entity = await _unitOfWork.ProjectMemberRepository.FindByIdAsync(id);
            if (entity == null) return null;

            entity.Role = dto.Role ?? entity.Role;

            await _unitOfWork.ProjectMemberRepository.UpdateAsync(entity);
            await _unitOfWork.CommitAsync();

            return new ProjectMemberDto(entity);
        }

        public async Task<bool> LeaveAsync(int userId, int projectId)
        {
            var entity = await _unitOfWork.ProjectMemberRepository.Entities
                .FirstOrDefaultAsync(pm => pm.UserId == userId && pm.ProjectId == projectId);

            if (entity == null) return false;

            var project = await _unitOfWork.ProjectRepository.FindByIdAsync(projectId);
            if (project == null) return false;

            using var transaction = await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted);
            try
            {
                project.CurrentMembers = Math.Max(0, project.CurrentMembers - 1);
                await _unitOfWork.ProjectRepository.UpdateAsync(project);

                await _unitOfWork.ProjectMemberRepository.DeleteAsync(entity);

                await _unitOfWork.CommitAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

            return true;
        }
    }
}