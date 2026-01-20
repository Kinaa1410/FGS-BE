using FGS_BE.Repo.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FGS_BE.Repo.DTOs.Projects
{
    public class CompleteProjectResultDto
    {
        public int ProjectId { get; set; }
        public ProjectStatus Status { get; set; }
    }
}
