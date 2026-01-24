using System.ComponentModel;

namespace FGS_BE.Repo.DTOs.Users;
public sealed record LoginRequest
{

    [DefaultValue("admin@gmail.com")]
    public string Username { get; init; } = default!;

    [DefaultValue("admin@gmail.com")]
    public string Password { get; init; } = default!;

}