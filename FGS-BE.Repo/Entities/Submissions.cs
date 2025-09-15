using System.ComponentModel.DataAnnotations;

namespace FGS_BE.Repo.Entities;

public class Submissions
{
    [Key]
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public int UserId { get; set; }
    public string FileUrl { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
    public string Status { get; set; } = string.Empty;

    public Projects Project { get; set; }
    public Users User { get; set; }
}