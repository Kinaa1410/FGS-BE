using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.DTOs.Projects;
using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Repositories.Interfaces;
using FGS_BE.Service.Interfaces;
using Mapster;

namespace FGS_BE.Service.Implements
{
    public class ProjectService : IProjectService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProjectService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
            var entity = dto.ToEntity();
            entity.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.ProjectRepository.CreateAsync(entity);
            await _unitOfWork.CommitAsync();

            return new ProjectDto(entity);
        }

        public async Task<ProjectDto?> UpdateAsync(int id, UpdateProjectDto dto)
        {
            var entity = await _unitOfWork.ProjectRepository.FindByIdAsync(id);

            if (entity == null) return null;

            entity.Title = dto.Title ?? entity.Title;
            entity.Description = dto.Description ?? entity.Description;
            entity.Status = dto.Status ?? entity.Status;
            entity.TotalPoints = dto.TotalPoints;

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
