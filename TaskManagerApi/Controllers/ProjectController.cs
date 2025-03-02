using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagerApi.Models;
using TaskManagerApi.Services.Interfaces;

namespace TaskManagerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController(IProjectService projectService) : ControllerBase
    {
        [HttpGet("all")]
        public async Task<ActionResult<List<ProjectItemDto>>> GetAllProjectsList()
        {
            return Ok(await projectService.GetProjectsAsync());
        }

        [HttpGet("{Id}")]
        public async Task<ActionResult<ProjectItemDto>> GetProjectById(Guid Id)
        {
            var project = await projectService.GetProjectById(Id);

            if (project is null)
                return BadRequest();

            return Ok(project);
        }

        [HttpPost("create")]
        public async Task<ActionResult<ProjectItemDto>> CreateProject(ProjectItemDto newProject)
        {
            var project = await projectService.CreateProjectAsync(newProject);

            if (project is null)
                return BadRequest();

            return Ok(project);
        }

        [HttpPut("edit/{Id}")]
        public async Task<ActionResult<ProjectItemDto>> EditProjectById(Guid Id, [FromBody] ProjectItemDto editProject)
        {
            var project = await projectService.EditProjectAsync(Id, editProject);

            if (project is null)
                return BadRequest();

            return Ok(project);
        }

        [HttpPut("edit/{projectId}/owner/{ownerId}")]
        public async Task<ActionResult<ProjectItemDto>> ChangeTaskAssigneeById(Guid projectId, Guid ownerId)
        {
            var project = await projectService.ChangeOwnerAsync(projectId, ownerId);

            if (project is null)
                return BadRequest();

            return Ok(project);
        }

        [HttpDelete("delete/{Id}")]
        public async Task<ActionResult<ProjectItemDto>> DeleteProject(Guid Id)
        {
            var project = await projectService.DeleteProjectAsync(Id);

            if (project is null)
                return BadRequest();

            return Ok(project);
        }
    }
}
