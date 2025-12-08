using FGS_BE.Service.Interfaces;
using FGS_BE.Repo.DTOs.PerformanceScores;
using Microsoft.AspNetCore.Mvc;

namespace FGS_BE.API.Controllers
{
    [Route("api/performance-scores")]
    [ApiController]
    public class PerformanceScoresController : ControllerBase
    {
        private readonly IPerformanceScoreService _service;

        public PerformanceScoresController(IPerformanceScoreService service)
        {
            _service = service;
        }

        // GET
        [HttpGet]
        public async Task<IActionResult> GetPaged(
            int pageIndex = 1,
            int pageSize = 10,
            string? keyword = null,
            int? userId = null,
            int? projectId = null,
            int? milestoneId = null,
            int? taskId = null,
            string? sortColumn = "Id",
            string? sortDir = "Asc")
        {
            return Ok(await _service.GetPagedAsync(
                pageIndex, pageSize, keyword, userId, projectId, milestoneId, taskId, sortColumn, sortDir));
        }

        // GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePerformanceScoreDto dto)
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
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Không thể tạo performance score.", detail = ex.Message });
            }
        }


        // PUT
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdatePerformanceScoreDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            return result == null ? NotFound() : Ok(result);
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            bool success = await _service.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }
    }
}
