using FGS_BE.Repo.Entities;

namespace FGS_BE.Repo.DTOs.Milestones
{
    public class MilestoneDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Weight { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ProjectId { get; set; }
        public bool IsDelayed { get; set; }
        public DateTime? OriginalDueDate { get; set; }

        public MilestoneDto() { }

        public MilestoneDto(FGS_BE.Repo.Entities.Milestone entity)
        {
            Id = entity.Id;
            Title = entity.Title;
            Description = entity.Description;
            StartDate = entity.StartDate;
            DueDate = entity.DueDate;
            Weight = entity.Weight;
            Status = entity.Status;
            CreatedAt = entity.CreatedAt;
            ProjectId = entity.ProjectId;
            IsDelayed = entity.IsDelayed;  
            OriginalDueDate = entity.OriginalDueDate; 
        }
    }
}