using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagerApi.Data;
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
    public partial class ProjectController(IProjectService projectService, TaskManagerAPIDbContext context) : ControllerBase
    {
        [HttpGet("all")]
        public async Task<ActionResult<List<ProjectItemDto>>> GetAllProjectsListAsync()
        {
            if(!this.Request.Headers.TryGetValue("organizationId", out var organizationId)
                || !Guid.TryParse(organizationId, out var organizationIdGuid)) 
                return BadRequest("Invalid request");

            return Ok(await projectService.GetProjectsAsync(organizationIdGuid));
        }

        [HttpGet("{Id}")]
        public async Task<ActionResult<ProjectItemDto>> GetProjectByIdAsync(Guid Id)
        {
            if(!this.Request.Headers.TryGetValue("organizationId", out var organizationId) 
                || !await ValidateAccountOrganizationConnectionAsync(organizationId!, Id)) 
                return BadRequest("Invalid request");

            var project = await projectService.GetProjectByIdAsync(Id);

            if (project is null)
                return BadRequest();

            return Ok(project);
        }

        [HttpPost("create")]
        public async Task<ActionResult<ProjectItemDto>> CreateProjectAsync(ProjectItemDto newProject)
        {
            if(!this.Request.Headers.TryGetValue("organizationId", out var organizationId) || !Guid.TryParse(organizationId, out var organizationIdGuid))
                return BadRequest("Invalid request");

            newProject.OrganizationId = organizationIdGuid;
            newProject.OwnerId = Guid.Parse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value);

            var project = await projectService.CreateProjectAsync(newProject);

            if (project is null)
                return BadRequest();

            return Ok(project);
        }

        [HttpPut("edit/")]
        public async Task<ActionResult<ProjectItemDto>> EditProjectByIdAsync([FromBody] ProjectItemDto editProject)
        {
            if(!this.Request.Headers.TryGetValue("organizationId", out var organizationId) 
                || !await ValidateAccountOrganizationConnectionAsync(organizationId!, editProject.Id)) 
                return BadRequest("Invalid request");

            var project = await projectService.EditProjectAsync(editProject);

            if (project is null)
                return BadRequest();

            return Ok(project);
        }

        [HttpPut("edit/{projectId}/owner/{ownerId}")]
        public async Task<ActionResult<ProjectItemDto>> ChangeProjectOwnerByIdAsync(Guid projectId, Guid ownerId)
        {
            if(!this.Request.Headers.TryGetValue("organizationId", out var organizationId) 
                || !await ValidateAccountOrganizationConnectionAsync(organizationId!, projectId)) 
                return BadRequest("Invalid request");

            var project = await projectService.ChangeOwnerAsync(projectId, ownerId);

            if (project is null)
                return BadRequest();

            return Ok(project);
        }

        [HttpDelete("delete/{Id}")]
        public async Task<ActionResult<ProjectItemDto>> DeleteProjectAsync(Guid Id)
        {
            if(!this.Request.Headers.TryGetValue("organizationId", out var organizationId) 
                || !await ValidateAccountOrganizationConnectionAsync(organizationId!, Id)) 
                return BadRequest("Invalid request");

            var project = await projectService.DeleteProjectAsync(Id);

            if (project is null)
                return BadRequest();

            return Ok(project);
        }

        private async Task<bool> ValidateAccountOrganizationConnectionAsync(string organizationId, Guid projectId)
        {
            return (!Guid.TryParse(organizationId, out var organizationIdGuid)
                || await GeneralService.VerifyAccountRelatesToOrganization(context, Guid.Parse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value)
                    , organizationIdGuid) is null
                || await GeneralService.VerifyProjectInOrganization(context, projectId, organizationIdGuid) is null);
        }
    }
}
