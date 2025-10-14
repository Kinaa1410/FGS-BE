using System.ComponentModel.DataAnnotations;

namespace FGS_BE.Repo.DTOs.Users;
public sealed class RegisterRequest
{
    /// <summary>
    /// The user's email address which acts as a user name.
    /// </summary>
    [EmailAddress]
    public string Username { get; init; } = default!;

    /// <summary>
    /// The user's password.
    /// </summary>
    public required string Password { get; init; }
}
