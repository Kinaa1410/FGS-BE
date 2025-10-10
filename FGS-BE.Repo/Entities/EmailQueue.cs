namespace FGS_BE.Repo.Entities;
public class EmailQueue
{
    public int Id { get; set; }

    public string? ToEmail { get; set; } = string.Empty;

    public string? Subject { get; set; } = string.Empty;
    public string? Body { get; set; } = string.Empty;
    public string? Status { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public DateTime CreatedAt { get; set; }

    public int NotificationId { get; set; }
    public virtual Notification Notification { get; set; } = default!;

}
