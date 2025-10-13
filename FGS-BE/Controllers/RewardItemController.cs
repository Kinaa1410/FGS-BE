using FGS_BE.Repo.DTOs.RewardItems;
using FGS_BE.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FGS_BE.Controllers
{
    [ApiController]
    [Route("api/reward-items")]
    public class RewardItemController : Controller
    {
        private readonly IRewardItemService _service;
        public RewardItemController(IRewardItemService service)
        {
            _service = service;
        }

        /// <summary>
        /// get-all danh sách reward items
        /// </summary>
        /// <param name="pageIndex">Số trang hiện tại</param>
        /// <param name="pageSize">Số lượng bản ghi mỗi trang</param>
        /// <param name="keyword">Search theo keyword</param>
        /// <param name="isActive">Search theo isActive</param>
        /// <param name="sortColumn">Tên cột cần sắp xếp : Id, Name, PriceInPoints..</param>
        /// <param name="sortDir">Chiều sắp Asc (tăng), Desc (giảm)</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll(
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? keyword = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] string? sortColumn = "Id",
        [FromQuery] string? sortDir = "Asc")
        {
            var result = await _service.GetPagedAsync(pageIndex, pageSize, keyword, isActive, sortColumn, sortDir);
            return Ok(result);
        }

        /// <summary>
        /// get-by-id reward item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var rewardItem = await _service.GetByIdAsync(id);
            if (rewardItem == null)
                return NotFound();
            return Ok(rewardItem);
        }

        /// <summary>
        /// create reward item
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRewardItemDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// update reward item
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRewardItemDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// delete reward item
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