using FGS_BE.Repo.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace FGS_BE.Repo.Entities;
public class RedeemRequest
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPoints { get; set; }

    [Column(TypeName = "nvarchar(24)")]
    public RedeemRequestStatus Status { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime ProcessedAt { get; set; }

    public int UserId { get; set; }
    public virtual User User { get; set; } = default!;
    public int RewardItemId { get; set; }
    public virtual RewardItem RewardItem { get; set; } = default!;

}
