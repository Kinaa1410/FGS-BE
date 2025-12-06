using FGS_BE.Repo.Entities;

namespace FGS_BE.Repo.DTOs.ProjectInvitations;

public class ProjectInvitationDto
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public string InviteCode { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiryAt { get; set; }
    public string? Message { get; set; }
    public int InviterId { get; set; }
    public int InvitedUserId { get; set; }

    public ProjectInvitationDto() { }

    public ProjectInvitationDto(ProjectInvitation entity)
    {
        Id = entity.Id;
        ProjectId = entity.ProjectId;
        InviteCode = entity.InviteCode;
        Status = entity.Status;
        CreatedAt = entity.CreatedAt;
        ExpiryAt = entity.ExpiryAt;
        Message = entity.Message;
        InviterId = entity.InviterId;
        InvitedUserId = entity.InvitedUserId ?? 0; 
    }
}