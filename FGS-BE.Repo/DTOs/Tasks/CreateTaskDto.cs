namespace FGS_BE.Repo.DTOs.Tasks
{
    public class CreateTaskDto
    {
        public string? Label { get; set; }
        public string? Description { get; set; }
        public string? Priority { get; set; }
        public int Complexity { get; set; }
        public decimal EstimatedHours { get; set; }
        public decimal Weight { get; set; } = 1.0m;
        public string? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public int MilestoneId { get; set; }
        public int? AssigneeId { get; set; }
        public int? ParentTaskId { get; set; }

        public FGS_BE.Repo.Entities.Task ToEntity() => new()
        {
            Label = Label,
            Description = Description,
            Priority = Priority,
            Complexity = Complexity,
            EstimatedHours = EstimatedHours,
            Weight = Weight,
            Status = Status,
            StartDate = StartDate,
            DueDate = DueDate,
            MilestoneId = MilestoneId,
            AssigneeId = AssigneeId,
            ParentTaskId = ParentTaskId,
            CreatedAt = DateTime.UtcNow
        };
    }
}
