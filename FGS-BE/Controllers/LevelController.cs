using FGS_BE.Repo.DTOs.Levels;
using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FGS_BE.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LevelsController : ControllerBase
{
    private readonly ILevelService _service;

    public LevelsController(ILevelService service)
    {
        _service = service;
    }

    /// <summary>
    /// Get all levels with pagination, keyword search, and sorting.
    /// </summary>
    /// <param name="pageIndex">Current page index (default: 1).</param>
    /// <param name="pageSize">Items per page (default: 10).</param>
    /// <param name="keyword">Search by name or description.</param>
    /// <param name="sortColumn">Column to sort by (default: Id).</param>
    /// <param name="sortDir">Sort direction: Asc or Desc (default: Asc).</param>
    /// <returns>Paginated list of levels.</returns>
    /// <response code="200">Success.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<LevelDto>), StatusCodes.Status200OK)]
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

    /// <summary>
    /// Get level by ID.
    /// </summary>
    /// <param name="id">Level ID.</param>
    /// <returns>Level details or NotFound.</returns>
    /// <response code="200">Success.</response>
    /// <response code="404">Not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(LevelDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LevelDto>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Create a new level.
    /// </summary>
    /// <param name="dto">Level creation data.</param>
    /// <returns>Created level with 201 status.</returns>
    /// <response code="201">Created.</response>
    /// <response code="400">Invalid input.</response>
    [HttpPost]
    [ProducesResponseType(typeof(LevelDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LevelDto>> Create([FromBody] CreateLevelDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Update an existing level.
    /// </summary>
    /// <param name="id">Level ID.</param>
    /// <param name="dto">Updated level data (partial).</param>
    /// <returns>Updated level or NotFound.</returns>
    /// <response code="200">Updated.</response>
    /// <response code="404">Not found.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(LevelDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LevelDto>> Update(int id, [FromBody] UpdateLevelDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _service.UpdateAsync(id, dto);
        return result == null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Delete a level by ID.
    /// </summary>
    /// <param name="id">Level ID.</param>
    /// <returns>NoContent on success, NotFound otherwise.</returns>
    /// <response code="204">Deleted.</response>
    /// <response code="404">Not found.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _service.DeleteAsync(id);
        return success ? NoContent() : NotFound();
    }

    /// <summary>
    /// Check and auto-assign level to a user if their points meet a condition.
    /// </summary>
    /// <param name="userId">User ID.</param>
    /// <param name="request">Current points to check.</param>
    /// <returns>Newly assigned user level or NotFound if no upgrade available.</returns>
    /// <response code="200">Level assigned.</response>
    /// <response code="404">No eligible upgrade.</response>
    [HttpPost("user/{userId}/check-level-up")]
    [ProducesResponseType(typeof(UserLevelDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserLevelDto>> CheckAndAssignUserLevel(int userId, [FromBody] CheckLevelUpRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _service.CheckAndAssignUserLevelAsync(userId, request);
        return result == null ? NotFound("No eligible level upgrade available.") : Ok(result);
    }
}