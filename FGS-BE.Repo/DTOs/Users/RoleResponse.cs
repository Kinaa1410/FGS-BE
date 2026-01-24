namespace FGS_BE.Repo.DTOs.Users;
public sealed record RoleResponse
{
    public int Id { get; set; }
    public string? Name { get; set; }
}