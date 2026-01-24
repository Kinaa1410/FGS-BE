using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FGS_BE.Repo.Entities;

public class Notification
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(UserId))]
    public int UserId { get; set; }

    [ForeignKey(nameof(NotificationTemplateId))]
    public int NotificationTemplateId { get; set; }

    [MaxLength(200)]
    public string Subject { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool SentByEmail { get; set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;

    public virtual EmailQueue EmailQueue { get; set; } = default!;
    public virtual NotificationTemplate NotificationTemplate { get; set; } = null!;
}