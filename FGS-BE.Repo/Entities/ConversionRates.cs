using System.ComponentModel.DataAnnotations;

namespace FGS_BE.Repo.Entities;

public class ConversionRates
{
    [Key]
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public decimal Requested { get; set; }
    public decimal Unit { get; set; }
    public bool Active { get; set; }
    public DateTime CreatedAt { get; set; }
}