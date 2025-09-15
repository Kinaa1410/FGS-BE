using System.ComponentModel.DataAnnotations;

namespace FGS_BE.Repo.Entities;

public class CashRedeemHistory
{
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; }
    public decimal Amount { get; set; }
    public string Method { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime RequestedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public string TransactionRef { get; set; } = string.Empty;

    public Users User { get; set; }
}