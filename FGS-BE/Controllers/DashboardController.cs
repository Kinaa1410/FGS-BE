using FGS_BE.Service.Implements;
using FGS_BE.Service.Interfaces;
using FGS_BE.Service.Services;
using FGS_BE.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FGS_BE.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _service;
        public DashboardController(IDashboardService dashboardService)
        {
            _service = dashboardService;
        }


        [HttpGet("dashboard/count")]
        public async Task<IActionResult> GetDashboardCount(
    [FromQuery] int semesterId)
        {
            var result = await _service.GetDashboardCountAsync(semesterId);
            return Ok(result);
        }


        [HttpGet("{semesterId:int}/users/total")]
        public async Task<IActionResult> GetTotalProjectMembers(int semesterId)
        {
            var totalMembers = await _service
                .GetTotalMembersBySemesterAsync(semesterId);

            return Ok(new
            {
                SemesterId = semesterId,
                TotalMembers = totalMembers
            });
        }
    }
}
