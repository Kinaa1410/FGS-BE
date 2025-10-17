namespace FGS_BE.Repo.DTOs.Projects
{
    public class UpdateProjectDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public decimal TotalPoints { get; set; }
    }
}
