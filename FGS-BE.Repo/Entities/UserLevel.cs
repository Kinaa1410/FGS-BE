using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FGS_BE.Repo.Entities
{
    public class UserLevel
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(UserId))]
        public int UserId { get; set; }

        [ForeignKey(nameof(LevelId))]
        public int LevelId { get; set; }

        [MaxLength(95)]
        public string Reason { get; set; } = string.Empty;

        public DateTime UnlockedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual User User { get; set; } = null!;
        public virtual Level Level { get; set; } = null!;
    }
}
