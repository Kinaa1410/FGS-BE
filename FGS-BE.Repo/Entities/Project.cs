namespace FGS_BE.Repo.Entities;

public class Project
{

    public int Id { get; set; }

    public string? Title { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public string? Status { get; set; } = string.Empty;

    public decimal TotalPoints { get; set; }

    public DateTime CreatedAt { get; set; }

    public int SemesterId { get; set; }
    public virtual Semester Semester { get; set; } = default!;

    public virtual ChatRoom ChatRoom { get; set; } = default!;

    public virtual ICollection<ProjectKeyword> ProjectKeywords { get; set; } = new HashSet<ProjectKeyword>();
    public virtual ICollection<Milestone> Milestones { get; set; } = new HashSet<Milestone>();
    public virtual ICollection<PerformanceScore> PerformanceScores { get; set; } = new HashSet<PerformanceScore>();
    public virtual ICollection<ProjectMember> ProjectMembers { get; set; } = new HashSet<ProjectMember>();

}