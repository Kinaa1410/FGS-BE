using System.ComponentModel.DataAnnotations;


namespace FGS_BE.Repo.DTOs.Users;
public sealed class RegisterStaffRequest
{

        public string Username { get; init; } = default!;

        public required string Password { get; init; }
        [MaxLength(100)] 
        public string? FullName { get; set; }
}
