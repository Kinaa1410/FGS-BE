using FGS_BE.Repo.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FGS_BE.Repo.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<User, Role, int, IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>(options)
{
    private const string Prefix = "AspNet";
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
    public DbSet<RedeemRequest> RedeemRequests { get; set; }
    public DbSet<RewardItem> RewardItems { get; set; }
    public DbSet<Semester> Semesters { get; set; }
    public DbSet<Submission> Submissions { get; set; }
    public DbSet<Entities.Task> Tasks { get; set; }
    public DbSet<TermKeyword> TermKeywords { get; set; }
    public DbSet<UserWallet> UserWallets { get; set; }
    public DbSet<SemesterMember> SemesterMembers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var tableName = entityType.GetTableName();
            if (tableName != null && tableName.StartsWith(Prefix))
            {
                entityType.SetTableName(tableName.Substring(6));
            }
        }

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

        // Projects <-> Proposer (Users) - one-to-many, required
        modelBuilder.Entity<Project>()
            .HasOne(p => p.Proposer)
            .WithMany()
            .HasForeignKey(p => p.ProposerId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
        modelBuilder.Entity<Project>().HasIndex(p => p.ProposerId);

        // Projects <-> Semester - one-to-many, required
        // FIXED: Added inverse collection reference to avoid convention duplicate
        modelBuilder.Entity<Project>()
            .HasOne(p => p.Semester)
            .WithMany(s => s.Projects)
            .HasForeignKey(p => p.SemesterId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
        modelBuilder.Entity<Project>().HasIndex(p => p.SemesterId);

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

        // PerformanceScores <-> Projects, Users, Milestones, Tasks
        // FIXED: Added inverse collection references for all to avoid duplicates
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

        // Milestones <-> Projects (one-to-many, cascade)
        modelBuilder.Entity<Milestone>()
            .HasOne(m => m.Project)
            .WithMany(p => p.Milestones)
            .HasForeignKey(m => m.ProjectId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
        modelBuilder.Entity<Milestone>().HasIndex(m => m.ProjectId);

        // Tasks <-> Milestones & Self (ParentTask)
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

        // Submissions <-> Tasks & Users
        // FIXED: Added inverse collection reference for User
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

        // ChatRooms <-> Projects (one-to-many, optional)
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

        // ChatParticipants <-> ChatRooms & Users
        // FIXED: Added inverse collection reference for User
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

        // ChatMessages <-> ChatRooms & Sender (Users)
        // FIXED: Added inverse collection references for both ChatRoom and User
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

        // PointTransactions <-> UserWallets (one-to-many, required)
        modelBuilder.Entity<PointTransaction>()
            .HasOne(pt => pt.UserWallet)
            .WithMany(w => w.PointTransactions)
            .HasForeignKey(pt => pt.UserWalletId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
        modelBuilder.Entity<PointTransaction>().HasIndex(pt => pt.UserWalletId);

        // RewardItems <-> CreatedBy (Users) (one-to-many, optional)
        modelBuilder.Entity<RewardItem>()
            .HasOne(ri => ri.CreatedBy)
            .WithMany()
            .HasForeignKey(ri => ri.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<RewardItem>().HasIndex(ri => ri.CreatedById);

        // RedeemRequests <-> Users & RewardItems
        // FIXED: Added inverse collection reference for User
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

        // Notifications <-> Users & NotificationTemplates
        // FIXED: Added inverse collection references for both User and NotificationTemplate
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

        // EmailQueue <-> Notifications (one-to-one, required) - Fixed lambdas
        modelBuilder.Entity<EmailQueue>()
            .HasOne(eq => eq.Notification)
            .WithOne(n => n.EmailQueue)
            .HasForeignKey<EmailQueue>(eq => eq.NotificationId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
        modelBuilder.Entity<EmailQueue>().HasIndex(eq => eq.NotificationId);

        // TermKeywords <-> Semesters (one-to-many, cascade)
        // FIXED: Added inverse collection reference
        modelBuilder.Entity<TermKeyword>()
            .HasOne(tk => tk.Semester)
            .WithMany(s => s.TermKeywords)
            .HasForeignKey(tk => tk.SemesterId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
        modelBuilder.Entity<TermKeyword>().HasIndex(tk => tk.SemesterId);

        // ProjectKeywords <-> TermKeywords & Projects (many-to-one)
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