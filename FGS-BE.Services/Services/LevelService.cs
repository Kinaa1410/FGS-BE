using FGS_BE.Repo.DTOs.Levels;
using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Repositories.Interfaces;
using FGS_BE.Service.Interfaces;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FGS_BE.Service.Implements;

public class LevelService : ILevelService
{
    private readonly IUnitOfWork _unitOfWork;

    public LevelService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginatedList<LevelDto>> GetPagedAsync(int pageIndex, int pageSize, string? keyword = null, string? sortColumn = "Id", string? sortDir = "Asc")
    {
        var levelRepo = _unitOfWork.Repository<Level>();
        var query = levelRepo.Entities.AsNoTracking().Where(l => l.IsActive);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(l => l.Name!.Contains(keyword) || l.Description!.Contains(keyword));
        }

        // Basic sorting (use OrderBy for common columns; extend for dynamic if needed)
        query = sortColumn?.ToLower() switch
        {
            "name" => sortDir == "desc" ? query.OrderByDescending(l => l.Name) : query.OrderBy(l => l.Name),
            "pointsreward" => sortDir == "desc" ? query.OrderByDescending(l => l.PointsReward) : query.OrderBy(l => l.PointsReward),
            "createdat" => sortDir == "desc" ? query.OrderByDescending(l => l.CreatedAt) : query.OrderBy(l => l.CreatedAt),
            _ => sortDir == "desc" ? query.OrderByDescending(l => l.Id) : query.OrderBy(l => l.Id)
        };

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedList<LevelDto>(
            items.Select(x => x.Adapt<LevelDto>()).ToList(),
            totalCount,
            pageIndex,
            pageSize);
    }

    public async Task<LevelDto?> GetByIdAsync(int id)
    {
        var levelRepo = _unitOfWork.Repository<Level>();
        var entity = await levelRepo.FindByIdAsync(id);
        return entity == null ? null : entity.Adapt<LevelDto>();
    }

    public async Task<LevelDto> CreateAsync(CreateLevelDto dto)
    {
        var levelRepo = _unitOfWork.Repository<Level>();
        var entity = dto.ToEntity();
        await levelRepo.CreateAsync(entity);
        await _unitOfWork.CommitAsync();
        return entity.Adapt<LevelDto>();
    }

    public async Task<LevelDto?> UpdateAsync(int id, UpdateLevelDto dto)
    {
        var levelRepo = _unitOfWork.Repository<Level>();
        var entity = await levelRepo.FindByIdAsync(id);
        if (entity == null) return null;
        dto.Adapt(entity); // Partial update
        entity.UpdatedAt = DateTime.UtcNow;
        await levelRepo.UpdateAsync(entity);
        await _unitOfWork.CommitAsync();
        return entity.Adapt<LevelDto>();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var levelRepo = _unitOfWork.Repository<Level>();
        var entity = await levelRepo.FindByIdAsync(id);
        if (entity == null) return false;
        await levelRepo.DeleteAsync(entity);
        await _unitOfWork.CommitAsync();
        return true;
    }

    public async Task<UserLevelDto?> CheckAndAssignUserLevelAsync(int userId, CheckLevelUpRequest request)
    {
        var levelRepo = _unitOfWork.Repository<Level>();
        var userLevelRepo = _unitOfWork.Repository<UserLevel>();
        var userWalletRepo = _unitOfWork.Repository<UserWallet>();

        // Fetch existing user levels
        var existingLevels = await userLevelRepo.Entities
            .Where(ul => ul.UserId == userId)
            .Select(ul => ul.LevelId)
            .ToListAsync(); // Simple ID list for check

        // Get eligible levels (use client-side for JSON parse)
        var allActiveLevels = await levelRepo.Entities
            .Where(l => l.IsActive)
            .ToListAsync();

        var eligibleLevels = allActiveLevels
            .Where(l =>
            {
                if (string.IsNullOrEmpty(l.ConditionJson)) return false;
                try
                {
                    var condition = JsonDocument.Parse(l.ConditionJson);
                    return condition.RootElement.TryGetProperty("threshold", out var thresholdProp) &&
                           thresholdProp.ValueKind == JsonValueKind.Number &&
                           thresholdProp.GetInt32() <= request.CurrentPoints;
                }
                catch (JsonException)
                {
                    return false;
                }
            })
            .OrderByDescending(l =>
            {
                if (string.IsNullOrEmpty(l.ConditionJson)) return 0;
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

        // Find next level not yet assigned
        var nextLevel = eligibleLevels.FirstOrDefault(l => !existingLevels.Contains(l.Id));
        if (nextLevel == null) return null;

        // Assign UserLevel
        var userLevel = new UserLevel
        {
            UserId = userId,
            LevelId = nextLevel.Id,
            UnlockedAt = DateTime.UtcNow
        };
        await userLevelRepo.CreateAsync(userLevel);
        await _unitOfWork.CommitAsync();
        return new UserLevelDto(userLevel);
    }
}