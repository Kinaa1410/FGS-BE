using FGS_BE.Repo.DTOs.Dashboard;
using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.DTOs.Projects;
using FGS_BE.Repo.Repositories.Interfaces;
using FGS_BE.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FGS_BE.Services.Implements
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ProjectDashboardCountDto> GetDashboardCountAsync(int semesterId)
        {
            return await _unitOfWork.ProjectRepository
                .GetDashboardCountAsync(semesterId);
        }


        public async Task<int> GetTotalMembersBySemesterAsync(int semesterId)
        {
            return await _unitOfWork.ProjectRepository
                .CountUsersBySemesterAsync(semesterId);
        }
    }
}
