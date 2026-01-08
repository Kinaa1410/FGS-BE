using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace FGS_BE.Repo.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ISemesterRepository SemesterRepository { get; }
        IRewardItemRepository RewardItemRepository { get; }
        ITermKeywordRepository TermKeywordRepository { get; }
        IProjectRepository ProjectRepository { get; }
        IMilestoneRepository MilestoneRepository { get; }
        ITaskRepository TaskRepository { get; }
        IRedeemRequestRepository RedeemRequestRepository { get; }
        ISubmissionRepository SubmissionRepository { get; }
        IProjectMemberRepository ProjectMemberRepository { get; }
        IPerformanceScoreRepository PerformanceScoreRepository { get; }
        IUserRepository UserRepository { get; }
        IProjectInvitationRepository ProjectInvitationRepository { get; }
        INotificationRepository NotificationRepository { get; }
        INotificationTemplateRepository NotificationTemplateRepository { get; }
        IUserProjectStatsRepository UserProjectStatsRepository { get; }
        IGenericRepository<T> Repository<T>() where T : class;

        Task CommitAsync(CancellationToken cancellationToken = default);

        Task<IDbContextTransaction> BeginTransactionAsync(
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
            CancellationToken cancellationToken = default);

        Task RollbackAsync();
    }
}
