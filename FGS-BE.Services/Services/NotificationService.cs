using FGS_BE.Repo.DTOs.Notifications;
using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Repositories.Interfaces;
using FGS_BE.Service.Interfaces;
using FGS_BE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace FGS_BE.Service.Implements
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Notification> _notificationRepository;
        private readonly IGenericRepository<NotificationTemplate> _templateRepository;

        public NotificationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _notificationRepository = _unitOfWork.Repository<Notification>();
            _templateRepository = _unitOfWork.Repository<NotificationTemplate>();
        }

        public async Task<PaginatedList<NotificationDto>> GetPagedByUserAsync(
            int userId,
            int pageIndex,
            int pageSize,
            bool? isRead = null,
            string? sortColumn = "CreatedAt",
            string? sortDir = "Desc")
        {
            // Assume userId >0 validated in controller
            var query = _notificationRepository.Entities.AsNoTracking()
                .Where(x => x.UserId == userId);
            if (isRead.HasValue)
            {
                query = query.Where(x => x.IsRead == isRead.Value);
            }
            // Safe sort: Default if invalid
            var validSortColumn = new[] { "Id", "Subject", "CreatedAt", "IsRead" }.Contains(sortColumn?.ToLower()) ? sortColumn : "CreatedAt";
            var validSortDir = new[] { "Asc", "Desc" }.Contains(sortDir?.ToUpper()) ? sortDir : "Desc";
            var order = $"{validSortColumn} {validSortDir}";
            query = query.OrderBy(order);
            var pagedEntities = await query.PaginatedListAsync(pageIndex, pageSize);
            var dtos = pagedEntities.Select(x => new NotificationDto(x)).ToList();
            return new PaginatedList<NotificationDto>(
                dtos,
                pagedEntities.TotalItems,
                pagedEntities.PageIndex,
                pagedEntities.PageSize);
        }

        public async Task<NotificationDto?> MarkAsReadAsync(int id)
        {
            // Assume id >0 validated in controller
            var entity = await _notificationRepository.FindByIdAsync(id);
            if (entity == null) return null;
            entity.IsRead = true;
            await _notificationRepository.UpdateAsync(entity);
            await _unitOfWork.CommitAsync();
            return new NotificationDto(entity);
        }

        public async Task<NotificationDto?> SendNotificationAsync(SendNotificationDto dto)
        {
            var template = await _templateRepository.FindByAsync<NotificationTemplate>(
                t => t.Code == dto.TemplateCode && t.IsActive);
            if (template == null)
            {
                return null; // Controller will handle as 404
            }
            var subject = ReplacePlaceholders(template.SubjectTemplate ?? string.Empty, dto.Placeholders);
            var message = ReplacePlaceholders(template.BodyTemplate ?? string.Empty, dto.Placeholders);
            var notification = new Notification
            {
                UserId = dto.UserId,
                NotificationTemplateId = template.Id,
                Subject = subject,
                Message = message,
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                SentByEmail = dto.SendByEmail
            };
            await _notificationRepository.CreateAsync(notification);
            await _unitOfWork.CommitAsync();
            return new NotificationDto(notification);
        }

        private string ReplacePlaceholders(string template, Dictionary<string, object>? placeholders)
        {
            if (string.IsNullOrEmpty(template) || placeholders == null) return template ?? string.Empty;
            var result = template;
            foreach (var kvp in placeholders)
            {
                result = result.Replace($"{{{kvp.Key}}}", kvp.Value?.ToString() ?? string.Empty);
            }
            return result;
        }
    }
}