using System.ComponentModel.DataAnnotations.Schema;

namespace FGS_BE.Repo.Entities;

public class ChatRoom
{
    public int Id { get; set; }

    public string? RoomName { get; set; } = string.Empty;
    public string? RoomType { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }

    [ForeignKey(nameof(UserId))]
    public int UserId { get; set; }
    public virtual User User { get; set; } = default!;

    [ForeignKey(nameof(ProjectId))]
    public int? ProjectId { get; set; } 
    public virtual Project? Project { get; set; }

    public virtual ICollection<ChatParticipant> ChatParticipants { get; set; } = new HashSet<ChatParticipant>();
    public virtual ICollection<ChatMessage> ChatMessages { get; set; } = new HashSet<ChatMessage>();
}