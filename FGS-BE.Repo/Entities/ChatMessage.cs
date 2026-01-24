using System.ComponentModel.DataAnnotations.Schema;

namespace FGS_BE.Repo.Entities;

public class ChatMessage
{
    public int Id { get; set; }
    public string? MessageType { get; set; } = string.Empty;
    public string? Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsDeleted { get; set; }

    [ForeignKey(nameof(ChatRoomId))]
    public int ChatRoomId { get; set; }
    public virtual ChatRoom ChatRoom { get; set; } = default!;

    [ForeignKey(nameof(SenderId))]
    public int SenderId { get; set; }
    public virtual User Sender { get; set; } = default!;
}