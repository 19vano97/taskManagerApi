using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagerApi.Models.TaskItemStatuses;
using static TaskManagerApi.Models.Constants;

namespace TaskManagerApi.Controllers
{
    public partial class ProjectController : ControllerBase
    {
        [HttpPost("statuses/add")]
        public async Task<ActionResult<ProjectSingleStatusDto>> CreateNewTaskStatusAsync(ProjectSingleStatusDto status)
        {
            if (!Guid.TryParse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value, out Guid accountId)
                || !await _accountVerification.VerifyAccountInOrganizationByProject(accountId, status.ProjectId))
                return BadRequest();

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
            if (!Guid.TryParse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value, out Guid accountId)
                || !await _accountVerification.VerifyAccountInOrganizationByProject(accountId, status.ProjectId))
                return BadRequest();

            var statusToDelete = await _projectStatuses.DeleteAsync(status);

            if (statusToDelete is null)
                BadRequest("Invalid request");

            return statusToDelete;
        }
    }
}
