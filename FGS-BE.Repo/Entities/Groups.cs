using System.ComponentModel.DataAnnotations;

namespace FGS_BE.Repo.Entities;

public class Groups
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int LeaderId { get; set; }
    public int MaxMembers { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<Users> Members { get; set; } = new List<Users>();
    public ICollection<Projects> Projects { get; set; } = new List<Projects>();

    public Users Leader { get; set; }
}