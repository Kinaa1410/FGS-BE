using FGS_BE.Repo.Data;
using FGS_BE.Repo.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections;
using System.Data;

namespace FGS_BE.Repo.Repositories.Implements
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly Hashtable _repositories = new();
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
        public IUserRepository UserRepository { get; }
        public IProjectInvitationRepository ProjectInvitationRepository { get; }
        public INotificationRepository NotificationRepository { get; }
        public INotificationTemplateRepository NotificationTemplateRepository { get; }
        public IUserProjectStatsRepository UserProjectStatsRepository { get; }  // New: For escalation threshold
        public IUserWalletRepository UserWalletRepository { get; }

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
            IUserRepository userRepository,
            IProjectInvitationRepository projectInvitationRepository,
            INotificationRepository notificationRepository,
            INotificationTemplateRepository notificationTemplateRepository,
            IUserProjectStatsRepository userProjectStatsRepository  // New: Inject this
,
            IUserWalletRepository userWalletRepository



        )
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
            UserRepository = userRepository;
            ProjectInvitationRepository = projectInvitationRepository;
            NotificationRepository = notificationRepository;
            NotificationTemplateRepository = notificationTemplateRepository;
            UserProjectStatsRepository = userProjectStatsRepository;  // New: Assign
            UserWalletRepository = userWalletRepository;
        }

        public IGenericRepository<T> Repository<T>() where T : class
        {
            var type = typeof(T).Name;
            if (!_repositories.ContainsKey(type))
            {
                var repoType = typeof(GenericRepository<>).MakeGenericType(typeof(T));
                var repoInstance = Activator.CreateInstance(repoType, _context);
                _repositories[type] = repoInstance!;
            }
            return (IGenericRepository<T>)_repositories[type]!;
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync(
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
            CancellationToken cancellationToken = default)
        {
            return await _context.Database.BeginTransactionAsync(isolationLevel, cancellationToken);
        }

        public async Task RollbackAsync()
        {
            foreach (var entry in _context.ChangeTracker.Entries())
            {
                await entry.ReloadAsync();
            }
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