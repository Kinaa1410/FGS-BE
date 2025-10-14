using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FGS_BE.Repo.Entities;

public class Notification
{
    public int Id { get; set; }

    public string? Subject { get; set; } = string.Empty;
    public string? Message { get; set; } = string.Empty;
    public int RelatedId { get; set; }
    public string? RelatedType { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool SentByEmail { get; set; }

    [ForeignKey(nameof(UserId))]
    public int UserId { get; set; }
    public virtual User User { get; set; } = default!;

    public virtual EmailQueue EmailQueue { get; set; } = default!;

    [ForeignKey(nameof(NotificationTemplateId))]
    public int NotificationTemplateId { get; set; }
    public virtual NotificationTemplate NotificationTemplate { get; set; } = default!;
}