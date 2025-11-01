using FGS_BE.Repo.DTOs.Milestones;
using FGS_BE.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FGS_BE.API.Controllers
{
    [ApiController]
    [Route("api/milestones")]
    public class MilestonesController : ControllerBase
    {
        private readonly IMilestoneService _service;

        public MilestonesController(IMilestoneService service)
        {
            _service = service;
        }

        /// <summary>
        /// get-all danh sách project
        /// </summary>
        /// <param name="pageIndex">Số trang hiện tại</param>
        /// <param name="pageSize">Số lượng bản ghi mỗi trang</param>
        /// <param name="keyword">Search theo Title</param>
        /// <param name="status">Search theo Status</param>
        /// <param name="projectId">Filter theo projectId</param>
        /// <param name="sortColumn">Tên cột cần sắp xếp</param>
        /// <param name="sortDir">Chiều sắp Asc (tăng), Desc (giảm)</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll(
    [FromQuery] int pageIndex = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] string? keyword = null,
    [FromQuery] string? status = null,
    [FromQuery] int? projectId = null,
    [FromQuery] string? sortColumn = "Id",
    [FromQuery] string? sortDir = "Asc")
        {
            var result = await _service.GetPagedAsync(pageIndex, pageSize, keyword, status, projectId, sortColumn, sortDir);
            return Ok(result);
        }

        /// <summary>
        /// get-by-id project
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
        /// create milestone
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMilestoneDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// update milestone
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMilestoneDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// detele milestone
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }
    }
}
