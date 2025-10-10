namespace FGS_BE.Repo.Entities;
public class PerformanceScore
{
    public int Id { get; set; }
    public decimal Score { get; set; }

    public string? Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int MilestoneId { get; set; }
    public virtual Milestone Milestone { get; set; } = default!;
    public int UserId { get; set; }
    public virtual User User { get; set; } = default!;

    public int ProjectId { get; set; }
    public virtual Project Project { get; set; } = default!;

    public int TaskId { get; set; }
    public virtual Task Task { get; set; } = default!;
}
