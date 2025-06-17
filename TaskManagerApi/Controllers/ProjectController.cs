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
        private IProjectStatusesService _projectStatuses;
        private ITaskItemService _taskItemService;
        private IAccountVerification _accountVerification;
        private TaskManagerAPIDbContext _context;

        public ProjectController(IProjectService projectService,
                                 IProjectStatusesService projectStatuses,
                                 ITaskItemService taskItemService,
                                 IAccountVerification accountVerification,
                                 TaskManagerAPIDbContext context)
        {
            _projectService = projectService;
            _projectStatuses = projectStatuses;
            _taskItemService = taskItemService;
            _accountVerification = accountVerification;
            _context = context;
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<ProjectItemDto>>> GetAllProjectsListAsync()
        {
            if (!this.Request.Headers.TryGetValue("organizationId", out var organizationIdString)
                || !Guid.TryParse(organizationIdString, out Guid organizationId)
                || !Guid.TryParse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value, out Guid accountId)
                || !await _accountVerification.VerifyAccountInOrganization(accountId, organizationId))
                return BadRequest();

            return Ok(await _projectService.GetProjectsAsync(organizationId));
        }

        [HttpGet("all/tasks")]
        public async Task<ActionResult<List<ProjectItemWithTasksDto>>> GetAllProjectsWithTasksListAsync()
        {
            if (!this.Request.Headers.TryGetValue("organizationId", out var organizationIdString)
                || !Guid.TryParse(organizationIdString, out Guid organizationId)
                || !Guid.TryParse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value, out Guid accountId)
                || !await _accountVerification.VerifyAccountInOrganization(accountId, organizationId))
                return BadRequest();

            var projects = await _projectService.GetProjectsAsync(organizationId);
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

        [HttpGet("{projectId}/accounts")]
        public async Task<ActionResult<ProjectAccountsDto>> GetAccountsByProjectId(Guid projectId)
        {
            if (!Guid.TryParse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value, out Guid accountId)
                || !await _accountVerification.VerifyAccountInOrganizationByProject(accountId, projectId))
                return BadRequest();

            var project = await _projectService.GetAccountsByProjectId(projectId);

            if (project is null)
                return BadRequest();

            return Ok(project);
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

        [HttpPut("edit")]
        public async Task<ActionResult<ProjectItemDto>> EditProjectByIdAsync([FromBody] ProjectItemDto editProject)
        {
            if (!Guid.TryParse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value, out Guid accountId)
                || !await _accountVerification.VerifyAccountInOrganization(accountId, editProject.OrganizationId))
                return BadRequest();

            var project = await _projectService.EditProjectAsync(editProject);

            if (project is null)
                return BadRequest();

            return Ok(project);
        }

        [HttpDelete("delete/{Id}")]
        public async Task<ActionResult<ProjectItemDto>> DeleteProjectAsync(Guid projectId)
        {
            if (!Guid.TryParse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value, out Guid accountId)
                || !await _accountVerification.VerifyAccountInOrganizationByProject(accountId, projectId))
                return BadRequest();

            var project = await _projectService.DeleteProjectAsync(projectId);

            if (project is null)
                return BadRequest();

            return Ok(project);
        }
    }
}
