namespace FGS_BE.Repo.Entities;

public class NotificationTemplate
{
    public int Id { get; set; }

    public string? Code { get; set; } = string.Empty;
    public string? SubjectTemplate { get; set; } = string.Empty;
    public string? BodyTemplate { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public virtual ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();
}