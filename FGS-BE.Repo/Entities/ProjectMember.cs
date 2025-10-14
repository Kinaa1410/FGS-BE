using System.ComponentModel.DataAnnotations.Schema;

namespace FGS_BE.Repo.Entities;

public class ProjectMember
{
    public int Id { get; set; }

    public string? Role { get; set; } = string.Empty;
    public DateTime JoinAt { get; set; }

    [ForeignKey(nameof(UserId))]
    public int UserId { get; set; }
    public virtual User User { get; set; } = default!;

    [ForeignKey(nameof(ProjectId))]
    public int ProjectId { get; set; }
    public virtual Project Project { get; set; } = default!;
}