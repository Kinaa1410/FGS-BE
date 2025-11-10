using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.DTOs.ProjectMembers;
using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Repositories.Interfaces;
using FGS_BE.Services.Interfaces;

namespace FGS_BE.Services.Implements
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
            var exists = await _unitOfWork.ProjectMemberRepository.ExistsByAsync(
                x => x.UserId == dto.UserId);

            if (exists)
            {
                throw new InvalidOperationException("User has already joined a project and cannot be added again.");
            }

            var entity = new ProjectMember
            {
                ProjectId = dto.ProjectId,
                UserId = dto.UserId,
                Role = dto.Role ?? "Member",
                JoinAt = DateTime.UtcNow
            };

            await _unitOfWork.ProjectMemberRepository.CreateAsync(entity);
            await _unitOfWork.CommitAsync();

            return new ProjectMemberDto(entity);
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

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.ProjectMemberRepository.FindByIdAsync(id);
            if (entity == null) return false;

            await _unitOfWork.ProjectMemberRepository.DeleteAsync(entity);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
