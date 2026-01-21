// File: FGS_BE.Repo.DTOs.Wallets/WalletDto.cs
using FGS_BE.Repo.Entities;

namespace FGS_BE.Repo.DTOs.Wallets;

public class WalletDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty; 
    public decimal Balance { get; set; }
    public DateTime LastUpdatedAt { get; set; }

    public WalletDto(UserWallet entity)
    {
        Id = entity.Id;
        UserId = entity.UserId;
        Balance = entity.Balance;
        LastUpdatedAt = entity.LastUpdatedAt;

        // An toàn với null
        if (entity.User != null)
        {
            UserName = !string.IsNullOrEmpty(entity.User.UserName)
                ? entity.User.UserName
                : entity.User.Email ?? "Unknown User";
        }
        else
        {
            UserName = "User Not Found";
        }
    }
}