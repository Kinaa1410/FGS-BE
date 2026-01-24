namespace FGS_BE.Repo.DTOs.ProjectInvitations;

public class AcceptProjectInvitationDto
{
    public string InviteCode { get; set; } = string.Empty;
    public bool Accept { get; set; } = true; // true=accept, false=deny
}