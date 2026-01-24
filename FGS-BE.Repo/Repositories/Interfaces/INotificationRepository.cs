using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.Entities;

namespace FGS_BE.Repo.Repositories.Interfaces
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        Task<PaginatedList<Notification>> GetPagedByUserAsync(
            int userId,
            int pageIndex,
            int pageSize,
            bool? isRead = null,
            string? sortColumn = "Id",
            string? sortDir = "Desc",
            CancellationToken cancellationToken = default);
    }
}