using System.ComponentModel.DataAnnotations;
using FGS_BE.Repo.Entities;

namespace FGS_BE.Repo.DTOs.Levels;

public class CreateLevelDto
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Description { get; set; }

    [Required]
    public string ConditionJson { get; set; } = string.Empty;

    public int PointsReward { get; set; }

    [MaxLength(255)]
    public string? Icon { get; set; }

    public bool IsActive { get; set; } = true;

    public Level ToEntity()
    {
        return new Level
        {
            Name = this.Name,
            Description = this.Description,
            ConditionJson = this.ConditionJson,
            PointsReward = this.PointsReward,
            Icon = this.Icon,
            IsActive = this.IsActive,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
}