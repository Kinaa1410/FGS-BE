using FGS_BE.Repo.DTOs.RedeemRequests;
using FGS_BE.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FGS_BE.API.Controllers
{
    [ApiController]
    [Route("api/redeemrequests")]
    public class RedeemRequestsController : ControllerBase
    {
        private readonly IRedeemRequestService _service;

        public RedeemRequestsController(IRedeemRequestService service)
        {
            _service = service;
        }

        // ===========================
        // GET ALL (Updated: Add 'collected' query param)
        // ===========================
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? keyword = null,
            [FromQuery] string? status = null,
            [FromQuery] int? userId = null,
            [FromQuery] int? rewardItemId = null,
            [FromQuery] bool? collected = null,  // New
            [FromQuery] string? sortColumn = "Id",
            [FromQuery] string? sortDir = "Asc")
        {
            var result = await _service.GetPagedAsync(
                pageIndex, pageSize, keyword, status, userId, rewardItemId, collected, sortColumn, sortDir);
            return Ok(result);
        }

        // ===========================
        // GET BY USER (Updated: Add 'collected' query param)
        // ===========================
        [HttpGet("by-user/{userId}")]
        public async Task<IActionResult> GetByUser(
            int userId,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? status = null,
            [FromQuery] bool? collected = null,  // New: e.g., ?collected=false for pending pickups
            [FromQuery] string? sortColumn = "Id",
            [FromQuery] string? sortDir = "Asc")
        {
            var result = await _service.GetPagedByUserAsync(
                userId, pageIndex, pageSize, status, collected, sortColumn, sortDir);
            return Ok(result);
        }

        // ===========================
        // GET BY ID (No changes)
        // ===========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null
                ? NotFound(new { message = "Redeem request not found." })
                : Ok(result);
        }

        // ===========================
        // CREATE (No changes)
        // ===========================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRedeemRequestDto dto)
        {
            try
            {
                var result = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        // ===========================
        // UPDATE STATUS (No changes)
        // ===========================
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(
            int id,
            [FromBody] UpdateStatusRedeemRequestDto dto)
        {
            try
            {
                var result = await _service.UpdateStatusAsync(id, dto);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ===========================
        // NEW: MARK AS COLLECTED
        // ===========================
        [HttpPut("{id}/collect")]
        public async Task<IActionResult> MarkAsCollected(int id)
        {
            try
            {
                var result = await _service.MarkAsCollectedAsync(id);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ===========================
        // DELETE (No changes)
        // ===========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}