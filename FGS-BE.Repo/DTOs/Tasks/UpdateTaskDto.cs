namespace FGS_BE.Repo.DTOs.Tasks
{
    public class UpdateTaskDto
    {
        public string? Label { get; set; }
        public string? Description { get; set; }
        public string? Priority { get; set; }
        public int Complexity { get; set; }
        public decimal EstimatedHours { get; set; }
        public decimal Weight { get; set; }
        public string? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public int? AssigneeId { get; set; }
        public int? ParentTaskId { get; set; }
        public int MilestoneId { get; set; }
    }
}
