using System.ComponentModel.DataAnnotations;

namespace FGS_BE.Repo.DTOs.Levels;

public class UpdateLevelDto
{
    [MaxLength(100)]
    public string? Name { get; set; }

    [MaxLength(100)]
    public string? Description { get; set; }

    public string? ConditionJson { get; set; }

    public int? PointsReward { get; set; }

    [MaxLength(255)]
    public string? Icon { get; set; }

    public bool? IsActive { get; set; }
}