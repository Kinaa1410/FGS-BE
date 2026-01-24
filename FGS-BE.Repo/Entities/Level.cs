using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json; // For JsonSerializer if needed

namespace FGS_BE.Repo.Entities;

public class Level
{
    [Key]
    public int Id { get; set; }

    [MaxLength(100)]
    public string? Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Description { get; set; } = string.Empty;

    // Changed to string for EF compatibility; store JSON as string
    public string? ConditionJson { get; set; } // e.g., "{\"threshold\": 100, \"type\": \"points\"}"

    [NotMapped]
    public JsonDocument? Condition => !string.IsNullOrEmpty(ConditionJson)
        ? JsonDocument.Parse(ConditionJson)
        : null;

    public int PointsReward { get; set; }

    [MaxLength(255)]
    public string? Icon { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual ICollection<UserLevel> UserLevels { get; set; } = new HashSet<UserLevel>();
}