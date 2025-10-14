namespace FGS_BE.Repo.DTOs.Achievements;
public sealed record AchievementResponse
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? ConditionType { get; set; }
    public bool IsActive { get; set; }
    public string? IconUrl { get; set; }
    public int PointsReward { get; set; }
    public string? ConditionValue { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
