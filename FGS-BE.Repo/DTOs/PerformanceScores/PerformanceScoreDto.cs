namespace FGS_BE.Repo.DTOs.PerformanceScores
{
    public class PerformanceScoreDto
    {
        public int Id { get; set; }
        public decimal Score { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }

        public int MilestoneId { get; set; }
        public int UserId { get; set; }
        public int ProjectId { get; set; }
        public int TaskId { get; set; }

        public PerformanceScoreDto() { }

        public PerformanceScoreDto(FGS_BE.Repo.Entities.PerformanceScore entity)
        {
            Id = entity.Id;
            Score = entity.Score;
            Comment = entity.Comment;
            CreatedAt = entity.CreatedAt;

            MilestoneId = entity.MilestoneId;
            UserId = entity.UserId;
            ProjectId = entity.ProjectId;
            TaskId = entity.TaskId;
        }
    }
}
