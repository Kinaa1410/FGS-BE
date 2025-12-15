using FGS_BE.Repo.DTOs.Projects;
using FGS_BE.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims; // For validation if needed
namespace FGS_BE.API.Controllers
{
    [ApiController]
    [Route("api/projects")]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;
        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }
        /// <summary>
        /// get-all danh sách project
        /// </summary>
        /// <param name="pageIndex">Số trang hiện tại</param>
        /// <param name="pageSize">Số lượng bản ghi mỗi trang</param>
        /// <param name="keyword">Search theo Title</param>
        /// <param name="status">Search theo Status</param>
        /// <param name="sortColumn">Tên cột cần sắp xếp</param>
        /// <param name="sortDir">Chiều sắp Asc (tăng), Desc (giảm)</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? keyword = null,
            [FromQuery] string? status = null,
            [FromQuery] string? sortColumn = "Id",
            [FromQuery] string? sortDir = "Asc")
        {
            try
            {
                var result = await _projectService.GetPagedAsync(
                    pageIndex,
                    pageSize,
                    keyword,
                    status,
                    sortColumn,
                    sortDir);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log the full exception if you have logging (e.g., ILogger)
                return BadRequest(new { message = "An unexpected error occurred while retrieving projects." });
            }
        }
        /// <summary>
        /// get-by-id project
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _projectService.GetByIdAsync(id);
                return result == null ? NotFound(new { message = "Project not found." }) : Ok(result);
            }
            catch (Exception ex)
            {
                // Log the full exception if you have logging
                return BadRequest(new { message = "An unexpected error occurred while retrieving the project." });
            }
        }
        /// <summary>
        /// add project
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(CreateProjectDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _projectService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log if possible
                return BadRequest(new { message = "An unexpected error occurred while creating the project." });
            }
        }
        /// <summary>
        /// update project
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateProjectDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _projectService.UpdateAsync(id, dto);
                return result == null ? NotFound(new { message = "Project not found." }) : Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log the full exception if you have logging
                return BadRequest(new { message = "An unexpected error occurred while updating the project." });
            }
        }
        /// <summary>
        /// delete project
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _projectService.DeleteAsync(id);
                return success ? NoContent() : NotFound(new { message = "Project not found." });
            }
            catch (Exception ex)
            {
                // Log the full exception if you have logging
                return BadRequest(new { message = "An unexpected error occurred while deleting the project." });
            }
        }

        // ============================================================
        // GET PAGED BY MENTOR ID
        // ============================================================
        /// <summary>
        /// Get paged list of projects assigned to a specific mentor.
        /// </summary>
        /// <param name="mentorId">The ID of the mentor.</param>
        /// <param name="pageIndex">Current page number.</param>
        /// <param name="pageSize">Number of records per page.</param>
        /// <param name="keyword">Search by Title or Description.</param>
        /// <param name="status">Filter by Status.</param>
        /// <param name="sortColumn">Column to sort by.</param>
        /// <param name="sortDir">Sort direction (Asc/Desc).</param>
        /// <returns>Paginated list of projects.</returns>
        [HttpGet("mentor/{mentorId}")]
        public async Task<IActionResult> GetByMentorId(
            int mentorId,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? keyword = null,
            [FromQuery] string? status = null,
            [FromQuery] string? sortColumn = "Id",
            [FromQuery] string? sortDir = "Asc")
        {
            try
            {
                var result = await _projectService.GetByMentorIdPagedAsync(
                    mentorId,
                    pageIndex,
                    pageSize,
                    keyword,
                    status,
                    sortColumn,
                    sortDir);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log the full exception if you have logging (e.g., ILogger)
                return BadRequest(new { message = "An unexpected error occurred while retrieving mentor's projects." });
            }
        }

        // ============================================================
        // GET PAGED BY MEMBER ID
        // ============================================================
        /// <summary>
        /// Get paged list of projects where a specific user is a member.
        /// </summary>
        /// <param name="memberId">The ID of the member/user.</param>
        /// <param name="pageIndex">Current page number.</param>
        /// <param name="pageSize">Number of records per page.</param>
        /// <param name="keyword">Search by Title or Description.</param>
        /// <param name="status">Filter by Status.</param>
        /// <param name="sortColumn">Column to sort by.</param>
        /// <param name="sortDir">Sort direction (Asc/Desc).</param>
        /// <returns>Paginated list of projects.</returns>
        [HttpGet("member/{memberId}")]
        public async Task<IActionResult> GetByMemberId(
            int memberId,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? keyword = null,
            [FromQuery] string? status = null,
            [FromQuery] string? sortColumn = "Id",
            [FromQuery] string? sortDir = "Asc")
        {
            try
            {
                var result = await _projectService.GetByMemberIdPagedAsync(
                    memberId,
                    pageIndex,
                    pageSize,
                    keyword,
                    status,
                    sortColumn,
                    sortDir);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log the full exception if you have logging (e.g., ILogger)
                return BadRequest(new { message = "An unexpected error occurred while retrieving member's projects." });
            }
        }
    }
}