using FGS_BE.Repo.DTOs.Notifications;
using FGS_BE.Service.Interfaces;
using FGS_BE.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FGS_BE.API.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _service;

        public NotificationsController(INotificationService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get paginated notifications for a user
        /// </summary>
        /// <param name="userId">User ID to filter notifications</param>
        /// <param name="pageIndex">Current page index</param>
        /// <param name="pageSize">Number of records per page</param>
        /// <param name="isRead">Filter by read status (true/false/null for all)</param>
        /// <param name="sortColumn">Column to sort by</param>
        /// <param name="sortDir">Sort direction: Asc/Desc</param>
        /// <returns>Paginated list of notifications</returns>
        [HttpGet]
        public async Task<IActionResult> GetPagedByUser(
            [FromQuery] int userId,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool? isRead = null,
            [FromQuery] string? sortColumn = "Id",
            [FromQuery] string? sortDir = "Desc")
        {
            var result = await _service.GetPagedByUserAsync(userId, pageIndex, pageSize, isRead, sortColumn, sortDir);
            return Ok(result);
        }

        /// <summary>
        /// Mark a notification as read
        /// </summary>
        /// <param name="id">Notification ID</param>
        /// <returns>Updated notification or NotFound</returns>
        [HttpPatch("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var result = await _service.MarkAsReadAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// Send a notification to a user using a template
        /// </summary>
        /// <param name="dto">Send notification details</param>
        /// <returns>Created notification</returns>
        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] SendNotificationDto dto)
        {
            var result = await _service.SendNotificationAsync(dto);
            return CreatedAtAction(nameof(GetPagedByUser), new { userId = dto.UserId }, result);
        }
    }
}