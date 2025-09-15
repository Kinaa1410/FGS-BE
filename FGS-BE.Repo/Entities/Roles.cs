using System.ComponentModel.DataAnnotations;

namespace FGS_BE.Repo.Entities;

public class Roles
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ICollection<Users> Users { get; set; } = new List<Users>();
}