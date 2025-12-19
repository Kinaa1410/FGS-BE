using FGS_BE.Repo.DTOs.ProjectInvitations;
using FGS_BE.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FGS_BE.API.Controllers
{
    [ApiController]
    [Route("api/project-invitations")]
    [Authorize]  // Global cho toàn controller—yêu cầu auth cho tất cả endpoints
    public class ProjectInvitationsController : ControllerBase
    {
        private readonly IProjectInvitationService _service;

        public ProjectInvitationsController(IProjectInvitationService service)
        {
            _service = service;
        }

        /// <summary>
        /// Tạo lời mời tham gia project
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProjectInvitationDto dto)
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int inviterId) || inviterId <= 0)
                return Unauthorized("Invalid user ID in token.");

            try
            {
                var result = await _service.CreateAsync(dto, inviterId);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Chấp nhận hoặc từ chối lời mời (qua DTO.Accept = true/false)
        /// </summary>
        [HttpPost("accept-or-deny")]  // Đổi tên cho rõ ràng hơn
        public async Task<IActionResult> AcceptOrDeny([FromBody] AcceptProjectInvitationDto dto)
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId) || userId <= 0)
                return Unauthorized("Invalid user ID in token.");

            try
            {
                var result = await _service.AcceptOrDenyAsync(dto, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);  // Ví dụ: "Invalid or expired invite."
            }
        }

        /// <summary>
        /// Lấy danh sách lời mời theo project (có phân trang)
        /// </summary>
        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetForProject(int projectId, int pageIndex = 1, int pageSize = 10)
        {
            // Optional: Check user là member của projectId để xem invites (thêm sau nếu cần)
            // if (!await CheckUserInProject(projectId)) return Forbid();

            var result = await _service.GetForProjectAsync(projectId, pageIndex, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Hủy lời mời (chỉ inviter mới hủy được)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Cancel(int id)
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId) || userId <= 0)
                return Unauthorized("Invalid user ID in token.");

            var success = await _service.CancelAsync(id, userId);
            return success ? NoContent() : NotFound("Invitation not found or not authorized to cancel.");
        }

        /// <summary>
        /// Lấy chi tiết lời mời theo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }
    }
}