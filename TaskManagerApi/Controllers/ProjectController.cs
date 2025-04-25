using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagerApi.Models;
using TaskManagerApi.Models.Project;
using TaskManagerApi.Services.Implementations;
using TaskManagerApi.Services.Interfaces;
using static TaskManagerApi.Models.Constants;

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
            if(!this.Request.Headers.TryGetValue("organizationId", out var organizationId)
                || Guid.TryParse(organizationId, out var organizationIdGuid)) 
                return BadRequest("Invalid request");

            return Ok(await projectService.GetProjectsAsync(organizationIdGuid));
        }

        [HttpGet("{Id}")]
        public async Task<ActionResult<ProjectItemDto>> GetProjectById(Guid Id)
        {
            if(!this.Request.Headers.TryGetValue("organizationId", out var organizationId) 
                || !await ValidateAccountOrganizationConnection(organizationId!, Id)) 
                return BadRequest("Invalid request");

            var project = await projectService.GetProjectByIdAsync(Id);

            if (project is null)
                return BadRequest();

            return Ok(project);
        }

        [HttpPost("create")]
        public async Task<ActionResult<ProjectItemDto>> CreateProject(ProjectItemDto newProject)
        {
            if(!this.Request.Headers.TryGetValue("organizationId", out var organizationId))
                return BadRequest("Invalid request");

            var project = await projectService.CreateProjectAsync(newProject, Guid.Parse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value));

            if (project is null)
                return BadRequest();

            return Ok(project);
        }

        [HttpPut("edit/")]
        public async Task<ActionResult<ProjectItemDto>> EditProjectById([FromBody] ProjectItemDto editProject)
        {
            if(!this.Request.Headers.TryGetValue("organizationId", out var organizationId) 
                || !await ValidateAccountOrganizationConnection(organizationId!, editProject.Id)) 
                return BadRequest("Invalid request");

            var project = await projectService.EditProjectAsync(editProject);

            if (project is null)
                return BadRequest();

            return Ok(project);
        }

        [HttpPut("edit/{projectId}/owner/{ownerId}")]
        public async Task<ActionResult<ProjectItemDto>> ChangeProjectOwnerById(Guid projectId, Guid ownerId)
        {
            if(!this.Request.Headers.TryGetValue("organizationId", out var organizationId) 
                || !await ValidateAccountOrganizationConnection(organizationId!, projectId)) 
                return BadRequest("Invalid request");

            var project = await projectService.ChangeOwnerAsync(projectId, ownerId);

            if (project is null)
                return BadRequest();

            return Ok(project);
        }

        [HttpDelete("delete/{Id}")]
        public async Task<ActionResult<ProjectItemDto>> DeleteProject(Guid Id)
        {
            if(!this.Request.Headers.TryGetValue("organizationId", out var organizationId) 
                || !await ValidateAccountOrganizationConnection(organizationId!, Id)) 
                return BadRequest("Invalid request");

            var project = await projectService.DeleteProjectAsync(Id);

            if (project is null)
                return BadRequest();

            return Ok(project);
        }

        private async Task<bool> ValidateAccountOrganizationConnection(string organizationId, Guid projectId)
        {
            return (!Guid.TryParse(organizationId, out var organizationIdGuid)
                || await GeneralService.VerifyAccountRelatesToOrganization(Guid.Parse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value)
                    , organizationIdGuid) is null
                || await GeneralService.VerifyProjectInOrganization(projectId, organizationIdGuid) is null);
        }
    }
}
