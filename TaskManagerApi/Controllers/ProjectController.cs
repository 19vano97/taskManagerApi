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
        private TaskManagerAPIDbContext _context;

        public ProjectController(IProjectService projectService,
                                 IProjectStatusesService projectStatuses,
                                 ITaskItemService taskItemService,
                                 TaskManagerAPIDbContext context)
        {
            _projectService = projectService;
            _projectStatuses = projectStatuses;
            _taskItemService = taskItemService;
            _context = context;
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<ProjectItemDto>>> GetAllProjectsListAsync()
        {
            if(!this.Request.Headers.TryGetValue("organizationId", out var organizationId)
                || !Guid.TryParse(organizationId, out var organizationIdGuid)) 
                return BadRequest("Invalid request");

            return Ok(await _projectService.GetProjectsAsync(organizationIdGuid));
        }

        [HttpGet("all/tasks")]
        public async Task<ActionResult<List<ProjectItemWithTasksDto>>> GetAllProjectsWithTasksListAsync()
        {
            if(!this.Request.Headers.TryGetValue("organizationId", out var organizationId)
                || !Guid.TryParse(organizationId, out var organizationIdGuid)) 
                return BadRequest("Invalid request");

            var projects = await _projectService.GetProjectsAsync(organizationIdGuid);
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
            if(!this.Request.Headers.TryGetValue("organizationId", out var organizationId) 
                || !await ValidateAccountOrganizationConnectionAsync(organizationId!, projectId)) 
                return BadRequest("Invalid request");

            var project = await _projectService.GetProjectByIdAsync(projectId);

            if (project is null)
                return BadRequest();

            return Ok(project);
        }

        [HttpGet("{projectId}/tasks")]
        public async Task<ActionResult<ProjectItemWithTasksDto>> GetProjectWithTasksByIdAsync(Guid projectId)
        {
            if(!this.Request.Headers.TryGetValue("organizationId", out var organizationId) 
                || !await ValidateAccountOrganizationConnectionAsync(organizationId!, projectId)) 
                return BadRequest("Invalid request");

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
            if(!this.Request.Headers.TryGetValue("organizationId", out var organizationId) 
                || !await ValidateAccountOrganizationConnectionAsync(organizationId!, projectId)) 
                return BadRequest("Invalid request");

            var project = await _projectService.GetAccountsByProjectId(projectId);

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

            var project = await _projectService.CreateProjectAsync(newProject);

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

            var project = await _projectService.EditProjectAsync(editProject);

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

            var project = await _projectService.ChangeOwnerAsync(projectId, ownerId);

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

            var project = await _projectService.DeleteProjectAsync(Id);

            if (project is null)
                return BadRequest();

            return Ok(project);
        }

        private async Task<bool> ValidateAccountOrganizationConnectionAsync(string organizationId, Guid projectId)
        {
            return Guid.TryParse(organizationId, out var organizationIdGuid)
                && await GeneralService.VerifyAccountRelatesToOrganization(_context, Guid.Parse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value)
                    , organizationIdGuid) is not null
                && await GeneralService.VerifyProjectInOrganization(_context, projectId, organizationIdGuid) is not null;
        }
    }
}
