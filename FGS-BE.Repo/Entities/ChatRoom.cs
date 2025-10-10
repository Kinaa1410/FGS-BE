namespace FGS_BE.Repo.Entities;
public class ChatRoom
{
    public int Id { get; set; }

    public string? RoomName { get; set; } = string.Empty;
    public string? RoomType { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }

    public int UserId { get; set; }
    public virtual User User { get; set; } = default!;
    public int ProjectId { get; set; }
    public virtual Project Project { get; set; } = default!;
    public virtual ICollection<ChatParticipant> ChatParticipants { get; set; } = new HashSet<ChatParticipant>();
    public virtual ICollection<ChatMessage> ChatMessages { get; set; } = new HashSet<ChatMessage>();

}
