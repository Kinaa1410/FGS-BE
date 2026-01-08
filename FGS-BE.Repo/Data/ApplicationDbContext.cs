using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FGS_BE.Repo.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : IdentityDbContext<User, Role, int, IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>(options)
    {
        private const string Prefix = "AspNet";

        // DbSets
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<ChatParticipant> ChatParticipants { get; set; }
        public DbSet<ChatRoom> ChatRooms { get; set; }
        public DbSet<EmailQueue> EmailQueues { get; set; }
        public DbSet<Milestone> Milestones { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationTemplate> NotificationTemplates { get; set; }
        public DbSet<PerformanceScore> PerformanceScores { get; set; }
        public DbSet<PointTransaction> PointTransactions { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectKeyword> ProjectKeywords { get; set; }
        public DbSet<ProjectMember> ProjectMembers { get; set; }
        public DbSet<ProjectInvitation> ProjectInvitations { get; set; }
        public DbSet<RedeemRequest> RedeemRequests { get; set; }
        public DbSet<RewardItem> RewardItems { get; set; }
        public DbSet<Semester> Semesters { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<Entities.Task> Tasks { get; set; }
        public DbSet<TermKeyword> TermKeywords { get; set; }
        public DbSet<UserWallet> UserWallets { get; set; }
        public DbSet<SemesterMember> SemesterMembers { get; set; }
        public DbSet<UserLevel> UserLevels { get; set; }
        public DbSet<UserProjectStats> UserProjectStats { get; set; }  // New: For escalation threshold tracking

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // Rename AspNet tables
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName != null && tableName.StartsWith(Prefix))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }

            // New: UserProjectStats configuration (for escalation threshold)
            modelBuilder.Entity<UserProjectStats>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FailureCount).HasDefaultValue(0);
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.HasIndex(e => new { e.UserId, e.ProjectId }).IsUnique();  // Prevent duplicate stats per user-project
                entity.HasOne(e => e.User)
                    .WithMany(u => u.ProjectStats)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
                entity.HasOne(e => e.Project)
                    .WithMany(p => p.UserStats)
                    .HasForeignKey(e => e.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.ProjectId);
            });

            // Updated: Submission configuration (added new fields)
            modelBuilder.Entity<Submission>(entity =>
            {
                entity.Property(s => s.IsResubmission).HasDefaultValue(false);
                entity.Property(s => s.RejectionDate).IsRequired(false);
                entity.Property(e => e.FileUrl).HasMaxLength(500);
                entity.Property(e => e.Feedback).HasMaxLength(500);
                entity.HasIndex(s => new { s.UserId, s.TaskId, s.Status });  // For grace period queries
            });

            // Updated: Milestone configuration (added new fields)
            modelBuilder.Entity<Milestone>(entity =>
            {
                entity.Property(m => m.IsDelayed).HasDefaultValue(false);
                entity.Property(m => m.OriginalDueDate).IsRequired(false);
                entity.Property(e => e.Title).HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
            });

            // ProjectMembers <-> Projects & Users (many-to-many junction)
            modelBuilder.Entity<ProjectMember>()
                .HasOne(pm => pm.Project)
                .WithMany(p => p.ProjectMembers)
                .HasForeignKey(pm => pm.ProjectId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
            modelBuilder.Entity<ProjectMember>()
                .HasOne(pm => pm.User)
                .WithMany(u => u.ProjectMembers)
                .HasForeignKey(pm => pm.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
            modelBuilder.Entity<ProjectMember>().HasIndex(pm => pm.ProjectId);
            modelBuilder.Entity<ProjectMember>().HasIndex(pm => pm.UserId);

            // ProjectInvitations config
            modelBuilder.Entity<ProjectInvitation>()
                .HasOne(pi => pi.Project)
                .WithMany(p => p.ProjectInvitations)
                .HasForeignKey(pi => pi.ProjectId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            modelBuilder.Entity<ProjectInvitation>()
                .HasOne(pi => pi.Inviter)
                .WithMany()
                .HasForeignKey(pi => pi.InviterId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
            modelBuilder.Entity<ProjectInvitation>()
                .HasOne(pi => pi.InvitedUser)
                .WithMany()
                .HasForeignKey(pi => pi.InvitedUserId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            // Indexes for performance/queries
            modelBuilder.Entity<ProjectInvitation>().HasIndex(pi => pi.ProjectId);
            modelBuilder.Entity<ProjectInvitation>().HasIndex(pi => pi.InviterId);
            modelBuilder.Entity<ProjectInvitation>().HasIndex(pi => pi.InvitedUserId);
            modelBuilder.Entity<ProjectInvitation>().HasIndex(pi => pi.InviteCode).IsUnique(); // For fast lookup/validation
            modelBuilder.Entity<ProjectInvitation>().HasIndex(pi => pi.Status); // For filtering pending/accepted
            modelBuilder.Entity<ProjectInvitation>().HasIndex(pi => pi.ExpiryAt); // For expiry sweeps

            // Optional: Property configs (defaults, lengths)
            modelBuilder.Entity<ProjectInvitation>(entity =>
            {
                entity.Property(pi => pi.Status).HasMaxLength(20).HasDefaultValue("pending");
                entity.Property(pi => pi.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(pi => pi.ExpiryAt).HasDefaultValueSql("DATEADD(MINUTE, 15, GETUTCDATE())"); // 15-min expiry
                entity.Property(pi => pi.InviteCode).HasMaxLength(50);
                entity.Property(pi => pi.Message).HasMaxLength(500);
            });

            // Project relationships (grouped for clarity)
            // Projects <-> Proposer (Users) - one-to-many, required
            modelBuilder.Entity<Project>()
                .HasOne(p => p.Proposer)
                .WithMany()
                .HasForeignKey(p => p.ProposerId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
            modelBuilder.Entity<Project>().HasIndex(p => p.ProposerId);

            // Projects <-> Mentor (Users) - one-to-many, optional
            modelBuilder.Entity<Project>()
                .HasOne(p => p.Mentor)
                .WithMany()
                .HasForeignKey(p => p.MentorId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);
            modelBuilder.Entity<Project>().HasIndex(p => p.MentorId);

            // Projects <-> Semester - one-to-many, required
            modelBuilder.Entity<Project>()
                .HasOne(p => p.Semester)
                .WithMany(s => s.Projects)
                .HasForeignKey(p => p.SemesterId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            modelBuilder.Entity<Project>().HasIndex(p => p.SemesterId);

            // Project property configs (limits, status, etc.)
            modelBuilder.Entity<Project>(entity =>
            {
                entity.Property(e => e.MinMembers).HasDefaultValue(2);
                entity.Property(e => e.MaxMembers).HasDefaultValue(10);
                entity.Property(e => e.CurrentMembers).HasDefaultValue(0);
                entity.HasIndex(e => e.CurrentMembers); // For fast limit checks
                entity.Property(e => e.ReservedMembers).HasDefaultValue(0);
                entity.HasIndex(e => e.ReservedMembers); // For fast queries
                entity.Property(p => p.Status)
                    .HasConversion<string>()
                    .HasDefaultValue(ProjectStatus.Open)
                    .HasMaxLength(20);
            });

            // SemesterMembers config
            modelBuilder.Entity<SemesterMember>()
                .HasKey(sm => new { sm.SemesterId, sm.UserId });
            modelBuilder.Entity<SemesterMember>()
                .HasOne(sm => sm.Semester)
                .WithMany(s => s.SemesterMembers)
                .HasForeignKey(sm => sm.SemesterId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
            modelBuilder.Entity<SemesterMember>()
                .HasOne(sm => sm.User)
                .WithMany(u => u.SemesterMembers)
                .HasForeignKey(sm => sm.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
            modelBuilder.Entity<SemesterMember>().HasIndex(sm => sm.SemesterId);
            modelBuilder.Entity<SemesterMember>().HasIndex(sm => sm.UserId);

            // PerformanceScores relationships
            modelBuilder.Entity<PerformanceScore>()
                .HasOne(ps => ps.Project)
                .WithMany(p => p.PerformanceScores)
                .HasForeignKey(ps => ps.ProjectId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
            modelBuilder.Entity<PerformanceScore>()
                .HasOne(ps => ps.User)
                .WithMany(u => u.PerformanceScores)
                .HasForeignKey(ps => ps.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
            modelBuilder.Entity<PerformanceScore>()
                .HasOne(ps => ps.Milestone)
                .WithMany(m => m.PerformanceScores)
                .HasForeignKey(ps => ps.MilestoneId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<PerformanceScore>()
                .HasOne(ps => ps.Task)
                .WithMany(t => t.PerformanceScores)
                .HasForeignKey(ps => ps.TaskId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<PerformanceScore>().HasIndex(ps => ps.ProjectId);
            modelBuilder.Entity<PerformanceScore>().HasIndex(ps => ps.UserId);
            modelBuilder.Entity<PerformanceScore>().HasIndex(ps => ps.MilestoneId);
            modelBuilder.Entity<PerformanceScore>().HasIndex(ps => ps.TaskId);

            // Milestones <-> Projects
            modelBuilder.Entity<Milestone>()
                .HasOne(m => m.Project)
                .WithMany(p => p.Milestones)
                .HasForeignKey(m => m.ProjectId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            modelBuilder.Entity<Milestone>().HasIndex(m => m.ProjectId);

            // Tasks relationships
            modelBuilder.Entity<Entities.Task>()
                .HasOne(t => t.Milestone)
                .WithMany(m => m.Tasks)
                .HasForeignKey(t => t.MilestoneId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            modelBuilder.Entity<Entities.Task>()
                .HasOne(t => t.ParentTask)
                .WithMany(t => t.SubTasks)
                .HasForeignKey(t => t.ParentTaskId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Entities.Task>()
                .HasOne(t => t.Assignee)
                .WithMany(u => u.AssignedTasks)
                .HasForeignKey(t => t.AssigneeId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Entities.Task>().HasIndex(t => t.MilestoneId);
            modelBuilder.Entity<Entities.Task>().HasIndex(t => t.ParentTaskId);
            modelBuilder.Entity<Entities.Task>().HasIndex(t => t.AssigneeId);

            // Submissions relationships
            modelBuilder.Entity<Submission>()
                .HasOne(s => s.Task)
                .WithMany(t => t.Submissions)
                .HasForeignKey(s => s.TaskId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
            modelBuilder.Entity<Submission>()
                .HasOne(s => s.User)
                .WithMany(u => u.Submissions)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
            modelBuilder.Entity<Submission>().HasIndex(s => s.TaskId);
            modelBuilder.Entity<Submission>().HasIndex(s => s.UserId);

            // ChatRooms relationships
            modelBuilder.Entity<ChatRoom>()
                .HasOne(cr => cr.Project)
                .WithMany(p => p.ChatRooms)
                .HasForeignKey(cr => cr.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ChatRoom>()
                .HasOne(cr => cr.User)
                .WithMany(u => u.ChatRooms)
                .HasForeignKey(cr => cr.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
            modelBuilder.Entity<ChatRoom>().HasIndex(cr => cr.ProjectId);
            modelBuilder.Entity<ChatRoom>().HasIndex(cr => cr.UserId);

            // ChatParticipants relationships
            modelBuilder.Entity<ChatParticipant>()
                .HasOne(cp => cp.ChatRoom)
                .WithMany(cr => cr.ChatParticipants)
                .HasForeignKey(cp => cp.ChatRoomId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            modelBuilder.Entity<ChatParticipant>()
                .HasOne(cp => cp.User)
                .WithMany(u => u.ChatParticipants)
                .HasForeignKey(cp => cp.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
            modelBuilder.Entity<ChatParticipant>().HasIndex(cp => cp.ChatRoomId);
            modelBuilder.Entity<ChatParticipant>().HasIndex(cp => cp.UserId);

            // ChatMessages relationships
            modelBuilder.Entity<ChatMessage>()
                .HasOne(cm => cm.ChatRoom)
                .WithMany(cr => cr.ChatMessages)
                .HasForeignKey(cm => cm.ChatRoomId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            modelBuilder.Entity<ChatMessage>()
                .HasOne(cm => cm.Sender)
                .WithMany(u => u.ChatMessages)
                .HasForeignKey(cm => cm.SenderId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
            modelBuilder.Entity<ChatMessage>().HasIndex(cm => cm.ChatRoomId);
            modelBuilder.Entity<ChatMessage>().HasIndex(cm => cm.SenderId);

            // PointTransactions <-> UserWallets
            modelBuilder.Entity<PointTransaction>()
                .HasOne(pt => pt.UserWallet)
                .WithMany(w => w.PointTransactions)
                .HasForeignKey(pt => pt.UserWalletId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
            modelBuilder.Entity<PointTransaction>().HasIndex(pt => pt.UserWalletId);

            // RewardItems <-> CreatedBy (Users)
            modelBuilder.Entity<RewardItem>()
                .HasOne(ri => ri.CreatedBy)
                .WithMany()
                .HasForeignKey(ri => ri.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RewardItem>().HasIndex(ri => ri.CreatedById);

            // RedeemRequests relationships
            modelBuilder.Entity<RedeemRequest>()
                .HasOne(rr => rr.User)
                .WithMany(u => u.RedeemRequests)
                .HasForeignKey(rr => rr.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
            modelBuilder.Entity<RedeemRequest>()
                .HasOne(rr => rr.RewardItem)
                .WithMany(ri => ri.RedeemRequests)
                .HasForeignKey(rr => rr.RewardItemId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
            modelBuilder.Entity<RedeemRequest>().HasIndex(rr => rr.UserId);
            modelBuilder.Entity<RedeemRequest>().HasIndex(rr => rr.RewardItemId);

            // Notifications relationships
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.NotificationTemplate)
                .WithMany(nt => nt.Notifications)
                .HasForeignKey(n => n.NotificationTemplateId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
            modelBuilder.Entity<Notification>().HasIndex(n => n.UserId);
            modelBuilder.Entity<Notification>().HasIndex(n => n.NotificationTemplateId);

            // EmailQueue <-> Notifications (one-to-one)
            modelBuilder.Entity<EmailQueue>()
                .HasOne(eq => eq.Notification)
                .WithOne(n => n.EmailQueue)
                .HasForeignKey<EmailQueue>(eq => eq.NotificationId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
            modelBuilder.Entity<EmailQueue>().HasIndex(eq => eq.NotificationId);

            // TermKeywords <-> Semesters
            modelBuilder.Entity<TermKeyword>()
                .HasOne(tk => tk.Semester)
                .WithMany(s => s.TermKeywords)
                .HasForeignKey(tk => tk.SemesterId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            modelBuilder.Entity<TermKeyword>().HasIndex(tk => tk.SemesterId);

            // ProjectKeywords relationships
            modelBuilder.Entity<ProjectKeyword>()
                .HasOne(pk => pk.TermKeyword)
                .WithMany(tk => tk.ProjectKeywords)
                .HasForeignKey(pk => pk.TermKeywordId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
            modelBuilder.Entity<ProjectKeyword>()
                .HasOne(pk => pk.Project)
                .WithMany(p => p.ProjectKeywords)
                .HasForeignKey(pk => pk.ProjectId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            modelBuilder.Entity<ProjectKeyword>().HasIndex(pk => pk.TermKeywordId);
            modelBuilder.Entity<ProjectKeyword>().HasIndex(pk => pk.ProjectId);

            // UserLevel relationships
            modelBuilder.Entity<UserLevel>()
                .HasOne(ul => ul.User)
                .WithMany(u => u.UserLevels)
                .HasForeignKey(ul => ul.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
            modelBuilder.Entity<UserLevel>()
                .HasOne(ul => ul.Level)
                .WithMany(l => l.UserLevels)
                .HasForeignKey(ul => ul.LevelId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
            modelBuilder.Entity<UserLevel>().HasIndex(ul => ul.UserId);
            modelBuilder.Entity<UserLevel>().HasIndex(ul => ul.LevelId);
        }
    }
}