using Microsoft.EntityFrameworkCore.Storage;
using System.Data; // Changed: Use System.Data.IsolationLevel for EF Core

namespace FGS_BE.Repo.Repositories.Interfaces;
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
    IProjectInvitationRepository ProjectInvitationRepository { get; }
    IGenericRepository<T> Repository<T>() where T : class;
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default); // Now uses System.Data.IsolationLevel
    Task RollbackAsync();
}