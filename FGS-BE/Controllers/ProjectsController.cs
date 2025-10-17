using FGS_BE.Repo.DTOs.Projects;
using FGS_BE.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FGS_BE.API.Controllers
{
    [ApiController]
    [Route("api/projects")]
    public class ProjectsController: ControllerBase
    {
        private readonly IProjectService _projectService;
        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        /// <summary>
        /// get-all danh sách project
        /// </summary>
        /// <param name="pageIndex">Số trang hiện tại</param>
        /// <param name="pageSize">Số lượng bản ghi mỗi trang</param>
        /// <param name="keyword">Search theo Title</param>
        /// <param name="status">Search theo Status</param>
        /// <param name="sortColumn">Tên cột cần sắp xếp</param>
        /// <param name="sortDir">Chiều sắp Asc (tăng), Desc (giảm)</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageIndex = 1, 
            [FromQuery] int pageSize = 10, 
            [FromQuery] string? keyword = null, 
            [FromQuery] string? status = null,
            [FromQuery] string? sortColumn = "Id", 
            [FromQuery] string? sortDir = "Asc")
        {
            var result = await _projectService.GetPagedAsync(
                pageIndex, 
                pageSize, 
                keyword, 
                status, 
                sortColumn, 
                sortDir);
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
            var result = await _projectService.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// add project
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(CreateProjectDto dto)
        {
            var result = await _projectService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// update project
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateProjectDto dto)
        {
            var result = await _projectService.UpdateAsync(id, dto);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// delete project
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _projectService.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }
    }
}
