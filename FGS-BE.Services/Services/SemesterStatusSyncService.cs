using FGS_BE.Repo.Repositories.Interfaces;
using FGS_BE.Service.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace FGS_BE.Service.Implements
{
    public class SemesterStatusSyncService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISemesterService _semesterService;
        private readonly ILogger<SemesterStatusSyncService> _logger;

        public SemesterStatusSyncService(
            IUnitOfWork unitOfWork,
            ISemesterService semesterService,
            ILogger<SemesterStatusSyncService> logger)
        {
            _unitOfWork = unitOfWork;
            _semesterService = semesterService;
            _logger = logger;
        }

        public async Task SyncAllSemesterStatusesAsync()
        {
            var allSemesters = await _unitOfWork.SemesterRepository.GetByAsync(s => true);  // All semesters

            int syncedCount = 0;
            foreach (var semester in allSemesters)
            {
                await _semesterService.SyncSemesterStatusAsync(semester.Id);
                syncedCount++;
            }

            _logger.LogInformation("Synced status for {SyncedCount} semesters.", syncedCount);
        }
    }
}