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
        private bool _disposed = false;

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
        public IUserProjectStatsRepository UserProjectStatsRepository { get; }
        public IUserWalletRepository UserWalletRepository { get; }
        public ISemesterMemberRepository SemesterMemberRepository { get; }
        public IWalletRepository WalletRepository { get; }

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
            IUserProjectStatsRepository userProjectStatsRepository,
            IUserWalletRepository userWalletRepository,
            ISemesterMemberRepository semesterMemberRepository,
            IWalletRepository walletRepository)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
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
            UserProjectStatsRepository = userProjectStatsRepository;
            UserWalletRepository = userWalletRepository;
            SemesterMemberRepository = semesterMemberRepository;
            WalletRepository = walletRepository;
        }

        public IGenericRepository<T> Repository<T>() where T : class
        {
            var typeName = typeof(T).Name;

            if (!_repositories.ContainsKey(typeName))
            {
                var repoType = typeof(GenericRepository<>).MakeGenericType(typeof(T));
                var repoInstance = Activator.CreateInstance(repoType, _context);
                _repositories[typeName] = repoInstance!;
            }

            return (IGenericRepository<T>)_repositories[typeName]!;
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
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}