using System.ComponentModel.DataAnnotations;

namespace FGS_BE.Repo.Entities;

public class Semesters
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<SemesterMembers> SemesterMembers { get; set; } = new List<SemesterMembers>();
}