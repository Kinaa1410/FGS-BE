using FGS_BE.Repo.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace FGS_BE.Repo.Entities;

public class RewardItem
{
    public int Id { get; set; }
    public string? Name { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;

    public decimal PriceInPoints { get; set; }
    public int Quantity { get; set; }

    [Column(TypeName = "nvarchar(24)")]
    public RewardItemType Type { get; set; }
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<RedeemRequest> RedeemRequests { get; set; } = new HashSet<RedeemRequest>();

    [ForeignKey(nameof(CreatedById))]
    public int? CreatedById { get; set; }
    public virtual User? CreatedBy { get; set; }
}