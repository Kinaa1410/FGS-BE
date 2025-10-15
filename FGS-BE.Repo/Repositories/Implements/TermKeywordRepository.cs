using FGS_BE.Repo.Data;
using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace FGS_BE.Repo.Repositories.Implements
{
    public class TermKeywordRepository : GenericRepository<TermKeyword>, ITermKeywordRepository
    {
        public TermKeywordRepository(ApplicationDbContext context) : base(context) { }

        public async Task<PaginatedList<TermKeyword>> GetPagedAsync(
            int pageIndex,
            int pageSize,
            string? keyword = null,
            string? sortColumn = "Id",
            string? sortDir = "Asc",
            int? semesterId = null,
            CancellationToken cancellationToken = default)
        {
            var query = Entities.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(x => x.Keyword!.Contains(keyword));

            if (semesterId.HasValue)
                query = query.Where(x => x.SemesterId == semesterId.Value);

            var order = $"{sortColumn} {sortDir}";
            query = query.OrderBy(order);

            return await query.PaginatedListAsync(pageIndex, pageSize, cancellationToken);
        }
    }
}
