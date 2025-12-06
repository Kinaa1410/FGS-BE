using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Enums;

namespace FGS_BE.Repo.DTOs.Projects
{
    public class ProjectDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "Open"; // String output
        public decimal TotalPoints { get; set; }
        public DateTime CreatedAt { get; set; }
        public int SemesterId { get; set; }
        public int ProposerId { get; set; }

        // New fields
        public int MinMembers { get; set; }
        public int MaxMembers { get; set; }
        public int CurrentMembers { get; set; }

        public ProjectDto() { }

        public ProjectDto(Project entity)
        {
            Id = entity.Id;
            Title = entity.Title ?? string.Empty;
            Description = entity.Description ?? string.Empty;
            Status = entity.Status.ToString(); // Enum to string (no ?? needed)
            TotalPoints = entity.TotalPoints;
            CreatedAt = entity.CreatedAt;
            SemesterId = entity.SemesterId;
            ProposerId = entity.ProposerId;
            MinMembers = entity.MinMembers;
            MaxMembers = entity.MaxMembers;
            CurrentMembers = entity.CurrentMembers;
        }
    }
}