using System.ComponentModel.DataAnnotations;

namespace FGS_BE.Repo.Entities;

public class Tasks
{
    [Key]
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsAssigned { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime AcceptedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public Projects Project { get; set; }
    public Users User { get; set; }
}