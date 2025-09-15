using System.ComponentModel.DataAnnotations;

namespace FGS_BE.Repo.Entities;

public class SemesterMembers
{
    [Key]
    public int Id { get; set; }
    public int SemesterId { get; set; }
    public int UserId { get; set; }
    public DateTime JoinedAt { get; set; }
    public string Status { get; set; } = string.Empty;

    public Semesters Semester { get; set; }
    public Users User { get; set; }
}