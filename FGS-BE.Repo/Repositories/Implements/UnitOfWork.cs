using FGS_BE.Repo.Data;
using FGS_BE.Repo.Repositories.Interfaces;
using System.Collections;

namespace FGS_BE.Repo.Repositories.Implements;
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private Hashtable? _repositories;
    private bool disposed = false;

    //
    public ISemesterRepository SemesterRepository { get; }
    public IRewardItemRepository RewardItemRepository { get; }
    public ITermKeywordRepository TermKeywordRepository { get; }
    public IProjectRepository ProjectRepository { get; }
    public IMilestoneRepository MilestoneRepository { get; }

    public UnitOfWork(ApplicationDbContext context, ISemesterRepository semesterRepository, 
        IRewardItemRepository rewardItemRepository, ITermKeywordRepository termKeywordRepository, 
        IProjectRepository projectRepository, IMilestoneRepository milestoneRepository)
    {
        _context = context;
        SemesterRepository = semesterRepository;
        RewardItemRepository = rewardItemRepository;
        TermKeywordRepository = termKeywordRepository;
        ProjectRepository = projectRepository;
        MilestoneRepository = milestoneRepository;
    }
    //

    public IGenericRepository<T> Repository<T>() where T : class
    {
        if (_repositories == null)
            _repositories = new Hashtable();

        var type = typeof(T).Name;

        if (!_repositories.ContainsKey(type))
        {
            var repositoryType = typeof(GenericRepository<>);

            var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), _context);

            _repositories.Add(type, repositoryInstance);
        }

        return (IGenericRepository<T>)_repositories[type];
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync();
    }

    public Task RollbackAsync()
    {
        _context.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
        return Task.CompletedTask;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
        disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

}