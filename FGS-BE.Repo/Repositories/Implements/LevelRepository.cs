using FGS_BE.Repo.Data;
using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Text.Json;

namespace FGS_BE.Repo.Repositories.Implements;

public class LevelRepository : GenericRepository<Level>, ILevelRepository
{
    public LevelRepository(ApplicationDbContext context) : base(context) { }

    public async Task<PaginatedList<Level>> GetPagedAsync(
        int pageIndex,
        int pageSize,
        string? keyword = null,
        string? sortColumn = "Id",
        string? sortDir = "Asc",
        CancellationToken cancellationToken = default)
    {
        var query = Entities.AsNoTracking().Where(l => l.IsActive); // Only active levels

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(l => l.Name!.Contains(keyword) || l.Description!.Contains(keyword));
        }

        var order = $"{sortColumn} {sortDir}";
        query = query.OrderBy(order);

        return await query.PaginatedListAsync(pageIndex, pageSize, cancellationToken);
    }

    public async Task<ICollection<Level>> GetEligibleLevelsAsync(int currentPoints, CancellationToken cancellationToken = default)
    {
        var activeLevels = await Entities
            .Where(l => l.IsActive)
            .ToListAsync(cancellationToken);

        var eligibleLevels = activeLevels
            .Where(l =>
            {
                if (string.IsNullOrEmpty(l.ConditionJson))
                    return false;

                try
                {
                    var condition = JsonDocument.Parse(l.ConditionJson);
                    return condition.RootElement.TryGetProperty("threshold", out var thresholdProp) &&
                           thresholdProp.ValueKind == JsonValueKind.Number &&
                           thresholdProp.GetInt32() <= currentPoints;
                }
                catch (JsonException)
                {
                    // Invalid JSON, skip
                    return false;
                }
            })
            .OrderByDescending(l =>
            {
                if (string.IsNullOrEmpty(l.ConditionJson))
                    return 0;

                try
                {
                    var condition = JsonDocument.Parse(l.ConditionJson);
                    return condition.RootElement.TryGetProperty("threshold", out var thresholdProp) &&
                           thresholdProp.ValueKind == JsonValueKind.Number
                        ? thresholdProp.GetInt32()
                        : 0;
                }
                catch (JsonException)
                {
                    return 0;
                }
            })
            .ToList();

        return eligibleLevels;
    }
}