using FGS_BE.Repo.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace FGS_BE.Repo.Entities;
public class PointTransaction
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Note { get; set; } = string.Empty;
    public decimal Amount { get; set; }

    [Column(TypeName = "nvarchar(24)")]
    public PointTransactionType Type { get; set; }

    [Column(TypeName = "nvarchar(24)")]
    public PointTransactionSourceType SourceType { get; set; }

    public int UserWalletId { get; set; }
    public virtual UserWallet UserWallet { get; set; } = default!;

}
