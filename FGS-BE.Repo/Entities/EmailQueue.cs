using System.ComponentModel.DataAnnotations.Schema;

namespace FGS_BE.Repo.Entities;

public class EmailQueue
{
    public int Id { get; set; }

    public string? Subject { get; set; } = string.Empty;
    public string? Body { get; set; } = string.Empty;
    public string? RecipientEmail { get; set; } = string.Empty;
    public DateTime ScheduledAt { get; set; }
    public bool IsSent { get; set; }
    public DateTime? SentAt { get; set; }

    [ForeignKey(nameof(NotificationId))]
    public int NotificationId { get; set; }
    public virtual Notification Notification { get; set; } = default!;
}