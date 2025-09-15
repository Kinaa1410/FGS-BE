using System.ComponentModel.DataAnnotations;

namespace FGS_BE.Repo.Entities;

public class Milestones
{
    [Key]
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public decimal Points { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public Projects Project { get; set; }
}