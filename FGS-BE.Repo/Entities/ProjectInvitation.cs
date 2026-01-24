using System.ComponentModel.DataAnnotations.Schema;

namespace FGS_BE.Repo.Entities;

public class ProjectInvitation
{
    public int Id { get; set; }

    [ForeignKey(nameof(ProjectId))]
    public int ProjectId { get; set; }
    public virtual Project Project { get; set; } = default!;

    [ForeignKey(nameof(InviterId))]
    public int InviterId { get; set; }
    public virtual User Inviter { get; set; } = default!;

    [ForeignKey(nameof(InvitedUserId))]
    public int? InvitedUserId { get; set; } // Nullable
    public virtual User? InvitedUser { get; set; }

    public string InviteCode { get; set; } = string.Empty; // Unique code for API
    public string Status { get; set; } = "pending"; // "pending", "accepted", "denied", "expired"
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiryAt { get; set; } // CreatedAt + 15 mins
    public string? Message { get; set; }
}