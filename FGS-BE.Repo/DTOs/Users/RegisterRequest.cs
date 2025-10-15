namespace FGS_BE.Repo.DTOs.Users;
public sealed class RegisterRequest
{

    public string Username { get; init; } = default!;

    public required string Password { get; init; }
}
