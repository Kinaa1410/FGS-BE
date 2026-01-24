using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.Entities;
namespace FGS_BE.Repo.DTOs.Projects
{
    public class UpdateProjectDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; } // String input, parsed to enum in service
        public decimal? TotalPoints { get; set; } // Nullable for partial updates
        // NEW: Optional mentor update
        public int? MentorId { get; set; }
        // Optional limit updates
        public int? MinMembers { get; set; }
        public int? MaxMembers { get; set; }
    }
}