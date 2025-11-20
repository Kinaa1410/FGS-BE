using FGS_BE.Repo.Data;
using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace FGS_BE.Repo.Repositories.Implements
{
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<PaginatedList<Notification>> GetPagedByUserAsync(
            int userId,
            int pageIndex,
            int pageSize,
            bool? isRead = null,
            string? sortColumn = "Id",
            string? sortDir = "Desc",
            CancellationToken cancellationToken = default)
        {
            var query = Entities
                .AsNoTracking()
                .Where(x => x.UserId == userId);

            if (isRead.HasValue)
            {
                query = query.Where(x => x.IsRead == isRead.Value);
            }

            var order = $"{sortColumn} {sortDir}";
            query = query.OrderBy(order);

            return await query.PaginatedListAsync(pageIndex, pageSize, cancellationToken);
        }
    }
}