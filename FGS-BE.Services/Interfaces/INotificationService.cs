using FGS_BE.Repo.DTOs.Notifications;
using FGS_BE.Repo.DTOs.Pages;

namespace FGS_BE.Services.Interfaces
{
    public interface INotificationService
    {
        Task<PaginatedList<NotificationDto>> GetPagedByUserAsync(
            int userId,
            int pageIndex,
            int pageSize,
            bool? isRead = null,
            string? sortColumn = "Id",
            string? sortDir = "Desc");

        Task<NotificationDto?> MarkAsReadAsync(int id);

        Task<NotificationDto> SendNotificationAsync(SendNotificationDto dto);
    }
}