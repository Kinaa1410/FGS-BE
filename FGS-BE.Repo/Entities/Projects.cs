using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace FGS_BE.Repo.Entities;

public class Projects
{
    [Key]
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Points { get; set; }
    public int LeaderId { get; set; }
    public int GroupId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ICollection<Milestones> Milestones { get; set; } = new List<Milestones>();
    public ICollection<Submissions> Submissions { get; set; } = new List<Submissions>();
    public ICollection<ProjectPoints> ProjectPoints { get; set; } = new List<ProjectPoints>();
    public ICollection<Tasks> Tasks { get; set; } = new List<Tasks>();
    public Users Leader { get; set; }
    public Groups Group { get; set; }
}