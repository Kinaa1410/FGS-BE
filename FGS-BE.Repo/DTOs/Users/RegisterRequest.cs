using System.ComponentModel.DataAnnotations;

namespace FGS_BE.Repo.DTOs.Users;
public sealed class RegisterRequest
{

    public string Username { get; init; } = default!;

    public required string Password { get; init; }
    [MaxLength(100)] // Optional: Limit length
    public string? FullName { get; set; }
    [MaxLength(50)] // Optional: Limit length
    public string? StudentCode { get; set; }
}
