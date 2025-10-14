using System.ComponentModel.DataAnnotations.Schema;

namespace FGS_BE.Repo.Entities;

public class ChatParticipant
{
    public int Id { get; set; }
    public DateTime JoinedAt { get; set; }
    public bool IsMuted { get; set; }
    public int? LastReadMessageId { get; set; }

    [ForeignKey(nameof(UserId))]
    public int UserId { get; set; }
    public virtual User User { get; set; } = default!;

    [ForeignKey(nameof(ChatRoomId))]
    public int ChatRoomId { get; set; }
    public virtual ChatRoom ChatRoom { get; set; } = default!;
}