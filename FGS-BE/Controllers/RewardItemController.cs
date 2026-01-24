using FGS_BE.Repo.DTOs.RewardItems;
using FGS_BE.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FGS_BE.Controllers
{
    [ApiController]
    [Route("api/reward-items")]
    public class RewardItemController : ControllerBase
    {
        private readonly IRewardItemService _service;

        public RewardItemController(IRewardItemService service)
        {
            _service = service;
        }

        // ============================
        // GET ALL
        // ============================
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? keyword = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] string? sortColumn = "Id",
            [FromQuery] string? sortDir = "Asc")
        {
            var result = await _service.GetPagedAsync(
                pageIndex, pageSize, keyword, isActive, sortColumn, sortDir);

            return Ok(result);
        }

        // ============================
        // GET BY ID
        // ============================
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var rewardItem = await _service.GetByIdAsync(id);

            if (rewardItem == null)
                return NotFound(new { message = "Reward item not found." });

            return Ok(rewardItem);
        }

        // ============================
        // CREATE
        // ============================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRewardItemDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // ✅ 400 validation error

            var created = await _service.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById),
                new { id = created.Id },
                created);
        }

        // ============================
        // UPDATE
        // ============================
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRewardItemDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // ✅ 400

            var result = await _service.UpdateAsync(id, dto);

            if (result == null)
                return NotFound(new { message = "Reward item not found." }); // ✅ 404

            return Ok(result);
        }

        // ============================
        // DELETE
        // ============================
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);

            if (!success)
                return NotFound(new { message = "Reward item not found." }); // ✅ 404

            return NoContent(); // ✅ 204
        }
    }
}
