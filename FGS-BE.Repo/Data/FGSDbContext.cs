using Microsoft.EntityFrameworkCore;
using FGS_BE.Repo.Entities;

namespace FGS_BE.Repo.Data;

public class FGSDbContext : DbContext
{
    public FGSDbContext(DbContextOptions<FGSDbContext> options) : base(options) { }

    public DbSet<Items> Items { get; set; }
    public DbSet<ItemsRedeemHistory> ItemsRedeemHistory { get; set; }
    public DbSet<CashRedeemHistory> CashRedeemHistory { get; set; }
    public DbSet<Users> Users { get; set; }
    public DbSet<SemesterMembers> SemesterMembers { get; set; }
    public DbSet<Groups> Groups { get; set; }
    public DbSet<Projects> Projects { get; set; }
    public DbSet<Tasks> Tasks { get; set; }
    public DbSet<TaskAssignment> TaskAssignment { get; set; }
    public DbSet<ConversionRates> ConversionRates { get; set; }
    public DbSet<Payout> Payout { get; set; }
    public DbSet<Roles> Roles { get; set; }
    public DbSet<Levels> Levels { get; set; }
    public DbSet<Milestones> Milestones { get; set; }
    public DbSet<Submissions> Submissions { get; set; }
    public DbSet<ProjectPoints> ProjectPoints { get; set; }
    public DbSet<Semesters> Semesters { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ItemsRedeemHistory relationships
        modelBuilder.Entity<ItemsRedeemHistory>()
            .HasOne(irh => irh.User)
            .WithMany(u => u.ItemsRedeemHistory)
            .HasForeignKey(irh => irh.UserId);

        modelBuilder.Entity<ItemsRedeemHistory>()
            .HasOne(irh => irh.Item)
            .WithMany()
            .HasForeignKey(irh => irh.ItemId);

        // CashRedeemHistory relationships
        modelBuilder.Entity<CashRedeemHistory>()
            .HasOne(crh => crh.User)
            .WithMany(u => u.CashRedeemHistory)
            .HasForeignKey(crh => crh.UserId);

        // SemesterMembers relationships
        modelBuilder.Entity<SemesterMembers>()
            .HasOne(sm => sm.Semester)
            .WithMany(s => s.SemesterMembers)
            .HasForeignKey(sm => sm.SemesterId);

        modelBuilder.Entity<SemesterMembers>()
            .HasOne(sm => sm.User)
            .WithMany(u => u.SemesterMembers)
            .HasForeignKey(sm => sm.UserId);

        // Groups relationships
        modelBuilder.Entity<Groups>()
            .HasOne(g => g.Leader)
            .WithMany()
            .HasForeignKey(g => g.LeaderId);

        // Projects relationships
        modelBuilder.Entity<Projects>()
            .HasOne(p => p.Leader)
            .WithMany(u => u.Projects)
            .HasForeignKey(p => p.LeaderId);

        modelBuilder.Entity<Projects>()
            .HasOne(p => p.Group)
            .WithMany(g => g.Projects)
            .HasForeignKey(p => p.GroupId);

        // Tasks relationships
        modelBuilder.Entity<Tasks>()
            .HasOne(t => t.Project)
            .WithMany(p => p.Tasks)
            .HasForeignKey(t => t.ProjectId);

        modelBuilder.Entity<Tasks>()
            .HasOne(t => t.User)
            .WithMany(u => u.Tasks)
            .HasForeignKey(t => t.UserId);

        // TaskAssignment relationships
        modelBuilder.Entity<TaskAssignment>()
            .HasOne(ta => ta.Task)
            .WithMany()
            .HasForeignKey(ta => ta.TaskId);

        modelBuilder.Entity<TaskAssignment>()
            .HasOne(ta => ta.User)
            .WithMany(u => u.TaskAssignments)
            .HasForeignKey(ta => ta.UserId);

        // Users relationships
        modelBuilder.Entity<Users>()
            .HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId);

        modelBuilder.Entity<Users>()
            .HasOne(u => u.Level)
            .WithMany(l => l.Users)
            .HasForeignKey(u => u.LevelId);

        // Milestones relationships
        modelBuilder.Entity<Milestones>()
            .HasOne(m => m.Project)
            .WithMany(p => p.Milestones)
            .HasForeignKey(m => m.ProjectId);

        // Submissions relationships
        modelBuilder.Entity<Submissions>()
            .HasOne(s => s.Project)
            .WithMany(p => p.Submissions)
            .HasForeignKey(s => s.ProjectId);

        modelBuilder.Entity<Submissions>()
            .HasOne(s => s.User)
            .WithMany(u => u.Submissions)
            .HasForeignKey(s => s.UserId);

        // ProjectPoints relationships
        modelBuilder.Entity<ProjectPoints>()
            .HasOne(pp => pp.Project)
            .WithMany(p => p.ProjectPoints)
            .HasForeignKey(pp => pp.ProjectId);

        modelBuilder.Entity<ProjectPoints>()
            .HasOne(pp => pp.Milestone)
            .WithMany()
            .HasForeignKey(pp => pp.MilestoneId);

        modelBuilder.Entity<ProjectPoints>()
            .HasOne(pp => pp.GrantedByUser)
            .WithMany()
            .HasForeignKey(pp => pp.GrantedBy);
    }
}