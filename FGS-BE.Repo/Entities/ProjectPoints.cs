using System.ComponentModel.DataAnnotations;

namespace FGS_BE.Repo.Entities;

public class ProjectPoints
{
    [Key]
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public int MilestoneId { get; set; }
    public int GrantedBy { get; set; }
    public decimal Points { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public Projects Project { get; set; }
    public Milestones Milestone { get; set; }
    public Users GrantedByUser { get; set; }
}