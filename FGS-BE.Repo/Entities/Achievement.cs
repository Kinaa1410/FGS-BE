namespace FGS_BE.Repo.Entities;
public class Achievement
{
    public int Id { get; set; }
    public string? Name { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public string? ConditionType { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string? IconUrl { get; set; } = string.Empty;
    public int PointsReward { get; set; }
    public string? ConditionValue { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<UserAchievement> UserAchievements { get; set; } = new HashSet<UserAchievement>();

}
