using FGS_BE.Repo.Data;
using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Enums; // Add if needed for any enum filters
using FGS_BE.Repo.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FGS_BE.Repo.Repositories.Implements
{
    public class SemesterRepository : GenericRepository<Semester>, ISemesterRepository
    {
        public SemesterRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<PaginatedList<Semester>> GetPagedAsync(
            int pageIndex,
            int pageSize,
            string? keyword = null,
            string? status = null,
            string? sortColumn = "Id",
            string? sortDir = "Asc",
            CancellationToken cancellationToken = default)
        {
            var query = Entities.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(x => x.Name!.Contains(keyword) || x.KeywordTheme!.Contains(keyword));
            }
            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(x => x.Status == status);
            }
            var order = $"{sortColumn} {sortDir}";
            query = query.OrderBy(order);
            return await query.PaginatedListAsync(pageIndex, pageSize, cancellationToken);
        }

        // NEW: Implementation for GetByAsync (custom query with predicate and optional include)
        public async Task<List<Semester>> GetByAsync(
            Expression<Func<Semester, bool>> predicate,
            Func<IQueryable<Semester>, IQueryable<Semester>>? include = null,
            CancellationToken cancellationToken = default)
        {
            var query = Entities.AsQueryable();
            if (include != null)
            {
                query = include(query);
            }
            return await query.Where(predicate).ToListAsync(cancellationToken);
        }
    }
}