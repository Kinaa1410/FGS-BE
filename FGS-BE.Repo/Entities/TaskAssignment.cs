using System.ComponentModel.DataAnnotations;

namespace FGS_BE.Repo.Entities;

public class TaskAssignment
{
    [Key]
    public int Id { get; set; }
    public int TaskId { get; set; }
    public int UserId { get; set; }
    public DateTime AssignedAt { get; set; }
    public string Status { get; set; } = string.Empty;

    public Tasks Task { get; set; }
    public Users User { get; set; }
}