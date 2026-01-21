using FGS_BE.Repo.Enums;
using Microsoft.AspNetCore.Http;

public class UpdateSubmissionDto
{
    public IFormFile? File { get; set; }         
    public SubmissionStatus? Status { get; set; }
    public decimal? Grade { get; set; }
    public string? Feedback { get; set; }
    public bool? IsFinal { get; set; }
}