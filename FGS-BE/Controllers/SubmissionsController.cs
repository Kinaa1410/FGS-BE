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

        /// <summary>
        /// get pages submissions with filter
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="userId"></param>
        /// <param name="taskId"></param>
        /// <param name="sortColumn"></param>
        /// <param name="sortDir"></param>
        /// <returns></returns>
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

        /// <summary>
        /// get submission by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<SubmissionDto>> GetById(int id)
        {
            var submission = await _service.GetByIdAsync(id);
            if (submission == null)
                return NotFound();

            return Ok(submission);
        }

        /// <summary>
        /// add new submission
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] CreateSubmissionDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }


        /// <summary>
        /// update submission by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAsync(int id, UpdateSubmissionDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            return updated == null ? NotFound() : Ok(updated);
        }

        /// <summary>
        /// delete submission by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }

        /// <summary>
        /// grade and feedback submission by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut("{id}/grade")]
        public async Task<IActionResult> GradeSubmission(int id, [FromBody] GradeSubmissionDto dto)
        {
            var result = await _service.GradeSubmissionAsync(id, dto);
            if (result == null)
                return NotFound();

            return Ok(result);
        }
    }
}
