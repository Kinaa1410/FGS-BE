using FGS_BE.Repo.Data;
using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FGS_BE.Repo.Repositories.Implements;

public class WalletRepository : GenericRepository<UserWallet>, IWalletRepository
{
    public WalletRepository(ApplicationDbContext context) : base(context) { }

    public async Task<UserWallet?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await Entities
            .AsNoTracking()
            .Include(w => w.User)
            .FirstOrDefaultAsync(w => w.UserId == userId, cancellationToken);
    }

    public async Task<decimal> GetBalanceByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await Entities
            .AsNoTracking()
            .Where(w => w.UserId == userId)
            .Select(w => w.Balance)
            .FirstOrDefaultAsync(cancellationToken);
    }
}