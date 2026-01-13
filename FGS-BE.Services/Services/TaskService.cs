using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.DTOs.Tasks;
using FGS_BE.Repo.Repositories.Interfaces;
using FGS_BE.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

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
            try
            {
                var paged = await _unitOfWork.TaskRepository.GetPagedAsync(
                    pageIndex, pageSize, keyword, status, milestoneId, assigneeId, parentTaskId, sortColumn, sortDir);

                return new PaginatedList<TaskDto>(
                    paged.Select(x => new TaskDto(x)).ToList(),
                    paged.TotalItems,
                    paged.PageIndex,
                    paged.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("Không thể lấy danh sách task: " + ex.Message);
            }
        }

        public async Task<TaskDto?> GetByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentException("Id không hợp lệ.");

                var entity = await _unitOfWork.TaskRepository.FindByIdAsync(id);
                return entity == null ? null : new TaskDto(entity);
            }
            catch (Exception ex)
            {
                throw new Exception("Không thể lấy thông tin task: " + ex.Message);
            }
        }

        public async Task<TaskDto> CreateAsync(CreateTaskDto dto)
        {
                if (string.IsNullOrWhiteSpace(dto.Label))
                    throw new ArgumentException("Label is required.");

                if (dto.EstimatedHours < 0)
                    throw new ArgumentException("EstimatedHours must be >= 0.");

                if (dto.Weight <= 0 || dto.Weight > 1)
                    throw new ArgumentException("Weight must be > 0 and <= 1.");

                var totalWeight = await _unitOfWork.TaskRepository.Entities
                    .Where(x => x.MilestoneId == dto.MilestoneId)
                    .SumAsync(x => x.Weight);

                if (totalWeight + dto.Weight > 1)
                    throw new InvalidOperationException("Total task weight cannot exceed 1.");


                if (dto.StartDate != null && dto.DueDate != null && dto.DueDate < dto.StartDate)
                    throw new ArgumentException("DueDate must be greater than or equal to StartDate.");

                if (dto.AssigneeId.HasValue && dto.AssigneeId <= 0)
                    throw new ArgumentException("AssigneeId is invalid.");

                var entity = dto.ToEntity();
                entity.ParentTaskId = null;

                await _unitOfWork.TaskRepository.CreateAsync(entity);
                await _unitOfWork.CommitAsync();

                return new TaskDto(entity);
        }

        public async Task<TaskDto?> UpdateAsync(int id, UpdateTaskDto dto)
        {
                if (id <= 0)
                    throw new ArgumentException("Id không hợp lệ.");

                var entity = await _unitOfWork.TaskRepository.FindByIdAsync(id);
                if (entity == null) return null;

                if (dto.EstimatedHours < 0)
                    throw new ArgumentException("EstimatedHours must be >= 0.");

                if (dto.Weight <= 0 || dto.Weight > 1)
                    throw new ArgumentException("Weight must be > 0 and <= 1.");

                var totalWeight = await _unitOfWork.TaskRepository.Entities
                    .Where(x => x.MilestoneId == dto.MilestoneId)
                    .SumAsync(x => x.Weight);

                if (totalWeight + dto.Weight > 1)
                    throw new InvalidOperationException("Total task weight cannot exceed 1.");


                if (dto.StartDate != null && dto.DueDate != null && dto.DueDate < dto.StartDate)
                    throw new ArgumentException("DueDate must be greater than or equal to StartDate.");

                if (dto.AssigneeId.HasValue && dto.AssigneeId <= 0)
                    throw new ArgumentException("AssigneeId is invalid.");

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
                entity.ParentTaskId = null;
                entity.MilestoneId = dto.MilestoneId;

                await _unitOfWork.TaskRepository.UpdateAsync(entity);
                await _unitOfWork.CommitAsync();

                return new TaskDto(entity);

        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentException("Id không hợp lệ.");

                var entity = await _unitOfWork.TaskRepository.FindByIdAsync(id);
                if (entity == null) return false;

                await _unitOfWork.TaskRepository.DeleteAsync(entity);
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Không thể xóa task: " + ex.Message);
            }
        }
    }
}
