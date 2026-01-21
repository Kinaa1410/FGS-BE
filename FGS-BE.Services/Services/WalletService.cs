using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.DTOs.Wallets;
using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Repositories.Interfaces;
using FGS_BE.Service.Interfaces;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace FGS_BE.Service.Implements;

public class WalletService : IWalletService
{
    private readonly IUnitOfWork _unitOfWork;

    public WalletService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<WalletDto?> GetByIdAsync(int id)
    {
        var repo = _unitOfWork.Repository<UserWallet>();
        var entity = await repo.FindByIdAsync(id);
        return entity?.Adapt<WalletDto>();
    }

    public async Task<WalletDto?> GetByUserIdAsync(int userId)
    {
        if (userId <= 0) return null;

        var entity = await _unitOfWork.WalletRepository.GetByUserIdAsync(userId);

        return entity == null ? null : new WalletDto(entity);
    }

    public async Task<decimal> GetBalanceByUserIdAsync(int userId)
    {
        if (userId <= 0) return 0;
        return await _unitOfWork.WalletRepository.GetBalanceByUserIdAsync(userId);
    }

    public async Task<PaginatedList<WalletDto>> GetPagedAsync(
    int pageIndex,
    int pageSize,
    string? keyword = null)
    {
        var baseQuery = _unitOfWork.Repository<UserWallet>().Entities.AsNoTracking();
        var query = baseQuery
            .Include(w => w.User) 
            .Where(w => string.IsNullOrWhiteSpace(keyword) ||
                        (w.User != null &&
                         ((w.User.UserName != null && w.User.UserName.Contains(keyword)) ||
                           (w.User.Email != null && w.User.Email.Contains(keyword)))))
            .OrderByDescending(w => w.LastUpdatedAt);

        var total = await query.CountAsync();

        var items = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtoList = items.Select(w => new WalletDto(w)).ToList();

        return new PaginatedList<WalletDto>(dtoList, total, pageIndex, pageSize);
    }

    public async Task<WalletDto> CreateOrUpdateAsync(int userId, decimal pointsToAdd)
    {
        if (pointsToAdd <= 0) throw new ArgumentException("Points to add must be greater than 0");

        var repo = _unitOfWork.Repository<UserWallet>();
        var wallet = await repo.Entities.FirstOrDefaultAsync(w => w.UserId == userId);

        if (wallet == null)
        {
            wallet = new UserWallet
            {
                UserId = userId,
                Balance = pointsToAdd,
                LastUpdatedAt = DateTime.UtcNow
            };
            await repo.CreateAsync(wallet);
        }
        else
        {
            wallet.Balance += pointsToAdd;
            wallet.LastUpdatedAt = DateTime.UtcNow;
            await repo.UpdateAsync(wallet);
        }

        await _unitOfWork.CommitAsync();
        return wallet.Adapt<WalletDto>();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var repo = _unitOfWork.Repository<UserWallet>();
        var entity = await repo.FindByIdAsync(id);
        if (entity == null) return false;

        await repo.DeleteAsync(entity);
        await _unitOfWork.CommitAsync();
        return true;
    }
}