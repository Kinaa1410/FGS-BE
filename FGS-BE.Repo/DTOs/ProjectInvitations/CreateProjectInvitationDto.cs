using FGS_BE.Repo.Entities;

namespace FGS_BE.Repo.DTOs.ProjectInvitations;

public class CreateProjectInvitationDto
{
    public int ProjectId { get; set; }
    public int InvitedUserId { get; set; }
    public string? Message { get; set; }

    public ProjectInvitation ToEntity(int inviterId)
    {
        return new ProjectInvitation
        {
            ProjectId = ProjectId,
            InviterId = inviterId,
            InvitedUserId = InvitedUserId,
            InviteCode = Guid.NewGuid().ToString("N")[..8].ToUpper(), // Short code
            Status = "pending",
            CreatedAt = DateTime.UtcNow,
            ExpiryAt = DateTime.UtcNow.AddMinutes(15), // 15-min lock
            Message = Message
        };
    }
}