using System.ComponentModel.DataAnnotations.Schema;

namespace FGS_BE.Repo.Entities
{
    public class Task
    {
        public int Id { get; set; }
        public string? Label { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string? Priority { get; set; } = string.Empty;
        public int Complexity { get; set; }
        public decimal EstimatedHours { get; set; }
        public decimal Weight { get; set; } = 1.0m;
        public string? Status { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(MilestoneId))]
        public int MilestoneId { get; set; }
        public virtual Milestone Milestone { get; set; } = default!;

        [ForeignKey(nameof(AssigneeId))]
        public int? AssigneeId { get; set; }
        public virtual User? Assignee { get; set; }

        [ForeignKey(nameof(ParentTaskId))]
        public int? ParentTaskId { get; set; }
        public virtual Task? ParentTask { get; set; }

        public virtual ICollection<Task> SubTasks { get; set; } = new HashSet<Task>();
        public virtual ICollection<PerformanceScore> PerformanceScores { get; set; } = new HashSet<PerformanceScore>();
        public virtual ICollection<Submission> Submissions { get; set; } = new HashSet<Submission>();
    }
}