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
    /// <summary>
    /// API controller for managing projects and their related tasks.
    /// </summary>
    public class ProjectController : ControllerBase
    {
        private IProjectService _projectService;
        private ITicketService _ticketItemService;
        private IAccountVerification _accountVerification;
        private ILogger<ProjectController> _logger;

        public ProjectController(IProjectService projectService,
                                 ITicketService ticketItemService,
                                 IAccountVerification accountVerification,
                                 ILogger<ProjectController> logger)
        {
            _projectService = projectService;
            _ticketItemService = ticketItemService;
            _accountVerification = accountVerification;
            _logger = logger;
        }

        /// <summary>
        /// Gets all projects with their tasks for a given organization.
        /// </summary>
        /// <param name="organizationId">The organization ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of projects with tasks.</returns>
        [HttpGet("/all/{organizationId}")]
        public async Task<ActionResult<List<ProjectItemDto>>> GetAllProjectsWithTasksListAsync(Guid organizationId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetAllProjectsWithTasksListAsync called for organizationId: {OrgId}", organizationId);
            if (!Guid.TryParse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value, out Guid accountId)
                || !await _accountVerification.VerifyAccountInOrganization(accountId, organizationId, cancellationToken))
            {
                _logger.LogWarning("Account verification failed for GetAllProjectsWithTasksListAsync. organizationId: {OrgId}", organizationId);
                return BadRequest();
            }

            var projects = await _projectService.GetProjectsByOrganizationIdAsync(organizationId, cancellationToken);
            var projectList = new List<ProjectItemDto>();

            foreach (var item in projects.Data)
            {
                var res = await _ticketItemService.GetTasksByProjectAsync(item.Id, cancellationToken);
                if (!res.IsSuccess) continue;

                item.Tickets = res.Data;
                projectList.Add(item);
            }

            if (projectList.Count == 0)
            {
                _logger.LogWarning("No projects with tasks found for organizationId: {OrgId}", organizationId);
                return NotFound();
            }

            _logger.LogInformation("Projects with tasks fetched for organizationId: {OrgId}", organizationId);
            return Ok(projectList);
        }

        /// <summary>
        /// Gets a project by its ID.
        /// </summary>
        /// <param name="projectId">The project ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The project with its details.</returns>
        [HttpGet("{projectId}")]
        public async Task<ActionResult<ProjectItemDto>> GetProjectByIdAsync(Guid projectId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetProjectByIdAsync called for projectId: {ProjectId}", projectId);
            if (!Guid.TryParse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value, out Guid accountId)
                || !await _accountVerification.VerifyAccountInOrganizationByProject(accountId, projectId, cancellationToken))
            {
                _logger.LogWarning("Account verification failed for GetProjectByIdAsync. projectId: {ProjectId}", projectId);
                return BadRequest();
            }

            var project = await _projectService.GetProjectByIdAsync(projectId, cancellationToken);

            if (!project.IsSuccess)
            {
                _logger.LogWarning("Failed to get project by id: {ProjectId}. Error: {Error}", projectId, project.ErrorMessage);
                if (project.ErrorMessage == LogPhrases.ServiceResult.Error.NOT_FOUND)
                {
                    return NotFound();
                }

                return BadRequest(project.ErrorMessage);
            }

            _logger.LogInformation("Project fetched successfully. Id: {ProjectId}", projectId);
            return Ok(project.Data);
        }

        /// <summary>
        /// Gets a project and its tasks by project ID.
        /// </summary>
        /// <param name="projectId">The project ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The project and its tasks.</returns>
        [HttpGet("{projectId}/tasks")]
        public async Task<ActionResult<ProjectItemDto>> GetProjectWithTasksByIdAsync(Guid projectId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetProjectWithTasksByIdAsync called for projectId: {ProjectId}", projectId);
            if (!Guid.TryParse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value, out Guid accountId)
                || !await _accountVerification.VerifyAccountInOrganizationByProject(accountId, projectId, cancellationToken))
            {
                _logger.LogWarning("Account verification failed for GetProjectWithTasksByIdAsync. projectId: {ProjectId}", projectId);
                return BadRequest();
            }

            var project = await _projectService.GetProjectByIdAsync(projectId, cancellationToken);

            if (!project.IsSuccess)
            {
                _logger.LogWarning("Failed to get project by id: {ProjectId}. Error: {Error}", projectId, project.ErrorMessage);
                if (project.ErrorMessage == LogPhrases.ServiceResult.Error.NOT_FOUND)
                {
                    return NotFound();
                }

                return BadRequest(project.ErrorMessage);
            }

            var tasks = await _ticketItemService.GetTasksByProjectAsync(project.Data.Id, cancellationToken);
            if (!tasks.IsSuccess)
            {
                _logger.LogWarning("Failed to get tasks for projectId: {ProjectId}", projectId);
                return Ok(project.Data);
            }

            project.Data.Tickets = tasks.Data;

            _logger.LogInformation("Project with tasks fetched successfully. Id: {ProjectId}", projectId);
            return Ok(project.Data);
        }

        /// <summary>
        /// Creates a new project.
        /// </summary>
        /// <param name="newProject">The project data to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created project.</returns>
        [HttpPost("create")]
        public async Task<ActionResult<ProjectItemDto>> CreateProjectAsync(ProjectItemDto newProject,
                                                                           CancellationToken cancellationToken)
        {
            _logger.LogInformation("CreateProjectAsync called");
            if (!Guid.TryParse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value, out Guid accountId)
                || !await _accountVerification.VerifyAccountInOrganization(accountId, newProject.OrganizationId, cancellationToken))
            {
                _logger.LogWarning("Account verification failed for CreateProjectAsync. organizationId: {OrgId}", newProject.OrganizationId);
                return BadRequest();
            }

            newProject.OrganizationId = newProject.OrganizationId;
            newProject.OwnerId = Guid.Parse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value);

            var project = await _projectService.CreateProjectAsync(newProject, cancellationToken);

            if (!project.IsSuccess)
            {
                _logger.LogWarning("Failed to create project. Error: {Error}", project.ErrorMessage);
                if (project.ErrorMessage == LogPhrases.ServiceResult.Error.NOT_FOUND)
                {
                    return NotFound();
                }

                return BadRequest(project.ErrorMessage);
            }

            _logger.LogInformation("Project created successfully. Id: {ProjectId}", project.Data?.Id);
            return Ok(project.Data);
        }

        /// <summary>
        /// Edits an existing project by its ID.
        /// </summary>
        /// <param name="editProject">The updated project data.</param>
        /// <param name="projectId">The project ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated project.</returns>
        [HttpPost("{projectId}/edit")]
        public async Task<ActionResult<ProjectItemDto>> EditProjectByIdAsync([FromBody] ProjectItemDto editProject,
                                                                             Guid projectId,
                                                                             CancellationToken cancellationToken)
        {
            _logger.LogInformation("EditProjectByIdAsync called for projectId: {ProjectId}", projectId);
            if (!Guid.TryParse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value, out Guid accountId)
                || projectId != editProject.Id
                || !await _accountVerification.VerifyAccountInOrganizationByProject(accountId, projectId, cancellationToken))
            {
                _logger.LogWarning("Account verification failed or projectId mismatch for EditProjectByIdAsync. projectId: {ProjectId}", projectId);
                return BadRequest();
            }

            var project = await _projectService.EditProjectAsync(editProject, cancellationToken);

            if (!project.IsSuccess)
            {
                _logger.LogWarning("Failed to edit project. projectId: {ProjectId}. Error: {Error}", projectId, project.ErrorMessage);
                if (project.ErrorMessage == LogPhrases.ServiceResult.Error.NOT_FOUND)
                {
                    return NotFound();
                }

                return BadRequest(project.ErrorMessage);
            }

            _logger.LogInformation("Project edited successfully. Id: {ProjectId}", projectId);
            return Ok(project.Data);
        }

        /// <summary>
        /// Deletes a project by its ID.
        /// </summary>
        /// <param name="projectId">The project ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>No content if successful.</returns>
        [HttpDelete("{projectId}/delete")]
        public async Task<ActionResult> DeleteProjectAsync(Guid projectId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("DeleteProjectAsync called for projectId: {ProjectId}", projectId);
            if (!Guid.TryParse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value, out Guid accountId)
                || !await _accountVerification.VerifyAccountInOrganizationByProject(accountId, projectId, cancellationToken))
            {
                _logger.LogWarning("Account verification failed for DeleteProjectAsync. projectId: {ProjectId}", projectId);
                return BadRequest();
            }

            var project = await _projectService.DeleteProjectAsync(projectId, cancellationToken);

            if (!project.IsSuccess)
            {
                _logger.LogWarning("Failed to delete project. projectId: {ProjectId}. Error: {Error}", projectId, project.ErrorMessage);
                if (project.ErrorMessage == LogPhrases.ServiceResult.Error.NOT_FOUND)
                {
                    return NotFound();
                }

                return BadRequest(project.ErrorMessage);
            }

            _logger.LogInformation("Project deleted successfully. Id: {ProjectId}", projectId);
            return Ok();
        }
    }
}
