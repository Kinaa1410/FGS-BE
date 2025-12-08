using FGS_BE.Repo.DTOs.Notifications;
using FGS_BE.Repo.DTOs.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FGS_BE.Service.Interfaces
{
    public interface INotificationTemplateService
    {
        Task<PaginatedList<NotificationTemplateDto>> GetPagedAsync(int pageIndex, int pageSize, string? keyword = null, string? sortColumn = "Id", string? sortDir = "Asc");
        Task<NotificationTemplateDto?> GetByIdAsync(int id);
        Task<NotificationTemplateDto> CreateAsync(CreateNotificationTemplateDto dto);
        Task<NotificationTemplateDto?> UpdateAsync(int id, UpdateNotificationTemplateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}