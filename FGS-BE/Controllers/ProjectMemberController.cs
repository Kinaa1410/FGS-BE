using FGS_BE.Repo.DTOs.ProjectMembers;
using FGS_BE.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FGS_BE.API.Controllers
{
    [ApiController]
    [Route("api/project-members")]
    public class ProjectMembersController : ControllerBase
    {
        private readonly IProjectMemberService _service;

        public ProjectMembersController(IProjectMemberService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get paged project members (filter by project or user)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetPaged(
            int pageIndex = 1,
            int pageSize = 10,
            string? keyword = null,
            int? projectId = null,
            int? userId = null,
            string? sortColumn = "Id",
            string? sortDir = "Asc")
        {
            // Input validation (basic range checks)
            if (pageIndex < 1) return BadRequest("PageIndex must be at least 1.");
            if (pageSize < 1 || pageSize > 100) return BadRequest("PageSize must be between 1 and 100.");
            if (!new[] { "Asc", "Desc" }.Contains(sortDir?.ToLowerInvariant())) return BadRequest("SortDir must be 'Asc' or 'Desc'.");

            var result = await _service.GetPagedAsync(pageIndex, pageSize, keyword, projectId, userId, sortColumn, sortDir);
            return Ok(result);
        }

        /// <summary>
        /// Get project member by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0) return BadRequest("ID must be greater than 0.");
            var result = await _service.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// Join a project (creates a new membership)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProjectMemberDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // FluentValidation errors here
            }

            try
            {
                var result = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message }); // Specific validation errors (e.g., role, capacity, already joined)
            }
            catch (Exception ex)
            {
                // Log if ILogger is injected: _logger.LogError(ex, "Unexpected error in CreateProjectMember");
                return StatusCode(500, new { message = "An unexpected error occurred while joining the project." });
            }
        }

        /// <summary>
        /// Update role of a project member
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProjectMemberDto dto)
        {
            if (id <= 0) return BadRequest("ID must be greater than 0.");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // FluentValidation errors here
            }

            try
            {
                var result = await _service.UpdateAsync(id, dto);
                return result == null ? NotFound("Project member not found.") : Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log if ILogger is injected
                return StatusCode(500, new { message = "An unexpected error occurred while updating the project member." });
            }
        }

        /// <summary>
        /// Remove a project member (admin-only or self-remove)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest("ID must be greater than 0.");

            try
            {
                var deleted = await _service.DeleteAsync(id);
                return deleted ? NoContent() : NotFound("Project member not found.");
            }
            catch (Exception ex)
            {
                // Log if ILogger is injected
                return StatusCode(500, new { message = "An unexpected error occurred while deleting the project member." });
            }
        }

        /// <summary>
        /// Leave a project (by user ID and project ID)
        /// </summary>
        [HttpPost("leave")]
        public async Task<IActionResult> Leave([FromBody] LeaveProjectDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // FluentValidation errors here
            }

            if (dto.UserId <= 0 || dto.ProjectId <= 0)
            {
                return BadRequest("UserId and ProjectId must be greater than 0.");
            }

            try
            {
                var left = await _service.LeaveAsync(dto.UserId, dto.ProjectId);
                return left
                    ? Ok(new { message = "Left project successfully." })
                    : NotFound("Not a member of this project.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log if ILogger is injected
                return StatusCode(500, new { message = "An unexpected error occurred while leaving the project." });
            }
        }
    }
}