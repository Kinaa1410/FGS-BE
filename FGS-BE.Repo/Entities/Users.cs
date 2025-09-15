using System.ComponentModel.DataAnnotations;

namespace FGS_BE.Repo.Entities;

public class Users
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string StudentCode { get; set; } = string.Empty;

    [Phone]
    [StringLength(20)]
    public string Phone { get; set; } = string.Empty;

    [Required]
    public int RoleId { get; set; }

    [Required]
    public int LevelId { get; set; }

    [Required]
    public DateTime JoinedAt { get; set; }

    public ICollection<SemesterMembers> SemesterMembers { get; set; } = new List<SemesterMembers>();
    public ICollection<ItemsRedeemHistory> ItemsRedeemHistory { get; set; } = new List<ItemsRedeemHistory>();
    public ICollection<CashRedeemHistory> CashRedeemHistory { get; set; } = new List<CashRedeemHistory>();
    public ICollection<Groups> Groups { get; set; } = new List<Groups>();
    public ICollection<Projects> Projects { get; set; } = new List<Projects>();
    public ICollection<Tasks> Tasks { get; set; } = new List<Tasks>();
    public ICollection<Submissions> Submissions { get; set; } = new List<Submissions>();
    public ICollection<TaskAssignment> TaskAssignments { get; set; } = new List<TaskAssignment>();

    public Roles Role { get; set; }
    public Levels Level { get; set; }
}