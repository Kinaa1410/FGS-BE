using System.ComponentModel.DataAnnotations.Schema;

namespace FGS_BE.Repo.Entities
{
    public class UserProjectStats
    {
        public int Id { get; set; }
        [ForeignKey(nameof(UserId))]
        public int UserId { get; set; }
        public virtual User User { get; set; } = default!;
        [ForeignKey(nameof(ProjectId))]
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; } = default!;
        public int FailureCount { get; set; } = 0;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}