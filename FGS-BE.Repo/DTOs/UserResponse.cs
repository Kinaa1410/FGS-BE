namespace FGS_BE.Repo.DTOs;
public record UserResponse
{
    public int Id { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? PasswordHash { get; set; }
    public string? StudentCode { get; set; }
    public string? Phone { get; set; }
    public string? Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int RoleId { get; set; }
}
