using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Enums;

namespace FGS_BE.Repo.DTOs.Submissions
{
    public class SubmissionDto
    {
        public int Id { get; set; }
        public DateTime SubmittedAt { get; set; }
        public string FileUrl { get; set; } = string.Empty;
        public SubmissionStatus Status { get; set; }
        public decimal? Grade { get; set; }
        public string? Feedback { get; set; }
        public int Version { get; set; }
        public bool IsFinal { get; set; }
        public int UserId { get; set; }
        public int TaskId { get; set; }

        public SubmissionDto() { }

        public SubmissionDto(Submission entity)
        {
            Id = entity.Id;
            SubmittedAt = entity.SubmittedAt;
            FileUrl = entity.FileUrl;
            Status = entity.Status;
            Grade = entity.Grade;
            Feedback = entity.Feedback;
            Version = entity.Version;
            IsFinal = entity.IsFinal;
            UserId = entity.UserId;
            TaskId = entity.TaskId;
        }
    }
}
