using System.ComponentModel.DataAnnotations;

namespace FGS_BE.Repo.Entities;

public class Notifications
{
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
    public Users User { get; set; }
}