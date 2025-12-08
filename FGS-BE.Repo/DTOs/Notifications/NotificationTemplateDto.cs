using System.ComponentModel.DataAnnotations;
using FGS_BE.Repo.Entities;

namespace FGS_BE.Repo.DTOs.Notifications
{
    public class NotificationTemplateDto
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? SubjectTemplate { get; set; }
        public string? BodyTemplate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public NotificationTemplateDto() { }

        public NotificationTemplateDto(NotificationTemplate entity)
        {
            Id = entity.Id;
            Code = entity.Code;
            SubjectTemplate = entity.SubjectTemplate;
            BodyTemplate = entity.BodyTemplate;
            IsActive = entity.IsActive;
        }
    }

    public class CreateNotificationTemplateDto
    {
        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;
        [Required]
        [MaxLength(500)]
        public string SubjectTemplate { get; set; } = string.Empty;
        [Required]
        [MaxLength(5000)]
        public string BodyTemplate { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }

    public class UpdateNotificationTemplateDto
    {
        [MaxLength(50)]
        public string? Code { get; set; }
        [MaxLength(500)]
        public string? SubjectTemplate { get; set; }
        [MaxLength(5000)]
        public string? BodyTemplate { get; set; }
        public bool? IsActive { get; set; }
    }
}