using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.DTOs.ProjectMembers;
using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Repositories.Interfaces;
using FGS_BE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace FGS_BE.Service.Implements
{
    public class ProjectMemberService : IProjectMemberService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProjectMemberService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
            var project = await _unitOfWork.ProjectRepository.FindByAsync(p => p.Id == dto.ProjectId, q => q.Include(x => x.Semester));
            if (project == null) throw new Exception("Project not found!");

            // UPDATED: Check total occupied (current + reserved)
            if (project.CurrentMembers + project.ReservedMembers >= project.MaxMembers)
                throw new Exception($"Project is full or locked (Max: {project.MaxMembers}). Cannot join.");

            // Existing checks
            bool alreadyInThisProject = await _unitOfWork.ProjectMemberRepository.Entities
                .AnyAsync(pm => pm.UserId == dto.UserId && pm.ProjectId == dto.ProjectId);
            if (alreadyInThisProject) throw new Exception("You have already joined this project!");

            bool alreadyJoinedOtherProject = await _unitOfWork.ProjectMemberRepository.Entities
                .Include(pm => pm.Project)
                .AnyAsync(pm => pm.UserId == dto.UserId && pm.Project.SemesterId == project.SemesterId && pm.ProjectId != dto.ProjectId);
            if (alreadyJoinedOtherProject) throw new Exception("You have taken on another project in the same semester!");

            var entity = dto.ToEntity();
            entity.JoinAt = DateTime.UtcNow;

            using var transaction = await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted);
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
            await transaction.DisposeAsync();

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

        
    }
}
