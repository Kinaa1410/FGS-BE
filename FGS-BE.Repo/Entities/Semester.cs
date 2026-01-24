using System.ComponentModel.DataAnnotations.Schema;

namespace FGS_BE.Repo.Entities;

public class Semester
{
    public int Id { get; set; }
    public string? Name { get; set; } = string.Empty;
    public string? KeywordTheme { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public virtual ICollection<TermKeyword> TermKeywords { get; set; } = new HashSet<TermKeyword>();
    public virtual ICollection<Project> Projects { get; set; } = new HashSet<Project>();
    public virtual ICollection<SemesterMember> SemesterMembers { get; set; } = new HashSet<SemesterMember>();
}