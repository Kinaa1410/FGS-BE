using EntityFrameworkCore.Projectables;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace FGS_BE.Repo.Entities;

public class User : IdentityUser<int>
{
    public string? FullName { get; set; }

    public string? StudentCode { get; set; }

    public string? Status { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? VerificationToken { get; set; } = null;
    public virtual ICollection<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();

    [Projectable]
    [NotMapped]
    public IEnumerable<Role> Roles => UserRoles.Select(ur => ur.Role);

    public virtual UserWallet UserWallet { get; set; } = default!;

    public virtual ICollection<RedeemRequest> RedeemRequests { get; set; } = new HashSet<RedeemRequest>();
    public virtual ICollection<PerformanceScore> PerformanceScores { get; set; } = new HashSet<PerformanceScore>();
    public virtual ICollection<Submission> Submissions { get; set; } = new HashSet<Submission>();
    public virtual ICollection<ProjectMember> ProjectMembers { get; set; } = new HashSet<ProjectMember>();
    public virtual ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();

    public virtual ICollection<ChatParticipant> ChatParticipants { get; set; } = new HashSet<ChatParticipant>();

    public virtual ICollection<ChatMessage> ChatMessages { get; set; } = new HashSet<ChatMessage>();

    public virtual ICollection<ChatRoom> ChatRooms { get; set; } = new HashSet<ChatRoom>();

    public virtual ICollection<Task> AssignedTasks { get; set; } = new HashSet<Task>();
    public virtual ICollection<UserLevel> UserLevels { get; set; } = new List<UserLevel>();
    public virtual ICollection<SemesterMember> SemesterMembers { get; set; } = new HashSet<SemesterMember>();
}