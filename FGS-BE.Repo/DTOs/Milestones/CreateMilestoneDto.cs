namespace FGS_BE.Repo.DTOs.Milestones
{
    public class CreateMilestoneDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Weight { get; set; }
        public string? Status { get; set; }
        public int ProjectId { get; set; }

        public FGS_BE.Repo.Entities.Milestone ToEntity()
        {
            return new FGS_BE.Repo.Entities.Milestone
            {
                Title = this.Title,
                Description = this.Description,
                StartDate = this.StartDate,
                DueDate = this.DueDate,
                Weight = this.Weight,
                Status = this.Status ?? "Pending",
                ProjectId = this.ProjectId,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
