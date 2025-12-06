using FGS_BE.Repo.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FGS_BE.Service.Implements;

public class InvitationExpiryService : BackgroundService
{
    private readonly IServiceProvider _provider;
    private readonly ILogger<InvitationExpiryService> _logger;

    public InvitationExpiryService(IServiceProvider provider, ILogger<InvitationExpiryService> logger)
    {
        _provider = provider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _provider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var repo = unitOfWork.ProjectInvitationRepository.Entities;

            var expired = await repo
                .Where(pi => pi.Status == "pending" && pi.ExpiryAt < DateTime.UtcNow)
                .ToListAsync(stoppingToken);

            foreach (var invite in expired)
            {
                var project = await unitOfWork.ProjectRepository.FindByIdAsync(invite.ProjectId);
                if (project != null)
                {
                    project.ReservedMembers = Math.Max(0, project.ReservedMembers - 1);
                    await unitOfWork.ProjectRepository.UpdateAsync(project);
                }
                invite.Status = "expired";
                await unitOfWork.ProjectInvitationRepository.UpdateAsync(invite);
            }

            await unitOfWork.CommitAsync();
            _logger.LogInformation("Released {Count} expired invitation locks.", expired.Count);

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Check every minute
        }
    }
}