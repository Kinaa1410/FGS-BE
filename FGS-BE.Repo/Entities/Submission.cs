using FGS_BE.Repo.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace FGS_BE.Repo.Entities;

public class Submission
{

    public int Id { get; set; }

    public DateTime SubmittedAt { get; set; }
    public string FileUrl { get; set; } = string.Empty;

    [Column(TypeName = "nvarchar(24)")]
    public SubmissionStatus Status { get; set; }
    public decimal? Grade { get; set; }
    public string? Feedback { get; set; }
    public int Version { get; set; }
    public bool IsFinal { get; set; }

    public int UserId { get; set; }
    public virtual User User { get; set; } = default!;

    public int TaskId { get; set; }
    public virtual Task Task { get; set; } = default!;

}