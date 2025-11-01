namespace FGS_BE.Repo.DTOs.Tasks
{
    public class TaskDto
    {
        public int Id { get; set; }
        public string? Label { get; set; }
        public string? Description { get; set; }
        public string? Priority { get; set; }
        public int Complexity { get; set; }
        public decimal EstimatedHours { get; set; }
        public decimal Weight { get; set; }
        public string? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public int MilestoneId { get; set; }
        public int? AssigneeId { get; set; }
        public int? ParentTaskId { get; set; }

        public TaskDto() { }

        public TaskDto(FGS_BE.Repo.Entities.Task entity)
        {
            Id = entity.Id;
            Label = entity.Label;
            Description = entity.Description;
            Priority = entity.Priority;
            Complexity = entity.Complexity;
            EstimatedHours = entity.EstimatedHours;
            Weight = entity.Weight;
            Status = entity.Status;
            StartDate = entity.StartDate;
            DueDate = entity.DueDate;
            CompletedAt = entity.CompletedAt;
            CreatedAt = entity.CreatedAt;
            MilestoneId = entity.MilestoneId;
            AssigneeId = entity.AssigneeId;
            ParentTaskId = entity.ParentTaskId;
        }
    }
}
