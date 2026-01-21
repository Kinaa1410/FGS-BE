using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.DTOs.Wallets;

namespace FGS_BE.Service.Interfaces;

public interface IWalletService
{
    Task<PaginatedList<WalletDto>> GetPagedAsync(int pageIndex, int pageSize, string? keyword = null);
    Task<WalletDto?> GetByIdAsync(int id);
    Task<WalletDto?> GetByUserIdAsync(int userId);
    Task<decimal> GetBalanceByUserIdAsync(int userId);
    Task<WalletDto> CreateOrUpdateAsync(int userId, decimal pointsToAdd);
    Task<bool> DeleteAsync(int id);
}