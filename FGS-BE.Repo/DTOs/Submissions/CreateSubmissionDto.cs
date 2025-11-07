using FGS_BE.Repo.Entities;

namespace FGS_BE.Repo.DTOs.Submissions
{
    public class CreateSubmissionDto
    {
        public string FileUrl { get; set; } = string.Empty;
        public int UserId { get; set; }
        public int TaskId { get; set; }

        public Submission ToEntity()
        {
            return new Submission
            {
                FileUrl = FileUrl,
                UserId = UserId,
                TaskId = TaskId,
                SubmittedAt = DateTime.UtcNow,
                Status = FGS_BE.Repo.Enums.SubmissionStatus.Pending,
                Version = 1,
                IsFinal = false
            };
        }
    }
}
