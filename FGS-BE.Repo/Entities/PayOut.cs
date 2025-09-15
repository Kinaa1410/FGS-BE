using System.ComponentModel.DataAnnotations;

namespace FGS_BE.Repo.Entities;

public class Payout
{
    [Key]
    public int Id { get; set; }
    public int CashId { get; set; }
    public string Method { get; set; } = string.Empty;
    public string BankAccount { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime PaidAt { get; set; }
    public string TransferContent { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}