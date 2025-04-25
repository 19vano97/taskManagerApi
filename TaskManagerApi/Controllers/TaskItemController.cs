using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerApi.Data;
using TaskManagerApi.Enitities;
using TaskManagerApi.Models;
using TaskManagerApi.Models.TaskItem;
using TaskManagerApi.Services.Implementations;
using TaskManagerApi.Services.Interfaces;
using static TaskManagerApi.Models.Constants;

namespace TaskManagerApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/task")]
    [ApiController]
    public class TaskItemController(ITaskItemService taskItemService) : ControllerBase
    {
        [HttpGet("all")]
        public async Task<ActionResult<List<TaskItemDto>>> GetTasksInOrganization()
        {
            if(!this.Request.Headers.TryGetValue("organizationId", out var organizationId) 
                || !await ValidateAccountOrganizationConnection(organizationId!)) 
                return BadRequest("Invalid request");

            return Ok(await taskItemService.GetTasksByOrganizationAsync());
        }

        [HttpGet("details/{Id}")]
        public async Task<ActionResult<TaskItemDto>> GetTaskById(Guid Id)
        {
            if(!this.Request.Headers.TryGetValue("organizationId", out var organizationId) 
                || !await ValidateAccountOrganizationConnection(organizationId!)) 
                return BadRequest("Invalid request");

            var task = await taskItemService.GetTaskByIdAsync(Id);

            if (task is null)
                return NotFound();

            return Ok(task);
        }

        [HttpPost("create")]
        public async Task<ActionResult<TaskItemDto>> CreateTask(TaskItemDto newTask)
        {
            if(!this.Request.Headers.TryGetValue("organizationId", out var organizationId) 
                || !await ValidateAccountOrganizationConnection(organizationId!)) 
                return BadRequest("Invalid request");

            var createdTask = await taskItemService.CreateTaskAsync(newTask);

            if (createdTask is null)
                return BadRequest("Empty title");

            return Ok(createdTask);
        }

        [HttpPost("edit/{Id}")]
        public async Task<ActionResult<TaskItemDto>> EditTaskById(Guid Id, TaskItemDto editTask)
        {
            if(!this.Request.Headers.TryGetValue("organizationId", out var organizationId) 
                || !await ValidateAccountOrganizationConnection(organizationId!)) 
                return BadRequest("Invalid request");

            var taskToEdit = await taskItemService.EditTaskByIdAsync(Id, editTask);

            if (taskToEdit is null)
                return BadRequest();

            return Ok(taskToEdit);
        }

        [HttpPut("edit/{taskId}/assignee/{assigneeId}")]
        public async Task<ActionResult<TaskItemDto>> ChangeTaskAssigneeById(Guid taskId, Guid assigneeId)
        {
            if(!this.Request.Headers.TryGetValue("organizationId", out var organizationId) 
                || !await ValidateAccountOrganizationConnection(organizationId!)) 
                return BadRequest("Invalid request");

            var taskToEdit = await taskItemService.ChangeAssigneeAsync(taskId, assigneeId);

            if (taskToEdit is null)
                return BadRequest();

            return Ok(taskToEdit);
        }

        [HttpPut("edit/{taskId}/project/{projectId}")]
        public async Task<ActionResult<TaskItemDto>> ChangeTaskProject(Guid Id, Guid projectId)
        {
            if(!this.Request.Headers.TryGetValue("organizationId", out var organizationId) 
                || !await ValidateAccountOrganizationConnection(organizationId!)) 
                return BadRequest("Invalid request");

            var taskToEdit = await taskItemService.ChangeAssigneeAsync(Id, projectId);

            if (taskToEdit is null)
                return BadRequest();

            return Ok(taskToEdit);
        }

        [HttpPut("edit/{taskId}/task/{statusId}")]
        public async Task<ActionResult<TaskItemDto>> ChangeTaskStatus(Guid Id, int statusId)
        {
            if(!this.Request.Headers.TryGetValue("organizationId", out var organizationId) 
                || !await ValidateAccountOrganizationConnection(organizationId!)) 
                return BadRequest("Invalid request");

            var taskToEdit = await taskItemService.ChangeTaskStatusAsync(Id, statusId);

            if (taskToEdit is null)
                return BadRequest();

            return Ok(taskToEdit);
        }

        [HttpDelete("delete/{Id}")]
        public async Task<ActionResult<TaskItemDto>> DeleteTaskById(Guid Id)
        {
            if(!this.Request.Headers.TryGetValue("organizationId", out var organizationId) 
                || !await ValidateAccountOrganizationConnection(organizationId!)) 
                return BadRequest("Invalid request");

            var taskToDelete = await taskItemService.DeleteTaskAsync(Id);

            if (taskToDelete is null)
                return NotFound();

            return Ok(taskToDelete);
        }

        private async Task<bool> ValidateAccountOrganizationConnection(string organizationId)
        {
            return (!Guid.TryParse(organizationId, out var organizationIdGuid)
                || await GeneralService.VerifyAccountRelatesToOrganization(Guid.Parse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value)
                    , organizationIdGuid) is null);
        }
    }
}
