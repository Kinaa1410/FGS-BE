using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.DTOs.Wallets;
using FGS_BE.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FGS_BE.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WalletsController : ControllerBase
{
    private readonly IWalletService _service;

    public WalletsController(IWalletService service)
    {
        _service = service;
    }

    // GET: api/wallets?pageIndex=1&pageSize=10&keyword=abc
    [HttpGet]
    public async Task<ActionResult<PaginatedList<WalletDto>>> GetPaged(
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? keyword = null)
    {
        var result = await _service.GetPagedAsync(pageIndex, pageSize, keyword);
        return Ok(result);
    }

    // GET: api/wallets/user/5/balance
    [HttpGet("user/{userId}/balance")]
    public async Task<ActionResult<decimal>> GetBalanceByUserId(int userId)
    {
        if (userId <= 0) return BadRequest(new { error = "User ID must be greater than 0" });
        var balance = await _service.GetBalanceByUserIdAsync(userId);
        return Ok(new { userId, balance });
    }

    // GET: api/wallets/user/5
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<WalletDto>> GetByUserId(int userId)
    {
        if (userId <= 0)
            return BadRequest(new { error = "User ID must be greater than 0" });

        var wallet = await _service.GetByUserIdAsync(userId);

        if (wallet == null)
            return NotFound(new { error = "Wallet not found for this user" });

        return Ok(wallet);
    }

    // POST: api/wallets/user/5/add-points (dùng khi project hoàn thành để cộng điểm)
    //[HttpPost("user/{userId}/add-points")]
    //public async Task<ActionResult<WalletDto>> AddPoints(int userId, [FromBody] AddPointsRequest request)
    //{
    //    if (userId <= 0) return BadRequest(new { error = "User ID must be greater than 0" });
    //    if (request.Points <= 0) return BadRequest(new { error = "Points must be greater than 0" });

    //    var updatedWallet = await _service.CreateOrUpdateAsync(userId, request.Points);
    //    return Ok(updatedWallet);
    //}

    // DELETE: api/wallets/id
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0) return BadRequest(new { error = "ID must be greater than 0" });
        var success = await _service.DeleteAsync(id);
        return success ? NoContent() : NotFound(new { error = "Wallet not found" });
    }
}

// Request DTO cho việc cộng điểm
public class AddPointsRequest
{
    public decimal Points { get; set; }
}