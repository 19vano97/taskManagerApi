using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagerApi.Models;
using TaskManagerApi.Models.Project;
using TaskManagerApi.Services.Interfaces;

namespace TaskManagerApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/project")]
    [ApiController]
    public class ProjectController(IProjectService projectService) : ControllerBase
    {
        [HttpGet("all")]
        public async Task<ActionResult<List<ProjectItemDto>>> GetAllProjectsList()
        {
            if(!this.Request.Headers.TryGetValue("organizationId", out var organizationId)) 
                return BadRequest();

            if(!Guid.TryParse(organizationId, out var organizationIdGuid)) 
                return BadRequest();

            return Ok(await projectService.GetProjectsAsync(organizationIdGuid, User));
        }

        [HttpGet("{Id}")]
        public async Task<ActionResult<ProjectItemDto>> GetProjectById(Guid Id)
        {
            if(!this.Request.Headers.TryGetValue("organizationId", out var organizationId)) 
                return BadRequest();

            if(!Guid.TryParse(organizationId, out var organizationIdGuid)) 
                return BadRequest();

            var project = await projectService.GetProjectById(Id, organizationIdGuid, User);

            if (project is null)
                return BadRequest();

            return Ok(project);
        }

        [HttpPost("create")]
        public async Task<ActionResult<ProjectItemDto>> CreateProject(ProjectItemDto newProject)
        {
            if(!this.Request.Headers.TryGetValue("organizationId", out var organizationId)) 
                return BadRequest();

            if(!Guid.TryParse(organizationId, out var organizationIdGuid)) 
                return BadRequest();

            var project = await projectService.CreateProjectAsync(newProject, organizationIdGuid, User);

            if (project is null)
                return BadRequest();

            return Ok(project);
        }

        [HttpPut("edit/{Id}")]
        public async Task<ActionResult<ProjectItemDto>> EditProjectById(Guid Id, [FromBody] ProjectItemDto editProject)
        {
            if(!this.Request.Headers.TryGetValue("organizationId", out var organizationId)) 
                return BadRequest();

            if(!Guid.TryParse(organizationId, out var organizationIdGuid)) 
                return BadRequest();

            var project = await projectService.EditProjectAsync(Id, editProject, organizationIdGuid, User);

            if (project is null)
                return BadRequest();

            return Ok(project);
        }

        [HttpPut("edit/{projectId}/owner/{ownerId}")]
        public async Task<ActionResult<ProjectItemDto>> ChangeProjectOwnerById(Guid projectId, Guid ownerId)
        {
            if(!this.Request.Headers.TryGetValue("organizationId", out var organizationId)) 
                return BadRequest();

            if(!Guid.TryParse(organizationId, out var organizationIdGuid)) 
                return BadRequest();

            var project = await projectService.ChangeOwnerAsync(projectId, ownerId, organizationIdGuid, User);

            if (project is null)
                return BadRequest();

            return Ok(project);
        }

        [HttpDelete("delete/{Id}")]
        public async Task<ActionResult<ProjectItemDto>> DeleteProject(Guid Id)
        {
            if(!this.Request.Headers.TryGetValue("organizationId", out var organizationId)) 
                return BadRequest();

            if(!Guid.TryParse(organizationId, out var organizationIdGuid)) 
                return BadRequest();

            var project = await projectService.DeleteProjectAsync(Id, organizationIdGuid, User);

            if (project is null)
                return BadRequest();

            return Ok(project);
        }
    }
}
