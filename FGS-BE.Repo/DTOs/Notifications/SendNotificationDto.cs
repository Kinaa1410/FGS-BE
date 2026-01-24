using System.ComponentModel.DataAnnotations;

namespace FGS_BE.Repo.DTOs.Notifications
{
    public class SendNotificationDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string TemplateCode { get; set; } = string.Empty;

        public Dictionary<string, object>? Placeholders { get; set; } = null; // e.g., { "RequestTitle": "Milestone X", "Status": "Approved" }

        public bool SendByEmail { get; set; } = false;
    }
}