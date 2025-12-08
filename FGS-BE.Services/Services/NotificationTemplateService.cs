using FGS_BE.Repo.DTOs.Notifications;
using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Repositories.Interfaces;
using FGS_BE.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace FGS_BE.Service.Implements
{
    public class NotificationTemplateService : INotificationTemplateService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<NotificationTemplate> _repository;

        public NotificationTemplateService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = _unitOfWork.Repository<NotificationTemplate>();
        }

        public async Task<PaginatedList<NotificationTemplateDto>> GetPagedAsync(int pageIndex, int pageSize, string? keyword = null, string? sortColumn = "Id", string? sortDir = "Asc")
        {
            var query = _repository.Entities.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(t => t.Code!.Contains(keyword) || t.SubjectTemplate!.Contains(keyword));
            }
            // Safe sorting
            var validSortColumn = new[] { "Id", "Code", "SubjectTemplate", "CreatedAt" }.Contains(sortColumn?.ToLower()) ? sortColumn : "Id";
            var validSortDir = new[] { "Asc", "Desc" }.Contains(sortDir?.ToUpper()) ? sortDir : "Asc";
            var order = $"{validSortColumn} {validSortDir}";
            query = query.OrderBy(order);
            var pagedEntities = await query.PaginatedListAsync(pageIndex, pageSize);
            var dtos = pagedEntities.Select(x => new NotificationTemplateDto(x)).ToList();
            return new PaginatedList<NotificationTemplateDto>(dtos, pagedEntities.TotalItems, pagedEntities.PageIndex, pagedEntities.PageSize);
        }

        public async Task<NotificationTemplateDto?> GetByIdAsync(int id)
        {
            var entity = await _repository.FindByIdAsync(id);
            return entity == null ? null : new NotificationTemplateDto(entity);
        }

        public async Task<NotificationTemplateDto> CreateAsync(CreateNotificationTemplateDto dto)
        {
            // Check duplicate Code
            if (await _repository.Entities.AnyAsync(t => t.Code == dto.Code))
            {
                throw new InvalidOperationException($"Template with code '{dto.Code}' already exists.");
            }
            var entity = new NotificationTemplate
            {
                Code = dto.Code,
                SubjectTemplate = dto.SubjectTemplate,
                BodyTemplate = dto.BodyTemplate,
                IsActive = dto.IsActive,
            };
            await _repository.CreateAsync(entity);
            await _unitOfWork.CommitAsync();
            return new NotificationTemplateDto(entity);
        }

        public async Task<NotificationTemplateDto?> UpdateAsync(int id, UpdateNotificationTemplateDto dto)
        {
            var entity = await _repository.FindByIdAsync(id);
            if (entity == null) return null;

            // Check duplicate Code if changing
            if (dto.Code != null && dto.Code != entity.Code && await _repository.Entities.AnyAsync(t => t.Code == dto.Code))
            {
                throw new InvalidOperationException($"Template with code '{dto.Code}' already exists.");
            }

            if (dto.Code != null) entity.Code = dto.Code;
            if (dto.SubjectTemplate != null) entity.SubjectTemplate = dto.SubjectTemplate;
            if (dto.BodyTemplate != null) entity.BodyTemplate = dto.BodyTemplate;
            if (dto.IsActive.HasValue) entity.IsActive = dto.IsActive.Value;

            await _repository.UpdateAsync(entity);
            await _unitOfWork.CommitAsync();
            return new NotificationTemplateDto(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _repository.FindByIdAsync(id);
            if (entity == null) return false;
            await _repository.DeleteAsync(entity);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}