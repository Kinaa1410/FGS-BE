using System.ComponentModel.DataAnnotations;

namespace FGS_BE.Repo.Entities;

public class Items
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Stock { get; set; }
    public string Type { get; set; } = string.Empty;
    public bool Active { get; set; }
    public DateTime CreatedAt { get; set; }
}