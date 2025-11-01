using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.DTOs.Tasks;
using FGS_BE.Repo.Repositories.Interfaces;
using FGS_BE.Service.Interfaces;

namespace FGS_BE.Service.Implements
{
    public class TaskService : ITaskService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TaskService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginatedList<TaskDto>> GetPagedAsync(
            int pageIndex,
            int pageSize,
            string? keyword = null,
            string? status = null,
            int? milestoneId = null,
            int? assigneeId = null,
            int? parentTaskId = null,
            string? sortColumn = "Id",
            string? sortDir = "Asc")
        {
            var paged = await _unitOfWork.TaskRepository.GetPagedAsync(
                pageIndex, pageSize, keyword, status, milestoneId, assigneeId, parentTaskId, sortColumn, sortDir);

            return new PaginatedList<TaskDto>(
                paged.Select(x => new TaskDto(x)).ToList(),
                paged.TotalItems,
                paged.PageIndex,
                paged.PageSize);
        }

        public async Task<TaskDto?> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.TaskRepository.FindByIdAsync(id);
            return entity == null ? null : new TaskDto(entity);
        }

        public async Task<TaskDto> CreateAsync(CreateTaskDto dto)
        {
            var entity = dto.ToEntity();
            await _unitOfWork.TaskRepository.CreateAsync(entity);
            await _unitOfWork.CommitAsync();
            return new TaskDto(entity);
        }

        public async Task<TaskDto?> UpdateAsync(int id, UpdateTaskDto dto)
        {
            var entity = await _unitOfWork.TaskRepository.FindByIdAsync(id);
            if (entity == null) return null;

            entity.Label = dto.Label;
            entity.Description = dto.Description;
            entity.Priority = dto.Priority;
            entity.Complexity = dto.Complexity;
            entity.EstimatedHours = dto.EstimatedHours;
            entity.Weight = dto.Weight;
            entity.Status = dto.Status;
            entity.StartDate = dto.StartDate;
            entity.DueDate = dto.DueDate;
            entity.AssigneeId = dto.AssigneeId;
            entity.ParentTaskId = dto.ParentTaskId;

            await _unitOfWork.TaskRepository.UpdateAsync(entity);
            await _unitOfWork.CommitAsync();

            return new TaskDto(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.TaskRepository.FindByIdAsync(id);
            if (entity == null) return false;

            await _unitOfWork.TaskRepository.DeleteAsync(entity);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
