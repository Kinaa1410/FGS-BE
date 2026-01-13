using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FGS_BE.Repo.DTOs.PerformanceScores
{
    public class MilestoneScoreDto
    {
        public int MilestoneId { get; set; }
        public decimal MilestoneWeight { get; set; }   // ví dụ 0.4
        public decimal MilestoneMaxScore { get; set; } // ví dụ 40
        public decimal EarnedScore { get; set; }       // user ăn được bao nhiêu
    }
}
