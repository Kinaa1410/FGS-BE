using FGS_BE.Repo.DTOs.Semesters;
using FGS_BE.Service.Interfaces;  // Standardized namespace (no 's')
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;  // For async Task<IActionResult>

namespace FGS_BE.API.Controllers  // Standardized to API folder
{
    [ApiController]
    [Route("api/semesters")]
    public class SemesterController : ControllerBase
    {
        private readonly ISemesterService _service;
        public SemesterController(ISemesterService service)
        {
            _service = service;
        }

        /// <summary>
        /// get-all danh sách semester
        /// </summary>
        /// <param name="pageIndex">Số trang hiện tại</param>
        /// <param name="pageSize">Số lượng bản ghi mỗi trang</param>
        /// <param name="keyword">Search theo keyword</param>
        /// <param name="status">Search theo status</param>
        /// <param name="sortColumn">Tên cột cần sắp xếp : Id, Name, StartDate..</param>
        /// <param name="sortDir">Chiều sắp Asc (tăng), Desc (giảm)</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? keyword = null,
            [FromQuery] string? status = null,
            [FromQuery] string? sortColumn = "Id",
            [FromQuery] string? sortDir = "Asc")
        {
            try
            {
                var result = await _service.GetPagedAsync(pageIndex, pageSize, keyword, status, sortColumn, sortDir);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log if ILogger injected
                return BadRequest(new { message = "An unexpected error occurred while retrieving semesters." });
            }
        }

        /// <summary>
        /// get-by-id semester
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var semester = await _service.GetByIdAsync(id);
                if (semester == null)
                    return NotFound(new { message = "Semester not found." });
                return Ok(semester);
            }
            catch (Exception ex)
            {
                // Log if ILogger injected
                return BadRequest(new { message = "An unexpected error occurred while retrieving the semester." });
            }
        }

        /// <summary>
        /// create semester
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSemesterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var created = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
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
                // Log if possible
                return BadRequest(new { message = "Cannot create semester.", detail = ex.Message });
            }
        }

        /// <summary>
        /// update semester
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSemesterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var result = await _service.UpdateAsync(id, dto);
                return result == null ? NotFound(new { message = "Semester not found." }) : Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log if possible
                return BadRequest(new { message = "An unexpected error occurred while updating the semester." });
            }
        }

        /// <summary>
        /// delete semester
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _service.DeleteAsync(id);
                return success ? NoContent() : NotFound(new { message = "Semester not found." });
            }
            catch (Exception ex)
            {
                // Log if possible
                return BadRequest(new { message = "An unexpected error occurred while deleting the semester." });
            }
        }
    }
}