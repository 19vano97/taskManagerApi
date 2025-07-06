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
    public partial class ProjectController : ControllerBase
    {
        private IProjectService _projectService;
        private ITicketService _taskItemService;
        private IAccountVerification _accountVerification;

        public ProjectController(IProjectService projectService,
                                 ITicketService taskItemService,
                                 IAccountVerification accountVerification)
        {
            _projectService = projectService;
            _taskItemService = taskItemService;
            _accountVerification = accountVerification;
        }

        [HttpGet("/all/{organizationId}")]
        public async Task<ActionResult<List<ProjectItemWithTasksDto>>> GetAllProjectsWithTasksListAsync(Guid organizationId)
        {
            if (!Guid.TryParse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value, out Guid accountId)
                || !await _accountVerification.VerifyAccountInOrganization(accountId, organizationId))
                return BadRequest();

            var projects = await _projectService.GetProjectsByOrganizationIdAsync(organizationId);
            var projectList = new List<ProjectItemWithTasksDto>();

            foreach (var item in projects)
            {
                projectList.Add(new ProjectItemWithTasksDto 
                { 
                    Project = item, 
                    Tasks = await _taskItemService.GetTasksByProjectAsync(item.Id)
                });
            }

            return Ok(projectList);
        }

        [HttpGet("{projectId}")]
        public async Task<ActionResult<ProjectItemWithTasksDto>> GetProjectByIdAsync(Guid projectId)
        {
            if (!Guid.TryParse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value, out Guid accountId)
                || !await _accountVerification.VerifyAccountInOrganizationByProject(accountId, projectId))
                return BadRequest();

            var project = await _projectService.GetProjectByIdAsync(projectId);

            if (project is null)
                return BadRequest();

            return Ok(project);
        }

        [HttpGet("{projectId}/tasks")]
        public async Task<ActionResult<ProjectItemWithTasksDto>> GetProjectWithTasksByIdAsync(Guid projectId)
        {
            if (!Guid.TryParse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value, out Guid accountId)
                || !await _accountVerification.VerifyAccountInOrganizationByProject(accountId, projectId))
                return BadRequest();

            var project = await _projectService.GetProjectByIdAsync(projectId);

            if (project is null)
                return BadRequest();

            var tasks = await _taskItemService.GetTasksByProjectAsync(project.Id);

            return Ok(new ProjectItemWithTasksDto{
                Project = project,
                Tasks = tasks
            });
        }

        [HttpPost("create")]
        public async Task<ActionResult<ProjectItemDto>> CreateProjectAsync(ProjectItemDto newProject)
        {
            if (!Guid.TryParse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value, out Guid accountId)
                || !await _accountVerification.VerifyAccountInOrganization(accountId, newProject.OrganizationId))
                return BadRequest();

            newProject.OrganizationId = newProject.OrganizationId;
            newProject.OwnerId = Guid.Parse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value);

            var project = await _projectService.CreateProjectAsync(newProject);

            if (project is null)
                return BadRequest();

            return Ok(project);
        }

        [HttpPost("{projectId}/edit")]
        public async Task<ActionResult<ProjectItemDto>> EditProjectByIdAsync([FromBody] ProjectItemDto editProject, Guid projectId)
        {
            if (!Guid.TryParse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value, out Guid accountId)
                || projectId != editProject.Id
                || !await _accountVerification.VerifyAccountInOrganizationByProject(accountId, projectId))
                return BadRequest();

            var project = await _projectService.EditProjectAsync(editProject);

            if (project is null)
                return BadRequest();

            return Ok(project);
        }

        [HttpDelete("{projectId}/delete")]
        public async Task<ActionResult> DeleteProjectAsync(Guid projectId)
        {
            if (!Guid.TryParse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value, out Guid accountId)
                || !await _accountVerification.VerifyAccountInOrganizationByProject(accountId, projectId))
                return BadRequest();

            var project = await _projectService.DeleteProjectAsync(projectId);

            if (project is null)
                return BadRequest();

            return Ok();
        }
    }
}
