using FGS_BE.Repo.DTOs.Submissions;
using FGS_BE.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FGS_BE.API.Controllers
{
    [ApiController]
    [Route("api/submissions")]
    public class SubmissionsController : ControllerBase
    {
        private readonly ISubmissionService _service;

        public SubmissionsController(ISubmissionService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get paginated submissions with filters
        /// </summary>
        /// <param name="pageIndex">Current page number</param>
        /// <param name="pageSize">Number of records per page</param>
        /// <param name="userId">Filter by UserId</param>
        /// <param name="taskId">Filter by TaskId</param>
        /// <param name="sortColumn">Column name for sorting</param>
        /// <param name="sortDir">Sort direction: Asc (ascending), Desc (descending)</param>
        /// <returns>Paginated submissions</returns>
        [HttpGet]
        public async Task<IActionResult> GetPagedAsync(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? userId = null,
            [FromQuery] int? taskId = null,
            [FromQuery] string? sortColumn = "Id",
            [FromQuery] string? sortDir = "Asc")
        {
            var result = await _service.GetPagedAsync(pageIndex, pageSize, userId, taskId, sortColumn, sortDir);
            return Ok(result);
        }

        /// <summary>
        /// Get submission by ID
        /// </summary>
        /// <param name="id">Submission ID</param>
        /// <returns>Submission details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<SubmissionDto>> GetById(int id)
        {
            var submission = await _service.GetByIdAsync(id);
            if (submission == null)
                return NotFound(new { message = "Submission not found." });
            return Ok(submission);
        }

        /// <summary>
        /// Create a new submission
        /// </summary>
        /// <param name="dto">Submission creation data (with file)</param>
        /// <returns>Created submission</returns>
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] CreateSubmissionDto dto)
        {
            try
            {
                var result = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Unable to create submission.", detail = ex.Message });
            }
        }

        /// <summary>
        /// Update submission by ID
        /// </summary>
        /// <param name="id">Submission ID</param>
        /// <param name="dto">Updated submission data</param>
        /// <returns>Updated submission</returns>
        /// <summary>
        /// Update submission (allow replacing file if provided)
        /// </summary>
        [HttpPut("{id:int}")]
        [Consumes("multipart/form-data")] 
        public async Task<IActionResult> UpdateAsync(int id, [FromForm] UpdateSubmissionDto dto)
        {
            try
            {
                var updated = await _service.UpdateAsync(id, dto);
                return updated == null
                    ? NotFound(new { message = "Submission not found." })
                    : Ok(updated);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Unable to update submission.", detail = ex.Message });
            }
        }

        /// <summary>
        /// Delete submission by ID
        /// </summary>
        /// <param name="id">Submission ID</param>
        /// <returns>No content if successful</returns>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                var deleted = await _service.DeleteAsync(id);
                return deleted ? NoContent() : NotFound(new { message = "Submission not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Unable to delete submission.", detail = ex.Message });
            }
        }

        /// <summary>
        /// Review/grade submission by ID (approve/reject with score/feedback)
        /// </summary>
        /// <param name="id">Submission ID</param>
        /// <param name="dto">Review decision, score, and feedback</param>
        /// <returns>Reviewed submission</returns>
        [HttpPut("{id:int}/review")]
        [Authorize(Roles = "Mentor")]
        public async Task<IActionResult> Review(int id, [FromBody] ReviewSubmissionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var result = await _service.ReviewSubmissionAsync(id, dto);
                if (result == null)
                    return NotFound(new { message = "Submission not found." });
                var extensionMessage = dto.ExtendDeadline == true ? "Deadline extended." : "No deadline adjustment.";
                return Ok(new
                {
                    message = dto.Decision?.ToLower() == "approve" ? "Approved successfully" : "Rejected successfully",
                    extensionMessage, 
                    data = result
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Unable to review submission.", detail = ex.Message });
            }
        }
    }
}