using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FGS_BE.Repo.DTOs.Dashboard
{
    public class ProjectDashboardCountDto
    {
        public int Total { get; set; }
        public int Open { get; set; }
        public int InProcess { get; set; }
        public int Close { get; set; }
        public int Complete { get; set; }
    }

}
