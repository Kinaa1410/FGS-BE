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


        /// <summary>
        /// 1. nhập semesterID để show list project của kỳ đó
        /// 2. nhập status để show list project của status đó (Open, InProcess, Close, Complete); nếu cần list prj của status theo semester chỉ định thì nhập cả 2
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="semesterId"></param>
        /// <param name="status"></param>
        /// <param name="sortColumn"></param>
        /// <param name="sortDir"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? semesterId = null,
            [FromQuery] string? status = null,
            [FromQuery] string? sortColumn = "Id",
            [FromQuery] string? sortDir = "Asc")
        {
            try
            {
                var result = await _service.GetDashboardPagedAsync(pageIndex, pageSize, semesterId, status, sortColumn, sortDir);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An unexpected error occurred while retrieving projects." });
            }
        }


        /// <summary>
        /// tinh tổng số user tham gia vào semester
        /// </summary>
        /// <param name="semesterId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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
