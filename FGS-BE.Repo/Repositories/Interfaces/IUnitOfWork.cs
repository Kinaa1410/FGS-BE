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

    IGenericRepository<T> Repository<T>() where T : class;
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync();
}