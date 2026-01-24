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
        Task<PaginatedList<ProjectDto>> GetDashboardPagedAsync(
            int pageIndex,
            int pageSize,
            int? semesterId = null,
            string? status = null,
            string? sortColumn = "Id",
            string? sortDir = "Asc");

        Task<int> GetTotalMembersBySemesterAsync(int semesterId);
    }
}
