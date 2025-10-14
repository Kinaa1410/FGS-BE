using System.ComponentModel.DataAnnotations.Schema;

namespace FGS_BE.Repo.Entities;

public class ProjectKeyword
{
    public int Id { get; set; }
    public int RulesMetPoints { get; set; }

    [ForeignKey(nameof(ProjectId))]
    public int ProjectId { get; set; }
    public virtual Project Project { get; set; } = default!;

    [ForeignKey(nameof(TermKeywordId))]
    public int TermKeywordId { get; set; }
    public virtual TermKeyword TermKeyword { get; set; } = default!;
}