using System.Text.Json.Serialization;

namespace FGS_BE.Repo.DTOs.Users;
public sealed record UpdateUserCommand
{
    public string? PhoneNumber { get; set; }
    public string? FullName { get; set; }
    public string? StudentCode { get; set; }
    public string? Status { get; set; }

    [JsonIgnore]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
