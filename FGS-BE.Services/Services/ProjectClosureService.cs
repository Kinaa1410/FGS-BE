using FGS_BE.Repo.Enums;
using FGS_BE.Repo.Repositories.Interfaces;
using FGS_BE.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FGS_BE.Service.Implements
{
    public class ProjectClosureService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISemesterService _semesterService;
        private readonly ILogger<ProjectClosureService> _logger;

        public ProjectClosureService(
            IUnitOfWork unitOfWork,
            ISemesterService semesterService,
            ILogger<ProjectClosureService> logger)
        {
            _unitOfWork = unitOfWork;
            _semesterService = semesterService;
            _logger = logger;
        }

        public async Task AutoCloseProjectsAsync()
        {
            var now = DateTime.UtcNow;
            _logger.LogInformation("Starting auto-close job at {Now}", now);

            var potentialClosedSemesters = await _unitOfWork.SemesterRepository.GetByAsync(
                s => s.EndDate < now && s.Status != "Closed");

            if (!potentialClosedSemesters.Any())
            {
                _logger.LogInformation("No semesters to close.");
                return;
            }

            int closedProjectsCount = 0;
            int closedSemestersCount = 0;

            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                foreach (var semester in potentialClosedSemesters)
                {
                    await _semesterService.SyncSemesterStatusAsync(semester.Id);
                    closedSemestersCount++;

                    var projectsToClose = await _unitOfWork.ProjectRepository.GetByAsync(
                        p => p.SemesterId == semester.Id &&
                             (p.Status == ProjectStatus.InProcess || p.Status == ProjectStatus.Open),
                        q => q.Include(p => p.Semester));

                    foreach (var project in projectsToClose)
                    {
                        project.Status = ProjectStatus.Close;
                        await _unitOfWork.ProjectRepository.UpdateAsync(project);
                        closedProjectsCount++;

                        _logger.LogInformation(
                            "Auto-closed project {ProjectId} (Title: {Title}, Previous Status: {PreviousStatus}) due to semester {SemesterId} end.",
                            project.Id, project.Title, project.Status, semester.Id);
                    }
                }

                await _unitOfWork.CommitAsync();
                await transaction.CommitAsync();

                _logger.LogInformation(
                    "Auto-close job completed: Closed {ClosedProjectsCount} projects from {ClosedSemestersCount} semesters.",
                    closedProjectsCount, closedSemestersCount);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Failed to auto-close projects.");
                throw;
            }
        }
    }
}