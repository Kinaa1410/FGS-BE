using System.ComponentModel.DataAnnotations.Schema;

namespace FGS_BE.Repo.Entities;

public class TermKeyword
{
    public int Id { get; set; }

    public string? Keyword { get; set; } = string.Empty;
    public int BasePoints { get; set; } = 20;
    public int RuleBonus { get; set; } = 10;

    [ForeignKey(nameof(SemesterId))]
    public int SemesterId { get; set; }
    public virtual Semester Semester { get; set; } = default!;

    public virtual ICollection<ProjectKeyword> ProjectKeywords { get; set; } = new HashSet<ProjectKeyword>();
}