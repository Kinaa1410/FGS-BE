using System.ComponentModel.DataAnnotations;

namespace FGS_BE.Repo.Entities;

public class ItemsRedeemHistory
{
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ItemId { get; set; }
    public decimal PointsSpent { get; set; }
    public decimal Quantity { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime RequestedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public string DeliveryInfo { get; set; } = string.Empty;

    public Users User { get; set; }
    public Items Item { get; set; }
}