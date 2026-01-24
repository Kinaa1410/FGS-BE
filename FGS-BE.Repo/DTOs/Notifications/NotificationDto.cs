namespace FGS_BE.Repo.DTOs.Notifications
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int NotificationTemplateId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool SentByEmail { get; set; }

        public NotificationDto() { }

        public NotificationDto(FGS_BE.Repo.Entities.Notification entity)
        {
            Id = entity.Id;
            UserId = entity.UserId;
            NotificationTemplateId = entity.NotificationTemplateId;
            Subject = entity.Subject ?? string.Empty;
            Message = entity.Message ?? string.Empty;
            IsRead = entity.IsRead;
            CreatedAt = entity.CreatedAt;
            SentByEmail = entity.SentByEmail;
        }
    }
}