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

        public async Task<PaginatedList<ProjectDto>> GetDashboardPagedAsync(
            int pageIndex,
            int pageSize,
            int? semesterId = null,
            string? status = null,
            string? sortColumn = "Id",
            string? sortDir = "Asc")
        {
            var paged = await _unitOfWork.ProjectRepository.GetDashboardPagedAsync(
                pageIndex, pageSize, semesterId, status, sortColumn, sortDir);
            var list = paged.Select(x => new ProjectDto(x)).ToList();
            return new PaginatedList<ProjectDto>(
                list,
                paged.TotalItems,
                paged.PageIndex,
                paged.PageSize
            );
        }

        public async Task<int> GetTotalMembersBySemesterAsync(int semesterId)
        {
            return await _unitOfWork.ProjectRepository
                .CountUsersBySemesterAsync(semesterId);
        }
    }
}
