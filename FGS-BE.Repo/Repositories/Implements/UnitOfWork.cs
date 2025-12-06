using FGS_BE.Repo.Data;
using FGS_BE.Repo.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections;
using System.Data; // For IsolationLevel

namespace FGS_BE.Repo.Repositories.Implements
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private Hashtable? _repositories;
        private bool disposed = false;

        public ISemesterRepository SemesterRepository { get; }
        public IRewardItemRepository RewardItemRepository { get; }
        public ITermKeywordRepository TermKeywordRepository { get; }
        public IProjectRepository ProjectRepository { get; }
        public IMilestoneRepository MilestoneRepository { get; }
        public ITaskRepository TaskRepository { get; }
        public IRedeemRequestRepository RedeemRequestRepository { get; }
        public ISubmissionRepository SubmissionRepository { get; }
        public IProjectMemberRepository ProjectMemberRepository { get; }
        public IPerformanceScoreRepository PerformanceScoreRepository { get; }
        public IProjectInvitationRepository ProjectInvitationRepository { get; }

        public UnitOfWork(
            ApplicationDbContext context,
            ISemesterRepository semesterRepository,
            IRewardItemRepository rewardItemRepository,
            ITermKeywordRepository termKeywordRepository,
            IProjectRepository projectRepository,
            IMilestoneRepository milestoneRepository,
            ITaskRepository taskRepository,
            IRedeemRequestRepository redeemRequestRepository,
            ISubmissionRepository submissionRepository,
            IProjectMemberRepository projectMemberRepository,
            IPerformanceScoreRepository performanceScoreRepository,
            IProjectInvitationRepository projectInvitationRepository)
        {
            _context = context;
            SemesterRepository = semesterRepository;
            RewardItemRepository = rewardItemRepository;
            TermKeywordRepository = termKeywordRepository;
            ProjectRepository = projectRepository;
            MilestoneRepository = milestoneRepository;
            TaskRepository = taskRepository;
            RedeemRequestRepository = redeemRequestRepository;
            SubmissionRepository = submissionRepository;
            ProjectMemberRepository = projectMemberRepository;
            PerformanceScoreRepository = performanceScoreRepository;
            ProjectInvitationRepository = projectInvitationRepository;
        }

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
            return (IGenericRepository<T>)_repositories[type]!;
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default)
        {
            return await _context.Database.BeginTransactionAsync(isolationLevel, cancellationToken);
        }

        public async Task RollbackAsync()
        {
            _context.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
            await Task.CompletedTask;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}