using FGS_BE.Repo.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FGS_BE.Repo.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Achievement> Achievements { get; set; }
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
    public DbSet<Role> Roles { get; set; }
    public DbSet<Semester> Semesters { get; set; }
    public DbSet<Submission> Submissions { get; set; }
    public DbSet<Entities.Task> Tasks { get; set; }
    public DbSet<TermKeyword> TermKeywords { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserAchievement> UserAchievements { get; set; }
    public DbSet<UserWallet> UserWallets { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}