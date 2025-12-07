using System.ComponentModel.DataAnnotations.Schema;
using FGS_BE.Repo.Enums; // For ProjectStatus
namespace FGS_BE.Repo.Entities;
public class Project
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ProjectStatus Status { get; set; } = ProjectStatus.Open;
    public decimal TotalPoints { get; set; }
    public DateTime CreatedAt { get; set; }
    // Member limits
    public int MinMembers { get; set; } = 2;
    public int MaxMembers { get; set; } = 10;
    public int CurrentMembers { get; set; } = 0;
    // NEW: For locked slots (invites)
    public int ReservedMembers { get; set; } = 0; // Incremented on invite, decremented on accept/deny/expiry
    [ForeignKey(nameof(SemesterId))]
    public int SemesterId { get; set; }
    public virtual Semester Semester { get; set; } = default!;
    [ForeignKey(nameof(ProposerId))]
    public int ProposerId { get; set; }
    public virtual User Proposer { get; set; } = default!;

    [ForeignKey(nameof(MentorId))]
    public int? MentorId { get; set; } // Nullable for optional assignment
    public virtual User? Mentor { get; set; } // Optional navigation
    // Navigation properties
    public virtual ICollection<ChatRoom> ChatRooms { get; set; } = new HashSet<ChatRoom>();
    public virtual ICollection<Milestone> Milestones { get; set; } = new HashSet<Milestone>();
    public virtual ICollection<PerformanceScore> PerformanceScores { get; set; } = new HashSet<PerformanceScore>();
    public virtual ICollection<ProjectKeyword> ProjectKeywords { get; set; } = new HashSet<ProjectKeyword>();
    public virtual ICollection<ProjectMember> ProjectMembers { get; set; } = new HashSet<ProjectMember>();
    public virtual ICollection<ProjectInvitation> ProjectInvitations { get; set; } = new HashSet<ProjectInvitation>();
}