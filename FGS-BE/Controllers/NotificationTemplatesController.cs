using FGS_BE.Repo.DTOs.Notifications;
using FGS_BE.Repo.DTOs.Notifications.Validators;
using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Service.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace FGS_BE.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationTemplatesController : ControllerBase
    {
        private readonly INotificationTemplateService _service;
        private readonly IValidator<CreateNotificationTemplateDto> _createValidator;
        private readonly IValidator<UpdateNotificationTemplateDto> _updateValidator;

        public NotificationTemplatesController(
            INotificationTemplateService service,
            IValidator<CreateNotificationTemplateDto> createValidator,
            IValidator<UpdateNotificationTemplateDto> updateValidator)
        {
            _service = service;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        /// <summary>
        /// Get all notification templates with pagination, keyword search, and sorting.
        /// </summary>
        /// <response code="200">Paginated list of templates.</response>
        /// <response code="400">Invalid query parameters.</response>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedList<NotificationTemplateDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PaginatedList<NotificationTemplateDto>>> GetAll(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? keyword = null,
            [FromQuery] string? sortColumn = "Id",
            [FromQuery] string? sortDir = "Asc")
        {
            if (pageIndex < 1) return BadRequest(new { error = "Page index must be >= 1" });
            if (pageSize < 1 || pageSize > 100) return BadRequest(new { error = "Page size must be between 1-100" });
            if (!new[] { "Asc", "Desc" }.Contains(sortDir?.ToUpper())) return BadRequest(new { error = "Sort direction must be Asc or Desc" });
            if (!new[] { "Id", "Code", "SubjectTemplate", "CreatedAt" }.Contains(sortColumn?.ToLower())) return BadRequest(new { error = "Sort column is invalid" });

            var result = await _service.GetPagedAsync(pageIndex, pageSize, keyword, sortColumn, sortDir);
            return Ok(result);
        }

        /// <summary>
        /// Get notification template by ID.
        /// </summary>
        /// <response code="200">Template details.</response>
        /// <response code="400">Invalid ID.</response>
        /// <response code="404">Not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(NotificationTemplateDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<NotificationTemplateDto>> GetById(int id)
        {
            if (id <= 0) return BadRequest(new { error = "ID must be greater than 0" });
            var result = await _service.GetByIdAsync(id);
            return result == null ? NotFound(new { error = "Template not found" }) : Ok(result);
        }

        /// <summary>
        /// Create a new notification template.
        /// </summary>
        /// <response code="201">Created template.</response>
        /// <response code="400">Invalid input or duplicate code.</response>
        [HttpPost]
        [ProducesResponseType(typeof(NotificationTemplateDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<NotificationTemplateDto>> Create([FromBody] CreateNotificationTemplateDto dto)
        {
            var validationResult = await _createValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(new { errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }) });
            }

            try
            {
                var result = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing notification template.
        /// </summary>
        /// <response code="200">Updated template.</response>
        /// <response code="400">Invalid input or duplicate code.</response>
        /// <response code="404">Not found.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(NotificationTemplateDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<NotificationTemplateDto>> Update(int id, [FromBody] UpdateNotificationTemplateDto dto)
        {
            if (id <= 0) return BadRequest(new { error = "ID must be greater than 0" });

            var validationResult = await _updateValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(new { errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }) });
            }

            try
            {
                var result = await _service.UpdateAsync(id, dto);
                return result == null ? NotFound(new { error = "Template not found" }) : Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Delete a notification template by ID.
        /// </summary>
        /// <response code="204">Deleted.</response>
        /// <response code="400">Invalid ID.</response>
        /// <response code="404">Not found.</response>z
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest(new { error = "ID must be greater than 0" });
            var success = await _service.DeleteAsync(id);
            return success ? NoContent() : NotFound(new { error = "Template not found" });
        }
    }
}