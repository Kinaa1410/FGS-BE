using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FGS_BE.Repo.Entities;

public class User
{
    public int Id { get; set; }

    [StringLength(100, MinimumLength = 2)]
    public string FullName { get; set; } = string.Empty;

    [EmailAddress]
    [StringLength(255)]
    public string Email { get; set; } = string.Empty;

    [StringLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    [StringLength(20)]
    public string StudentCode { get; set; } = string.Empty;

    [Phone]
    [StringLength(20)]
    public string Phone { get; set; } = string.Empty;

    public string? Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    [ForeignKey(nameof(RoleId))]
    public int? RoleId { get; set; }
    public virtual Role? Role { get; set; } = default!;

    public virtual UserWallet UserWallet { get; set; } = default!;

    public virtual ICollection<UserAchievement> UserAchievements { get; set; } = new HashSet<UserAchievement>();
    public virtual ICollection<RedeemRequest> RedeemRequests { get; set; } = new HashSet<RedeemRequest>();
    public virtual ICollection<PerformanceScore> PerformanceScores { get; set; } = new HashSet<PerformanceScore>();
    public virtual ICollection<Submission> Submissions { get; set; } = new HashSet<Submission>();
    public virtual ICollection<ProjectMember> ProjectMembers { get; set; } = new HashSet<ProjectMember>();
    public virtual ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();

    public virtual ICollection<ChatParticipant> ChatParticipants { get; set; } = new HashSet<ChatParticipant>();

    public virtual ICollection<ChatMessage> ChatMessages { get; set; } = new HashSet<ChatMessage>();

    public virtual ICollection<ChatRoom> ChatRooms { get; set; } = new HashSet<ChatRoom>();

    public virtual ICollection<Task> AssignedTasks { get; set; } = new HashSet<Task>();
}