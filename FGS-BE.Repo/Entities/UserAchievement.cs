using System.ComponentModel.DataAnnotations.Schema;

namespace FGS_BE.Repo.Entities;

public class UserAchievement
{
    public int Id { get; set; }
    public DateTime UnlockedAt { get; set; }

    [ForeignKey(nameof(UserId))]
    public int UserId { get; set; }
    public virtual User User { get; set; } = default!;

    [ForeignKey(nameof(AchievementId))]
    public int AchievementId { get; set; }
    public virtual Achievement Achievement { get; set; } = default!;
}