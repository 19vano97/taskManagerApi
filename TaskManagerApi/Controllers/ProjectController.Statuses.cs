using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagerApi.Models.TaskItemStatuses;

namespace TaskManagerApi.Controllers
{
    public partial class ProjectController : ControllerBase
    {
        [HttpPost("statuses/add")]
        public async Task<ActionResult<ProjectSingleStatusDto>> CreateNewTaskStatusAsync(ProjectSingleStatusDto status)
        {
            if(!this.Request.Headers.TryGetValue("organizationId", out var organizationId) 
                || !await ValidateAccountOrganizationConnectionAsync(organizationId!, status.ProjectId)) 
                return BadRequest("Invalid request");

            var statusToAdd = await _projectStatuses.AddAsync(status);

            if (statusToAdd is null)
            {
                BadRequest("Invalid request");
            }

            return statusToAdd;
        }
        // [HttpPost("statuses/add-all")]
        // public async Task<ActionResult<TaskItemAllStatusesProjectDto>> SyncAllTaskStatusesAsync(TaskItemAllStatusesProjectDto statuses)
        // {
        //     if(!this.Request.Headers.TryGetValue("organizationId", out var organizationId) 
        //         || !await ValidateAccountOrganizationConnectionAsync(organizationId!, statuses.ProjectId)) 
        //         return BadRequest("Invalid request");

        //     return statuses;
        // }

        [HttpDelete("statuses/delete")]
        public async Task<ActionResult<ProjectSingleStatusDto>> DeleteTaskStatusAsync(ProjectSingleStatusDto status)
        {
            if(!this.Request.Headers.TryGetValue("organizationId", out var organizationId) 
                || !await ValidateAccountOrganizationConnectionAsync(organizationId!, status.ProjectId)) 
                return BadRequest("Invalid request");

            var statusToDelete = await _projectStatuses.DeleteAsync(status);

            if (statusToDelete is null)
            {
                BadRequest("Invalid request");
            }

            return statusToDelete;
        }
    }
}
