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
    public class ProjectController : ControllerBase
    {
        private IProjectService _projectService;
        private ITicketService _ticketItemService;
        private IAccountVerification _accountVerification;

        public ProjectController(IProjectService projectService,
                                 ITicketService ticketItemService,
                                 IAccountVerification accountVerification)
        {
            _projectService = projectService;
            _ticketItemService = ticketItemService;
            _accountVerification = accountVerification;
        }

        [HttpGet("/all/{organizationId}")]
        public async Task<ActionResult<List<ProjectItemWithTasksDto>>> GetAllProjectsWithTasksListAsync(Guid organizationId, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value, out Guid accountId)
                || !await _accountVerification.VerifyAccountInOrganization(accountId, organizationId, cancellationToken))
                return BadRequest();

            var projects = await _projectService.GetProjectsByOrganizationIdAsync(organizationId, cancellationToken);
            var projectList = new List<ProjectItemWithTasksDto>();

            foreach (var item in projects.Data)
            {
                var res = await _ticketItemService.GetTasksByProjectAsync(item.Id, cancellationToken);
                if (!res.Success) continue;

                projectList.Add(new ProjectItemWithTasksDto 
                { 
                    Project = item, 
                    Tasks = res.Data!
                });
            }

            if (projectList.Count == 0) return NotFound();

            return Ok(projectList);
        }

        [HttpGet("{projectId}")]
        public async Task<ActionResult<ProjectItemWithTasksDto>> GetProjectByIdAsync(Guid projectId, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value, out Guid accountId)
                || !await _accountVerification.VerifyAccountInOrganizationByProject(accountId, projectId, cancellationToken))
                return BadRequest();

            var project = await _projectService.GetProjectByIdAsync(projectId, cancellationToken);

            if (!project.Success)
            {
                if (project.ErrorMessage == LogPhrases.ServiceResult.Error.NOT_FOUND)
                {
                    return NotFound();
                }

                return BadRequest(project.ErrorMessage);
            }

            return Ok(project.Data);
        }

        [HttpGet("{projectId}/tasks")]
        public async Task<ActionResult<ProjectItemWithTasksDto>> GetProjectWithTasksByIdAsync(Guid projectId, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value, out Guid accountId)
                || !await _accountVerification.VerifyAccountInOrganizationByProject(accountId, projectId, cancellationToken))
                return BadRequest();

            var project = await _projectService.GetProjectByIdAsync(projectId, cancellationToken);

            if (!project.Success)
            {
                if (project.ErrorMessage == LogPhrases.ServiceResult.Error.NOT_FOUND)
                {
                    return NotFound();
                }

                return BadRequest(project.ErrorMessage);
            }

            var tasks = await _ticketItemService.GetTasksByProjectAsync(project.Data.Id, cancellationToken);
            if (!tasks.Success) return Ok(new ProjectItemWithTasksDto { Project = project.Data });

            return Ok(new ProjectItemWithTasksDto
            {
                Project = project.Data,
                Tasks = tasks.Data
            });
        }

        [HttpPost("create")]
        public async Task<ActionResult<ProjectItemDto>> CreateProjectAsync(ProjectItemDto newProject,
                                                                           CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value, out Guid accountId)
                || !await _accountVerification.VerifyAccountInOrganization(accountId, newProject.OrganizationId, cancellationToken))
                return BadRequest();

            newProject.OrganizationId = newProject.OrganizationId;
            newProject.OwnerId = Guid.Parse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value);

            var project = await _projectService.CreateProjectAsync(newProject, cancellationToken);

            if (!project.Success)
            {
                if (project.ErrorMessage == LogPhrases.ServiceResult.Error.NOT_FOUND)
                {
                    return NotFound();
                }

                return BadRequest(project.ErrorMessage);
            }

            return Ok(project.Data);
        }

        [HttpPost("{projectId}/edit")]
        public async Task<ActionResult<ProjectItemDto>> EditProjectByIdAsync([FromBody] ProjectItemDto editProject,
                                                                             Guid projectId,
                                                                             CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value, out Guid accountId)
                || projectId != editProject.Id
                || !await _accountVerification.VerifyAccountInOrganizationByProject(accountId, projectId, cancellationToken))
                return BadRequest();

            var project = await _projectService.EditProjectAsync(editProject, cancellationToken);

            if (!project.Success)
            {
                if (project.ErrorMessage == LogPhrases.ServiceResult.Error.NOT_FOUND)
                {
                    return NotFound();
                }

                return BadRequest(project.ErrorMessage);
            }

            return Ok(project.Data);
        }

        [HttpDelete("{projectId}/delete")]
        public async Task<ActionResult> DeleteProjectAsync(Guid projectId, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value, out Guid accountId)
                || !await _accountVerification.VerifyAccountInOrganizationByProject(accountId, projectId, cancellationToken))
                return BadRequest();

            var project = await _projectService.DeleteProjectAsync(projectId, cancellationToken);

            if (!project.Success)
            {
                if (project.ErrorMessage == LogPhrases.ServiceResult.Error.NOT_FOUND)
                {
                    return NotFound();
                }

                return BadRequest(project.ErrorMessage);
            }

            return Ok();
        }
    }
}
