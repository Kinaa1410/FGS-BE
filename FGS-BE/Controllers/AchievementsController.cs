using FGS_BE.Repo.DTOs.Achievements;
using FGS_BE.Repo.DTOs.Pages;
using FGS_BE.Repo.DTOs.Users;
using FGS_BE.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FGS_BE.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AchievementsController(IAchievementService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<AchievementResponse>>> Find([FromQuery] GetAchievementsQuery request)
    {
        return await service.FindAsync(request);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AchievementResponse>> FindById(int id)
    {
        return await service.FindByAsync(id);
    }

    [HttpPost]
    public async Task<ActionResult<MessageResponse>> Create(CreateAchievementCommand command)
    {
        await service.CreateAsync(command);
        return new MessageResponse("Created Success");
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<MessageResponse>> Update(int id, UpdateAchievementCommand request)
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
