using System.ComponentModel.DataAnnotations;

namespace FGS_BE.Repo.DTOs.Levels;

public class CheckLevelUpRequest
{
    [Required, Range(0, int.MaxValue)]
    public int CurrentPoints { get; set; }
}