using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Enums;

namespace FGS_BE.Repo.DTOs.Projects
{
    public class CreateProjectDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal TotalPoints { get; set; } = 0;
        public int SemesterId { get; set; }
        public int ProposerId { get; set; }

        // Limits with defaults
        public int MinMembers { get; set; } = 2;
        public int MaxMembers { get; set; } = 10;

        public Project ToEntity()
        {
            if (MinMembers > MaxMembers)
                throw new ArgumentException("MinMembers cannot exceed MaxMembers.");

            return new Project
            {
                Title = Title ?? string.Empty,
                Description = Description ?? string.Empty,
                Status = ProjectStatus.Open, // Default enum (no string input needed)
                TotalPoints = TotalPoints,
                SemesterId = SemesterId,
                ProposerId = ProposerId,
                MinMembers = MinMembers,
                MaxMembers = MaxMembers,
                CurrentMembers = 0, // Auto-incremented after join
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}