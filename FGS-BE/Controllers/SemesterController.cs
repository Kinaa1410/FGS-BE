using FGS_BE.Repo.DTOs.Semesters;
using FGS_BE.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FGS_BE.Controllers
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
            var result = await _service.GetPagedAsync(pageIndex, pageSize, keyword, status, sortColumn, sortDir);
            return Ok(result);
        }

        /// <summary>
        /// get-by-id semester
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var semester = await _service.GetByIdAsync(id);
            if (semester == null)
                return NotFound();
            return Ok(semester);
        }

        /// <summary>
        /// create semester
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSemesterDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
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
            var result = await _service.UpdateAsync(id, dto);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// delete semester
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }
    }
}
