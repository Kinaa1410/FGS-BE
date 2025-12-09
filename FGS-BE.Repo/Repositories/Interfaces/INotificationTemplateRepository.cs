using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.Entities;

namespace FGS_BE.Repo.Repositories.Interfaces
{
    public interface INotificationTemplateRepository : IGenericRepository<NotificationTemplate>
    {
        Task<PaginatedList<NotificationTemplate>> GetPagedAsync(
            int pageIndex,
            int pageSize,
            bool? isActive = null,
            string? sortColumn = "Id",
            string? sortDir = "Desc",
            CancellationToken cancellationToken = default);

        Task<NotificationTemplate?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    }
}
