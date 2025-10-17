namespace FGS_BE.Repo.DTOs.Projects
{
    public class CreateProjectDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; } = "Pending";
        public decimal TotalPoints { get; set; } = 0;
        public int SemesterId { get; set; }
        public int ProposerId { get; set; }

        public Entities.Project ToEntity()
        {
            return new Entities.Project
            {
                Title = Title,
                Description = Description,
                Status = Status,
                TotalPoints = TotalPoints,
                SemesterId = SemesterId,
                ProposerId = ProposerId,
                CreatedAt = DateTime.UtcNow
            };

        }
    }
}
