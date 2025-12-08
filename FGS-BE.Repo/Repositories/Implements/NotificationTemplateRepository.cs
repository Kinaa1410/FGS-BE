using FGS_BE.Repo.Data;
using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace FGS_BE.Repo.Repositories.Implements
{
    public class NotificationTemplateRepository
        : GenericRepository<NotificationTemplate>, INotificationTemplateRepository
    {
        public NotificationTemplateRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        public async Task<PaginatedList<NotificationTemplate>> GetPagedAsync(
            int pageIndex,
            int pageSize,
            bool? isActive = null,
            string? sortColumn = "Id",
            string? sortDir = "Desc",
            CancellationToken cancellationToken = default)
        {
            var query = Entities.AsNoTracking();

            if (isActive.HasValue)
            {
                query = query.Where(x => x.IsActive == isActive.Value);
            }

            var order = $"{sortColumn} {sortDir}";
            query = query.OrderBy(order);

            return await query.PaginatedListAsync(
                pageIndex,
                pageSize,
                cancellationToken);
        }

        public async Task<NotificationTemplate?> GetByCodeAsync(
            string code,
            CancellationToken cancellationToken = default)
        {
            return await Entities
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
        }
    }
}
