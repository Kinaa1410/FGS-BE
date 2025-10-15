using FGS_BE.Repo.DTOs.TermKeywords;
using FGS_BE.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FGS_BE.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TermKeywordController : ControllerBase
    {
        private readonly ITermKeywordService _service;

        public TermKeywordController(ITermKeywordService service)
        {
            _service = service;
        }

        /// <summary>
        /// get-all term keywords with pagination, sorting, and filtering
        /// </summary>
        /// <param name="pageIndex">Số trang hiện tại</param>
        /// <param name="pageSize">Số lượng bản ghi mỗi trang</param>
        /// <param name="keyword">Search theo keyword</param>
        /// <param name="sortColumn">Tên cột cần sắp xếp</param>
        /// <param name="sortDir">Chiều sắp Asc, Desc</param>
        /// <param name="semesterId">filter theo học kỳ</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? keyword = null,
            [FromQuery] string? sortColumn = "Id",
            [FromQuery] string? sortDir = "Asc",
            [FromQuery] int? semesterId = null)
        {
            var result = await _service.GetPagedAsync(pageIndex, pageSize, keyword, sortColumn, sortDir, semesterId);
            return Ok(result);
        }

        /// <summary>
        /// get-by-id term keyword
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// create term keyword
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTermKeywordDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// update term keyword
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTermKeywordDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// delete term keyword
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}
