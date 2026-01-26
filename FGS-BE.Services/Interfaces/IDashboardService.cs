using FGS_BE.Repo.DTOs.Dashboard;
using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.DTOs.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FGS_BE.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<ProjectDashboardCountDto> GetDashboardCountAsync(int semesterId);

        Task<int> GetTotalMembersBySemesterAsync(int semesterId);
    }
}
