using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.DTOs.ProjectMembers;
using FGS_BE.Repo.DTOs.Projects;
using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Enums;
using FGS_BE.Repo.Repositories.Interfaces;
using FGS_BE.Service.Interfaces;
using FGS_BE.Services.Interfaces;
using Mapster;

namespace FGS_BE.Service.Implements
{
    public class ProjectService : IProjectService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProjectMemberService _projectMemberService;
        public ProjectService(IUnitOfWork unitOfWork, IProjectMemberService projectMemberService)
        {
            _unitOfWork = unitOfWork;
            _projectMemberService = projectMemberService;
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
                pageIndex, 
                pageSize, 
                keyword,
                status,
                sortColumn, 
                sortDir);
            var pagedDtos = new PaginatedList<ProjectDto>(
                paged.Select(x => new ProjectDto(x)).ToList(), 
                paged.TotalItems, 
                paged.PageIndex, 
                paged.PageSize);
            return pagedDtos;
        }

        public async Task<ProjectDto?> GetByIdAsync(int id)
        {
            var project = await _unitOfWork.ProjectRepository.FindByIdAsync(id);
            return project == null ? null : new ProjectDto(project);
        }

        public async Task<ProjectDto> CreateAsync(CreateProjectDto dto)
        {
            var entity = dto.ToEntity(); // Now uses enum default
            await _unitOfWork.ProjectRepository.CreateAsync(entity);
            await _unitOfWork.CommitAsync();

            // Auto-join proposer
            var memberDto = new CreateProjectMemberDto
            {
                ProjectId = entity.Id,
                UserId = entity.ProposerId,
                Role = "proposer"
            };
            await _projectMemberService.CreateAsync(memberDto); // Now exists

            // Refresh entity to get updated CurrentMembers
            entity = await _unitOfWork.ProjectRepository.FindByIdAsync(entity.Id);
            return new ProjectDto(entity);
        }

        public async Task<ProjectDto?> UpdateAsync(int id, UpdateProjectDto dto)
        {
            var entity = await _unitOfWork.ProjectRepository.FindByIdAsync(id);
            if (entity == null) return null;

            if (dto.Title != null) entity.Title = dto.Title;
            if (dto.Description != null) entity.Description = dto.Description;
            if (dto.TotalPoints.HasValue) entity.TotalPoints = dto.TotalPoints.Value;
            if (dto.MinMembers.HasValue) entity.MinMembers = dto.MinMembers.Value;
            if (dto.MaxMembers.HasValue) entity.MaxMembers = dto.MaxMembers.Value;

            if (dto.Status != null)
            {
                if (!Enum.TryParse<ProjectStatus>(dto.Status, true, out var parsedStatus))
                    throw new ArgumentException($"Invalid Status: {dto.Status}. Valid: Open, InProcess, Close, Complete.");

                // Check min members for InProcess
                if (parsedStatus == ProjectStatus.InProcess && entity.CurrentMembers < entity.MinMembers)
                    throw new InvalidOperationException($"Need at least {entity.MinMembers} members to set InProcess. Current: {entity.CurrentMembers}");

                entity.Status = parsedStatus; // Enum assignment
            }

            await _unitOfWork.ProjectRepository.UpdateAsync(entity);
            await _unitOfWork.CommitAsync();
            return new ProjectDto(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.ProjectRepository.FindByIdAsync(id);
            if (entity == null) return false;

            await _unitOfWork.ProjectRepository.DeleteAsync(entity);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
