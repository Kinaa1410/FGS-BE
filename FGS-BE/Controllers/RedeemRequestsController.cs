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

        /// <summary>
        /// Get all redeem requests with pagination and filters
        /// </summary>
        /// <param name="pageIndex">Current page number</param>
        /// <param name="pageSize">Number of records per page</param>
        /// <param name="keyword">Search by RewardItem name or description</param>
        /// <param name="status">Filter by Status</param>
        /// <param name="userId">Filter by UserId</param>
        /// <param name="rewardItemId">Filter by RewardItemId</param>
        /// <param name="sortColumn">Column to sort by</param>
        /// <param name="sortDir">Sort direction: Asc or Desc</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? keyword = null,
            [FromQuery] string? status = null,
            [FromQuery] int? userId = null,
            [FromQuery] int? rewardItemId = null,
            [FromQuery] string? sortColumn = "Id",
            [FromQuery] string? sortDir = "Asc")
        {
            var result = await _service.GetPagedAsync(pageIndex, pageSize, keyword, status, userId, rewardItemId, sortColumn, sortDir);
            return Ok(result);
        }

        /// <summary>
        /// Get redeem requests by user ID with pagination
        /// </summary>
        /// <param name="userId">User ID to filter requests</param>
        /// <param name="pageIndex">Current page number</param>
        /// <param name="pageSize">Number of records per page</param>
        /// <param name="status">Filter by Status</param>
        /// <param name="sortColumn">Column to sort by</param>
        /// <param name="sortDir">Sort direction: Asc or Desc</param>
        /// <returns></returns>
        [HttpGet("by-user/{userId}")]
        public async Task<IActionResult> GetByUser(
            int userId,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? status = null,
            [FromQuery] string? sortColumn = "Id",
            [FromQuery] string? sortDir = "Asc")
        {
            var result = await _service.GetPagedByUserAsync(userId, pageIndex, pageSize, status, sortColumn, sortDir);
            return Ok(result);
        }

        /// <summary>
        /// Get redeem request by ID
        /// </summary>
        /// <param name="id">Redeem request ID</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// Create a new redeem request
        /// </summary>
        /// <param name="dto">Create data</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRedeemRequestDto dto)
        {
            var result = await _service.CreateAsync(dto);
            if (result == null)
            {
                return BadRequest(new { message = "You don't have enough points to exchange" });
            }
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Update redeem request status (approve/reject)
        /// </summary>
        /// <param name="id">Redeem request ID</param>
        /// <param name="dto">Update status data</param>
        /// <returns></returns>
        [HttpPut("{id}/status")]
        public async Task<IActionResult> ApproveOrReject(int id, [FromBody] UpdateStatusRedeemRequestDto dto)
        {
            var result = await _service.UpdateStatusAsync(id, dto);
            if (result == null)
            {
                return BadRequest(new { message = "Invalid status. Only Approved or Rejected allowed, and only for Pending requests." });
            }
            return Ok(result);
        }

        /// <summary>
        /// Delete redeem request
        /// </summary>
        /// <param name="id">Redeem request ID</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }
    }
}