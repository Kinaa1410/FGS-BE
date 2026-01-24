using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FGS_BE.Repo.Enums;

namespace FGS_BE.Repo.Entities
{
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
        public bool IsResubmission { get; set; } = false;  
        public DateTime? RejectionDate { get; set; } 
        [ForeignKey(nameof(UserId))]
        public int UserId { get; set; }
        public virtual User User { get; set; } = default!;
        [ForeignKey(nameof(TaskId))]
        public int TaskId { get; set; }
        public virtual Task Task { get; set; } = default!;
    }
}