namespace FGS_BE.Repo.DTOs.PerformanceScores
{
    public class CreatePerformanceScoreDto
    {
        public decimal Score { get; set; }
        public string? Comment { get; set; }

        public int MilestoneId { get; set; }
        public int UserId { get; set; }
        public int ProjectId { get; set; }
        public int TaskId { get; set; }

        public Entities.PerformanceScore ToEntity()
        {
            return new Entities.PerformanceScore
            {
                Score = Score,
                Comment = Comment,
                MilestoneId = MilestoneId,
                UserId = UserId,
                ProjectId = ProjectId,
                TaskId = TaskId,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
