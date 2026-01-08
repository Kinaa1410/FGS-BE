using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FGS_BE.Repo.DTOs.Users
{
    public class RegisterDTO
    {
        public string Username { get; init; } = default!;
        public string Email { get; set; }

        public required string Password { get; init; }
        [MaxLength(100)] // Optional: Limit length
        public string? FullName { get; set; }
        [MaxLength(50)] // Optional: Limit length
        public string? StudentCode { get; set; }
    }
}
