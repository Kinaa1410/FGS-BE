using FGS_BE.Repo.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FGS_BE.Service.Implements
{
    public class InvitationExpiryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<InvitationExpiryService> _logger;

        public InvitationExpiryService(IUnitOfWork unitOfWork, ILogger<InvitationExpiryService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task ProcessExpiredInvitationsAsync()
        {
            var repo = _unitOfWork.ProjectInvitationRepository.Entities;
            var expired = await repo
                .Where(pi => pi.Status == "pending" && pi.ExpiryAt < DateTime.UtcNow)
                .ToListAsync();

            foreach (var invite in expired)
            {
                var project = await _unitOfWork.ProjectRepository.FindByIdAsync(invite.ProjectId);
                if (project != null)
                {
                    project.ReservedMembers = Math.Max(0, project.ReservedMembers - 1);
                    await _unitOfWork.ProjectRepository.UpdateAsync(project);
                }
                invite.Status = "expired";
                await _unitOfWork.ProjectInvitationRepository.UpdateAsync(invite);
            }

            await _unitOfWork.CommitAsync();
            _logger.LogInformation("Released {Count} expired invitation locks.", expired.Count);
        }
    }
}