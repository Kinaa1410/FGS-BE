using FGS_BE.Repo.Entities;

namespace FGS_BE.Repo.DTOs.Projects
{
    public class ProjectDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public decimal TotalPoints { get; set; }
        public DateTime CreatedAt { get; set; }
        public int SemesterId { get; set; }
        public int ProposerId { get; set; }

        public ProjectDto() { }

        public ProjectDto(Project entity)
        {
            Id = entity.Id;
            Title = entity.Title;
            Description = entity.Description;
            Status = entity.Status;
            TotalPoints = entity.TotalPoints;
            CreatedAt = entity.CreatedAt;
            SemesterId = entity.SemesterId;
            ProposerId = entity.ProposerId;
        }
    }
}
