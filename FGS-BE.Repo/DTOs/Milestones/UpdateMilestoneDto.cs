using FGS_BE.Repo.Entities;

namespace FGS_BE.Repo.DTOs.Milestones
{
    public class UpdateMilestoneDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal? Weight { get; set; }  
        public string? Status { get; set; }
        public bool? IsDelayed { get; set; }
        public DateTime? OriginalDueDate { get; set; }
    }
}