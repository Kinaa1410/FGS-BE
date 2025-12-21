using FGS_BE.Repo.DTOs.Projects;
using FGS_BE.Service.Interfaces;  // Standardized namespace (no 's')
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;  // For async Task<IActionResult>

namespace FGS_BE.API.Controllers  // Assume API folder
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
                var result = await _projectService.GetPagedAsync(pageIndex, pageSize, keyword, status, sortColumn, sortDir);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log if ILogger injected
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
                // Log if ILogger injected
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
                return BadRequest(ModelState);
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
                return BadRequest(ModelState);
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

        /// <summary>
        /// Bắt đầu project (chỉ mentor của project)
        /// </summary>
        /// <param name="id">Project ID</param>
        /// <returns></returns>
        [HttpPost("{id}/start")]
        [Authorize]
        public async Task<IActionResult> StartByMentor(int id)
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int mentorId))
                return Unauthorized("User ID not found in token.");
            try
            {
                var success = await _projectService.StartByMentorAsync(id, mentorId);
                return success ? NoContent() : NotFound("Project not found.");
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log if possible
                return BadRequest(new { message = "An unexpected error occurred while starting the project." });
            }
        }

        /// <summary>
        /// Kết thúc project (chỉ mentor của project)
        /// </summary>
        /// <param name="id">Project ID</param>
        /// <returns></returns>
        [HttpPost("{id}/close")]
        [Authorize]
        public async Task<IActionResult> CloseByMentor(int id)
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int mentorId))
                return Unauthorized("User ID not found in token.");
            try
            {
                var success = await _projectService.CloseByMentorAsync(id, mentorId);
                return success ? NoContent() : NotFound("Project not found.");
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log if possible
                return BadRequest(new { message = "An unexpected error occurred while closing the project." });
            }
        }

        /// <summary>
        /// Get paged list of projects assigned to a specific mentor.
        /// </summary>
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
                var result = await _projectService.GetByMentorIdPagedAsync(mentorId, pageIndex, pageSize, keyword, status, sortColumn, sortDir);
                if (result.TotalItems == 0)
                {
                    return Ok(new { message = "No projects found for this mentor.", data = result });
                }
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

        /// <summary>
        /// Get paged list of projects where a specific user is a member.
        /// </summary>
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
                var result = await _projectService.GetByMemberIdPagedAsync(memberId, pageIndex, pageSize, keyword, status, sortColumn, sortDir);
                if (result.TotalItems == 0)
                {
                    return Ok(new { message = "No projects found for this member.", data = result });
                }
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