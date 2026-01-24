namespace FGS_BE.Repo.DTOs.Submissions
{
    public class ReviewSubmissionDto
    {
        public string Decision { get; set; } = default!;
        public string? Feedback { get; set; }
        public decimal? Score { get; set; }
        public bool? ExtendDeadline { get; set; }
    }
}
