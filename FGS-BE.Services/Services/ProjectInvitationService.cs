using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.DTOs.ProjectInvitations;
using FGS_BE.Repo.DTOs.ProjectMembers;
using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Repositories.Interfaces;
using FGS_BE.Service.Interfaces;
using FGS_BE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace FGS_BE.Service.Implements;

public class ProjectInvitationService : IProjectInvitationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProjectMemberService _projectMemberService;

    public ProjectInvitationService(IUnitOfWork unitOfWork, IProjectMemberService projectMemberService)
    {
        _unitOfWork = unitOfWork;
        _projectMemberService = projectMemberService;
    }

    public async Task<ProjectInvitationDto> CreateAsync(CreateProjectInvitationDto dto, int inviterId)
    {
        // Prevent self-invite
        if (inviterId == dto.InvitedUserId)
            throw new Exception("Cannot invite yourself.");

        var project = await _unitOfWork.ProjectRepository.FindByIdAsync(dto.ProjectId);
        if (project == null) throw new Exception("Project not found.");

        var isInviterMember = await _unitOfWork.ProjectMemberRepository.Entities.AnyAsync(pm => pm.ProjectId == dto.ProjectId && pm.UserId == inviterId);
        if (!isInviterMember)
        {
            throw new Exception("Only members can invite other members to the project.");
        }

        // Check available slots (current + reserved < max)
        if (project.CurrentMembers + project.ReservedMembers >= project.MaxMembers)
            throw new Exception("No available slots (full or locked).");

        // Check not already invited/joined
        if (await _unitOfWork.ProjectInvitationRepository.Entities.AnyAsync(
            pi => pi.ProjectId == dto.ProjectId &&
                  pi.InvitedUserId == dto.InvitedUserId &&
                  pi.Status == "pending"))
            throw new Exception("Already invited.");

        if (await _unitOfWork.ProjectMemberRepository.Entities.AnyAsync(
            pm => pm.ProjectId == dto.ProjectId &&
                  pm.UserId == dto.InvitedUserId))
            throw new Exception("User already joined.");

        // Chặn invite nếu người được mời đang ở project khác cùng semester
        var targetProject = await _unitOfWork.ProjectRepository.FindByAsync(p => p.Id == dto.ProjectId, q => q.Include(x => x.Semester));  // Lấy SemesterId
        if (targetProject != null)
        {
            bool alreadyInSameSemesterProject = await _unitOfWork.ProjectMemberRepository.Entities
                .Include(pm => pm.Project)
                .AnyAsync(pm => pm.UserId == dto.InvitedUserId &&
                               pm.Project.SemesterId == targetProject.SemesterId &&  
                               pm.ProjectId != dto.ProjectId); 
            if (alreadyInSameSemesterProject)
            {
                throw new Exception("The invited has already joined another project in current semester.");
            }
        }

        var entity = dto.ToEntity(inviterId);

        using var transaction = await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        try
        {
            await _unitOfWork.ProjectInvitationRepository.CreateAsync(entity);
            project.ReservedMembers += 1;
            await _unitOfWork.ProjectRepository.UpdateAsync(project);
            await _unitOfWork.CommitAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
        await transaction.DisposeAsync();

        return new ProjectInvitationDto(entity);
    }

    public async Task<ProjectInvitationDto?> AcceptOrDenyAsync(AcceptProjectInvitationDto dto, int userId)
    {
        var invitation = await _unitOfWork.ProjectInvitationRepository.Entities
            .FirstOrDefaultAsync(pi => pi.InviteCode == dto.InviteCode && pi.InvitedUserId == userId && pi.Status == "pending" && pi.ExpiryAt > DateTime.UtcNow);
        if (invitation == null) throw new Exception("Invalid or expired invite.");

        var project = await _unitOfWork.ProjectRepository.FindByIdAsync(invitation.ProjectId);
        if (project == null) throw new Exception("Project not found.");

        using var transaction = await _unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        try
        {
            if (dto.Accept)
            {
                // Accept: Join, unlock reserved, increment current
                project.ReservedMembers -= 1;
                project.CurrentMembers += 1;
                await _unitOfWork.ProjectRepository.UpdateAsync(project);

                invitation.Status = "accepted";
                await _unitOfWork.ProjectInvitationRepository.UpdateAsync(invitation); // Specific repo

                // Auto-join (reuses existing logic for semester checks)
                var memberDto = new CreateProjectMemberDto { ProjectId = invitation.ProjectId, UserId = userId, Role = "member" };
                await _projectMemberService.CreateAsync(memberDto);
            }
            else
            {
                // Deny: Unlock reserved
                project.ReservedMembers -= 1;
                await _unitOfWork.ProjectRepository.UpdateAsync(project);

                invitation.Status = "denied";
                await _unitOfWork.ProjectInvitationRepository.UpdateAsync(invitation);
            }

            await _unitOfWork.CommitAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
        await transaction.DisposeAsync();

        return new ProjectInvitationDto(invitation);
    }

    public async Task<bool> CancelAsync(int id, int userId)
    {
        var invitation = await _unitOfWork.ProjectInvitationRepository.FindByIdAsync(id); // Specific repo
        if (invitation == null || invitation.Status != "pending" || invitation.InviterId != userId)
            return false;

        var project = await _unitOfWork.ProjectRepository.FindByIdAsync(invitation.ProjectId);
        if (project != null)
        {
            project.ReservedMembers = Math.Max(0, project.ReservedMembers - 1);
            await _unitOfWork.ProjectRepository.UpdateAsync(project);
        }

        invitation.Status = "cancelled";
        await _unitOfWork.ProjectInvitationRepository.UpdateAsync(invitation);
        await _unitOfWork.CommitAsync();
        return true;
    }

    public async Task<PaginatedList<ProjectInvitationDto>> GetForProjectAsync(int projectId, int pageIndex = 1, int pageSize = 10)
    {
        var paged = await _unitOfWork.ProjectInvitationRepository.GetPagedAsync(projectId, pageIndex, pageSize); // Uses specific method
        return new PaginatedList<ProjectInvitationDto>(
            paged.Select(x => new ProjectInvitationDto(x)).ToList(),
            paged.TotalItems,
            paged.PageIndex,
            paged.PageSize);
    }

    public async Task<ProjectInvitationDto?> GetByIdAsync(int id)
    {
        var entity = await _unitOfWork.ProjectInvitationRepository.FindByIdAsync(id); // Specific repo
        return entity == null ? null : new ProjectInvitationDto(entity);
    }
}