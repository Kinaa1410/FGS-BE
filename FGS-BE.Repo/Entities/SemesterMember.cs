using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FGS_BE.Repo.Entities;

public class SemesterMember
{
    [ForeignKey(nameof(Semester))]
    public int SemesterId { get; set; }

    [ForeignKey(nameof(User))]
    public int UserId { get; set; }

    public virtual Semester Semester { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}