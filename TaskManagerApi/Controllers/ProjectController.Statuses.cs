using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagerApi.Models.TaskItemStatuses;

namespace TaskManagerApi.Controllers
{
    public partial class ProjectController : ControllerBase
    {
        [HttpPost("statuses/add")]
        public async Task<ActionResult<TaskItemStatusProjectSingleDto>> CreateNewTaskStatusAsync(TaskItemStatusProjectSingleDto status)
        {
            if(!this.Request.Headers.TryGetValue("organizationId", out var organizationId) 
                || !await ValidateAccountOrganizationConnectionAsync(organizationId!, status.ProjectId)) 
                return BadRequest("Invalid request");

            return status;
        }
        [HttpPost("statuses/add-all")]
        public async Task<ActionResult<TaskItemAllStatusesProjectDto>> SyncAllTaskStatusesAsync(TaskItemAllStatusesProjectDto statuses)
        {
            if(!this.Request.Headers.TryGetValue("organizationId", out var organizationId) 
                || !await ValidateAccountOrganizationConnectionAsync(organizationId!, statuses.ProjectId)) 
                return BadRequest("Invalid request");

            return statuses;
        }

        [HttpDelete("statuses/delete")]
        public async Task<ActionResult<TaskItemStatusProjectSingleDto>> DeleteTaskStatusAsync(TaskItemStatusProjectSingleDto status)
        {
            if(!this.Request.Headers.TryGetValue("organizationId", out var organizationId) 
                || !await ValidateAccountOrganizationConnectionAsync(organizationId!, status.ProjectId)) 
                return BadRequest("Invalid request");

            return status;
        }
    }
}
