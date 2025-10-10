namespace FGS_BE.Repo.Entities;
public class UserWallet
{
    public int Id { get; set; }
    public decimal Balance { get; set; }

    public DateTime LastUpdatedAt { get; set; }

    public int UserId { get; set; }
    public virtual User User { get; set; } = default!;

    public virtual ICollection<PointTransaction> PointTransactions { get; set; } = new HashSet<PointTransaction>();
}
