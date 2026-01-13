using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FGS_BE.Repo.DTOs.PerformanceScores
{
    public class UserProjectScoreDto
    {
        public int ProjectId { get; set; }
        public int UserId { get; set; }

        public decimal TotalScore { get; set; }   // 0–100
        public decimal MaxScore { get; set; } = 100;

        public List<MilestoneScoreDto> Milestones { get; set; } = new();
    }
}
