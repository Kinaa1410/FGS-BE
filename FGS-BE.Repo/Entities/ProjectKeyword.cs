using FGS_BE.Repo.Entities; 

namespace FGS_BE.Repo.Entities;

public class ProjectKeyword
{
    public int Id { get; set; }
    public int RulesMetPoints { get; set; }

    public int ProjectId { get; set; }
    public virtual Project Project { get; set; } = default!;

    public int TermKeywordId { get; set; }
    public virtual TermKeyword TermKeyword { get; set; } = default!;
}