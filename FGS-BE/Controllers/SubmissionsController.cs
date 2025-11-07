using FGS_BE.Repo.DTOs.Submissions;
using FGS_BE.Service.Implements;
using FGS_BE.Service.Interfaces;
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

        [HttpGet("{id}")]
        public async Task<ActionResult<SubmissionDto>> GetById(int id)
        {
            var submission = await _service.GetByIdAsync(id);
            if (submission == null)
                return NotFound();

            return Ok(submission);
        }


        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateSubmissionDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAsync(int id, UpdateSubmissionDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}
