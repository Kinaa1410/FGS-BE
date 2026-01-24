using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.Entities;

namespace FGS_BE.Repo.Repositories.Interfaces;

public interface IWalletRepository : IGenericRepository<UserWallet>
{
    Task<UserWallet?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<decimal> GetBalanceByUserIdAsync(int userId, CancellationToken cancellationToken = default);
}