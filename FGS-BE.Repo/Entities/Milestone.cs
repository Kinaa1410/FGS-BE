namespace FGS_BE.Repo.Entities;

public class Milestone
{
    public int Id { get; set; }

    public string? Title { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime DueDate { get; set; }
    public decimal Weight { get; set; }
    public string? Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public int ProjectId { get; set; }
    public virtual Project Project { get; set; } = default!;
    public virtual ICollection<PerformanceScore> PerformanceScores { get; set; } = new HashSet<PerformanceScore>();
    public virtual ICollection<Task> Tasks { get; set; } = new HashSet<Task>();

}