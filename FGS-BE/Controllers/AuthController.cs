using FGS_BE.Repo.DTOs.Users;
using FGS_BE.Repo.Resources;
using FGS_BE.Service.Services;
using FGS_BE.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FGS_BE.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController(IUserService service) : ControllerBase
{
    /// <remarks>
    /// ```
    /// Admin account: admin@gmail.com - @1
    /// Staff account: staff@gmail.com - @1
    /// ```
    /// </remarks>
    [HttpPost("login")]
    public async Task<ActionResult<AccessTokenResponse>> Login(LoginRequest request)
    {
        return await service.LoginAsync(request);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AccessTokenResponse>> RefreshToken(RefreshTokenRequest request)
    {
        return await service.RefreshTokenAsync(request);
    }

    [HttpPost("register")]
    public async Task<ActionResult<MessageResponse>> Register(RegisterRequest request)
    {
        await service.RegisterAsync(request);
        return new MessageResponse(Resource.CreatedSuccess);
    }

    [Authorize]
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
    {
        var userId = int.Parse(User.FindFirst("id")!.Value);
        await service.ChangePasswordAsync(userId, request);
        return Ok(new { message = "Password updated successfully" });
    }

}
