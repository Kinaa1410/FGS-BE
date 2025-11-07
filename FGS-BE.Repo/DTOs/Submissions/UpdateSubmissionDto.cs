using FGS_BE.Repo.Enums;

namespace FGS_BE.Repo.DTOs.Submissions
{
    public class UpdateSubmissionDto
    {
        public string? FileUrl { get; set; }
        public SubmissionStatus? Status { get; set; }
        public decimal? Grade { get; set; }
        public string? Feedback { get; set; }
        public bool? IsFinal { get; set; }
    }
}
