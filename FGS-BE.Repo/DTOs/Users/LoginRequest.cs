using System.ComponentModel;

namespace FGS_BE.Repo.DTOs.Users;
public sealed record LoginRequest
{

    /// <summary>
    /// The user's email address which acts as a user name.
    /// </summary>
    [DefaultValue("admin@gmail.com")]
    public string Username { get; init; } = default!;

    /// <summary>
    /// The user's password.
    /// </summary>
    [DefaultValue("admin@gmail.com")]
    public string Password { get; init; } = default!;

}