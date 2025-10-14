using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.DTOs.Users;
using FGS_BE.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FGS_BE.Controllers;
[Route("api/[controller]")]
[ApiController]
public class UsersController(IUserService service) : ControllerBase
{

    [HttpPost("login")]
    public async Task<ActionResult<UserResponse>> Login(LoginRequest request)
    {
        return await service.LoginAsync(request);
    }

    [HttpPost("register")]
    public async Task<ActionResult<MessageResponse>> Register(RegisterRequest request)
    {
        await service.RegisterAsync(request);
        return new MessageResponse("Created Success");
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<UserResponse>>> Find([FromQuery] GetUsersQuery request)
    {
        return await service.FindAsync(request);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponse>> FindById(int id)
    {
        return await service.FindByAsync(id);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<MessageResponse>> Update(int id, UpdateUserCommand request)
    {
        await service.UpdateAsync(id, request);
        return new MessageResponse("Updated Success");
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<MessageResponse>> Delete(int id)
    {
        await service.DeleteAsync(id);
        return new MessageResponse("Deleted Success");
    }
}
