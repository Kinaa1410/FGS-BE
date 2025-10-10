namespace FGS_BE.Repo.Entities;
public class ChatParticipant
{
    public int Id { get; set; }
    public DateTime JoinedAt { get; set; }
    public bool IsMuted { get; set; }
    public int? LastReadMessageId { get; set; }
    public int UserId { get; set; }
    public virtual User User { get; set; } = default!;
    public int ChatRoomId { get; set; }
    public virtual ChatRoom ChatRoom { get; set; } = default!;

}
