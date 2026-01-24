using FGS_BE.Repo.DTOs.Levels;
using FGS_BE.Repo.DTOs.Levels.Validators;
using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Service.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace FGS_BE.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LevelsController : ControllerBase
    {
        private readonly ILevelService _service;
        private readonly IValidator<CreateLevelDto> _createValidator;
        private readonly IValidator<UpdateLevelDto> _updateValidator;
        private readonly IValidator<CheckLevelUpRequest> _checkValidator;

        public LevelsController(
            ILevelService service,
            IValidator<CreateLevelDto> createValidator,
            IValidator<UpdateLevelDto> updateValidator,
            IValidator<CheckLevelUpRequest> checkValidator)
        {
            _service = service;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _checkValidator = checkValidator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PaginatedList<LevelDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PaginatedList<LevelDto>>> GetAll(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? keyword = null,
            [FromQuery] string? sortColumn = "Id",
            [FromQuery] string? sortDir = "Asc")
        {
            var result = await _service.GetPagedAsync(pageIndex, pageSize, keyword, sortColumn, sortDir);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(LevelDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LevelDto>> GetById(int id)
        {
            if (id <= 0) return BadRequest(new { error = "ID must larger than 0" });
            var result = await _service.GetByIdAsync(id);
            return result == null ? NotFound(new { error = "Level doesn't exist" }) : Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(LevelDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<LevelDto>> Create([FromBody] CreateLevelDto dto)
        {
            var validationResult = await _createValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(new { errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }) });
            }
            var result = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(LevelDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LevelDto>> Update(int id, [FromBody] UpdateLevelDto dto)
        {
            if (id <= 0) return BadRequest(new { error = "ID must larger than 0" });

            var validationResult = await _updateValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(new { errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }) });
            }

            var result = await _service.UpdateAsync(id, dto);
            return result == null ? NotFound(new { error = "Level doesn't exist" }) : Ok(result);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest(new { error = "ID must larger than 0" });
            var success = await _service.DeleteAsync(id);
            return success ? NoContent() : NotFound(new { error = "Level doesn't exist" });
        }

        [HttpPost("user/{userId}/check-level-up")]
        [ProducesResponseType(typeof(UserLevelDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserLevelDto>> CheckAndAssignUserLevel(int userId, [FromBody] CheckLevelUpRequest request)
        {
            if (userId <= 0) return BadRequest(new { error = "User ID must larger than 0" });

            var validationResult = await _checkValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new { errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }) });
            }

            var result = await _service.CheckAndAssignUserLevelAsync(userId, request);
            return result == null ? NotFound(new { error = "Doesn't have suitable level upgrade" }) : Ok(result);
        }
    }
}