using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagerConvertor.Models.Project;
using TaskManagerConvertor.Services.Interfaces;

namespace TaskManagerConvertor.Controllers.TaskManager
{
    [Route("api/project")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly ILogger<ProjectController> _logger;

        public ProjectController(IProjectService projectService, ILogger<ProjectController> logger)
        {
            _logger = logger;
            _projectService = projectService;
        }

        [HttpGet("/all/{organizationId}")]
        public async Task<ActionResult<List<ProjectItemDto>>> GetAllProjectsWithTasksListAsync(Guid organizationId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetAllProjectsWithTasksListAsync called for organizationId: {OrgId}", organizationId);

            var response = await _projectService.GetAllProjectsWithTasksListAsync(Request.Headers, organizationId, cancellationToken);
            if (response.IsSuccess)
            {
                _logger.LogInformation(response.Data!.ToString());
                return Ok(response.Data!);
            }

            _logger.LogWarning(response.ErrorMessage);

            return BadRequest();
        }

        [HttpGet("{projectId}")]
        public async Task<ActionResult<ProjectItemDto>> GetProjectByIdAsync(Guid projectId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetProjectByIdAsync called for projectId: {ProjectId}", projectId);

            var response = await _projectService.GetProjectByIdAsync(Request.Headers, projectId, cancellationToken);
            if (response.IsSuccess)
            {
                _logger.LogInformation(response.Data!.ToString());
                return Ok(response.Data!);
            }

            _logger.LogWarning(response.ErrorMessage);

            return BadRequest();
        }

        [HttpGet("{projectId}/tasks")]
        public async Task<ActionResult<ProjectItemDto>> GetProjectWithTasksByIdAsync(Guid projectId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetProjectWithTasksByIdAsync called for projectId: {ProjectId}", projectId);

            var response = await _projectService.GetProjectWithTasksByIdAsync(Request.Headers, projectId, cancellationToken);
            if (response.IsSuccess)
            {
                _logger.LogInformation(response.Data!.ToString());
                return Ok(response.Data!);
            }

            _logger.LogWarning(response.ErrorMessage);

            return BadRequest();
        }

        [HttpPost("create")]
        public async Task<ActionResult<ProjectItemDto>> CreateProjectAsync(ProjectItemDto newProject,
                                                                           CancellationToken cancellationToken)
        {
            _logger.LogInformation("CreateProjectAsync called");

            var response = await _projectService.CreateProjectAsync(Request.Headers, newProject, cancellationToken);
            if (response.IsSuccess)
            {
                _logger.LogInformation(response.Data!.ToString());
                return Ok(response.Data!);
            }

            _logger.LogWarning(response.ErrorMessage);

            return BadRequest();
        }

        [HttpPost("{projectId}/edit")]
        public async Task<ActionResult<ProjectItemDto>> EditProjectByIdAsync([FromBody] ProjectItemDto editProject,
                                                                             Guid projectId,
                                                                             CancellationToken cancellationToken)
        {
            _logger.LogInformation("EditProjectByIdAsync called for projectId: {ProjectId}", projectId);

            var response = await _projectService.EditProjectByIdAsync(Request.Headers, editProject, projectId, cancellationToken);
            if (response.IsSuccess)
            {
                _logger.LogInformation(response.Data!.ToString());
                return Ok(response.Data!);
            }

            _logger.LogWarning(response.ErrorMessage);

            return BadRequest();
        }

        [HttpDelete("{projectId}/delete")]
        public async Task<ActionResult> DeleteProjectAsync(Guid projectId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("DeleteProjectAsync called for projectId: {ProjectId}", projectId);

            var response = await _projectService.DeleteProjectAsync(Request.Headers, projectId, cancellationToken);
            if (response.IsSuccess)
            {
                _logger.LogInformation(response.Data!.ToString());
                return Ok(response.Data!);
            }

            _logger.LogWarning(response.ErrorMessage);

            return BadRequest();
        }
    }
}
