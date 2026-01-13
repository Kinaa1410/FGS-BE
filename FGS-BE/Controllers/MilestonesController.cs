using FGS_BE.Repo.DTOs.Milestones;
using FGS_BE.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FGS_BE.API.Controllers
{
    [ApiController]
    [Route("api/milestones")]
    //[Authorize]  // New: Require auth (e.g., mentor role for updates/delays)
    public class MilestonesController : ControllerBase
    {
        private readonly IMilestoneService _service;

        public MilestonesController(IMilestoneService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get paginated list of milestones with filters
        /// </summary>
        /// <param name="pageIndex">Current page number</param>
        /// <param name="pageSize">Number of records per page</param>
        /// <param name="keyword">Search by Title</param>
        /// <param name="status">Filter by Status</param>
        /// <param name="projectId">Filter by ProjectId</param>
        /// <param name="sortColumn">Column name for sorting</param>
        /// <param name="sortDir">Sort direction: Asc (ascending), Desc (descending)</param>
        /// <returns>Paginated milestones</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? keyword = null,
            [FromQuery] string? status = null,
            [FromQuery] int? projectId = null,
            [FromQuery] string? sortColumn = "Id",
            [FromQuery] string? sortDir = "Asc")
        {
            var result = await _service.GetPagedAsync(pageIndex, pageSize, keyword, status, projectId, sortColumn, sortDir);
            return Ok(result);
        }

        /// <summary>
        /// Get milestone by ID
        /// </summary>
        /// <param name="id">Milestone ID</param>
        /// <returns>Milestone details</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// Create a new milestone
        /// </summary>
        /// <param name="dto">Milestone creation data</param>
        /// <returns>Created milestone</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMilestoneDto dto)
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
                return StatusCode(500, new { message = "Unable to create milestone.", detail = ex.Message });
            }
        }

        /// <summary>
        /// Update milestone by ID
        /// </summary>
        /// <param name="id">Milestone ID</param>
        /// <param name="dto">Updated milestone data</param>
        /// <returns>Updated milestone</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMilestoneDto dto)
        {
            try
            {
                var result = await _service.UpdateAsync(id, dto);
                return result == null ? NotFound() : Ok(result);
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
                return StatusCode(500, new { message = "Unable to update milestone.", detail = ex.Message });
            }
        }

        /// <summary>
        /// Delete milestone by ID
        /// </summary>
        /// <param name="id">Milestone ID</param>
        /// <returns>No content if successful</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _service.DeleteAsync(id);
                return success ? NoContent() : NotFound(new { message = "Milestone not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Unable to delete milestone.", detail = ex.Message });
            }
        }

        /// <summary>
        /// Apply delay buffer to milestone (extends DueDate by hours; for manual override)
        /// </summary>
        /// <param name="id">Milestone ID</param>
        /// <param name="hours">Delay hours (default 36)</param>
        /// <returns>No content if successful</returns>
        [HttpPut("{id}/delay-buffer")]
        public async Task<IActionResult> ApplyDelayBuffer(int id, [FromQuery] int hours = 36)
        {
            try
            {
                await _service.ApplyDelayBufferAsync(id, hours);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Unable to apply delay buffer.", detail = ex.Message });
            }
        }

        /// <summary>
        /// Reset delay on milestone (reverts to OriginalDueDate)
        /// </summary>
        /// <param name="id">Milestone ID</param>
        /// <returns>No content if successful</returns>
        [HttpPut("{id}/reset-delay")]
        public async Task<IActionResult> ResetDelay(int id)
        {
            try
            {
                await _service.ResetDelayAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Unable to reset delay.", detail = ex.Message });
            }
        }
    }
}