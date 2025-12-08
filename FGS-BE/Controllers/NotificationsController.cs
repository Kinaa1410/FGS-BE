using FGS_BE.Repo.DTOs.Notifications;
using FGS_BE.Repo.DTOs.Notifications.Validators; // Add this
using FGS_BE.Service.Interfaces;
using FGS_BE.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace FGS_BE.API.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _service;
        private readonly IValidator<SendNotificationDto> _sendValidator;

        public NotificationsController(
            INotificationService service,
            IValidator<SendNotificationDto> sendValidator)
        {
            _service = service;
            _sendValidator = sendValidator;
        }

        /// <summary>
        /// Get paginated notifications for a user
        /// </summary>
        /// <response code="200">Paginated list of notifications</response>
        /// <response code="400">Invalid query parameters</response>
        [HttpGet]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)] // PaginatedList<NotificationDto>
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPagedByUser(
            [FromQuery] int userId,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool? isRead = null,
            [FromQuery] string? sortColumn = "CreatedAt",
            [FromQuery] string? sortDir = "Desc")
        {
            // Validate query params
            if (userId <= 0) return BadRequest(new { error = "User ID must be greater than 0" });
            if (pageIndex < 1) return BadRequest(new { error = "Page index must be >= 1" });
            if (pageSize < 1 || pageSize > 100) return BadRequest(new { error = "Page size must be between 1-100" });
            if (!new[] { "Asc", "Desc" }.Contains(sortDir?.ToUpper())) return BadRequest(new { error = "Sort direction must be Asc or Desc" });
            if (!new[] { "Id", "Subject", "CreatedAt", "IsRead" }.Contains(sortColumn?.ToLower())) return BadRequest(new { error = "Sort column is invalid" });

            var result = await _service.GetPagedByUserAsync(userId, pageIndex, pageSize, isRead, sortColumn, sortDir);
            return Ok(result);
        }

        /// <summary>
        /// Mark a notification as read
        /// </summary>
        /// <response code="200">Updated notification</response>
        /// <response code="400">Invalid ID</response>
        /// <response code="404">Notification not found</response>
        [HttpPatch("{id}/read")]
        [ProducesResponseType(typeof(NotificationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            if (id <= 0) return BadRequest(new { error = "Notification ID must be greater than 0" });
            var result = await _service.MarkAsReadAsync(id);
            return result == null ? NotFound(new { error = "Notification not found" }) : Ok(result);
        }

        /// <summary>
        /// Send a notification to a user using a template
        /// </summary>
        /// <response code="201">Created notification</response>
        /// <response code="400">Invalid input</response>
        /// <response code="404">Template not found</response>
        [HttpPost("send")]
        [ProducesResponseType(typeof(NotificationDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Send([FromBody] SendNotificationDto dto)
        {
            var validationResult = await _sendValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(new { errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }) });
            }

            var result = await _service.SendNotificationAsync(dto);
            if (result == null)
            {
                return NotFound(new { error = "No active template found with the provided code" });
            }

            return CreatedAtAction(nameof(GetPagedByUser), new { userId = dto.UserId }, result);
        }
    }
}