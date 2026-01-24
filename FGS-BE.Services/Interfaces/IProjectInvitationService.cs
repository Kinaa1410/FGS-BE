using FGS_BE.Repo.DTOs.ProjectInvitations;
using FGS_BE.Repo.DTOs.Pages;

namespace FGS_BE.Service.Interfaces;

public interface IProjectInvitationService
{
    Task<ProjectInvitationDto> CreateAsync(CreateProjectInvitationDto dto, int inviterId);
    Task<ProjectInvitationDto?> AcceptOrDenyAsync(AcceptProjectInvitationDto dto, int userId);
    Task<bool> CancelAsync(int id, int userId);
    Task<PaginatedList<ProjectInvitationDto>> GetForProjectAsync(int projectId, int pageIndex = 1, int pageSize = 10);
    Task<ProjectInvitationDto?> GetByIdAsync(int id);
}