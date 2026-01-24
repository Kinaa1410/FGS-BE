namespace FGS_BE.Repo.DTOs.Users;
public record UserResponse
{
    public int Id { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? FullName { get; set; }
    public string? StudentCode { get; set; }
    public string? Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<RoleResponse> Roles { get; set; } = new HashSet<RoleResponse>();
}
