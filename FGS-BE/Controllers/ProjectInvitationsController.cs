using FGS_BE.Repo.DTOs.ProjectInvitations;
using FGS_BE.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FGS_BE.API.Controllers;

[ApiController]
[Route("api/project-invitations")]
public class ProjectInvitationsController : ControllerBase
{
    private readonly IProjectInvitationService _service;

    public ProjectInvitationsController(IProjectInvitationService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProjectInvitationDto dto)
    {
        var inviterId = int.Parse(User.FindFirst("Id")?.Value ?? "0");
        var result = await _service.CreateAsync(dto, inviterId);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPost("accept")]
    public async Task<IActionResult> AcceptOrDeny([FromBody] AcceptProjectInvitationDto dto)
    {
        var userId = int.Parse(User.FindFirst("Id")?.Value ?? "0");
        var result = await _service.AcceptOrDenyAsync(dto, userId);
        return Ok(result);
    }

    [HttpGet("project/{projectId}")]
    public async Task<IActionResult> GetForProject(int projectId, int pageIndex = 1, int pageSize = 10)
    {
        var result = await _service.GetForProjectAsync(projectId, pageIndex, pageSize);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Cancel(int id)
    {
        var userId = int.Parse(User.FindFirst("Id")?.Value ?? "0");
        var success = await _service.CancelAsync(id, userId);
        return success ? NoContent() : NotFound();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result == null ? NotFound() : Ok(result);
    }
}