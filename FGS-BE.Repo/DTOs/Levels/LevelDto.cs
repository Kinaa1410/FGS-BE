using System.Text.Json;
using FGS_BE.Repo.Entities;

namespace FGS_BE.Repo.DTOs.Levels;

public class LevelDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? ConditionJson { get; set; }
    public JsonDocument? Condition => !string.IsNullOrEmpty(ConditionJson) ? JsonDocument.Parse(ConditionJson) : null;
    public int PointsReward { get; set; }
    public string? Icon { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsActive { get; set; }

    public LevelDto() { }

    public LevelDto(Level entity)
    {
        Id = entity.Id;
        Name = entity.Name;
        Description = entity.Description;
        ConditionJson = entity.ConditionJson;
        PointsReward = entity.PointsReward;
        Icon = entity.Icon;
        CreatedAt = entity.CreatedAt;
        UpdatedAt = entity.UpdatedAt;
        IsActive = entity.IsActive;
    }
}