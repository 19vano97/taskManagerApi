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
    public class TaskItemController : ControllerBase
    {
        private ITaskItemService _taskItemService;
        private TaskManagerAPIDbContext _context;

        public TaskItemController(ITaskItemService taskItemService, TaskManagerAPIDbContext context)
        {
            _taskItemService = taskItemService;
            _context = context;
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<TaskItemDto>>> GetTasksInOrganizationAsync()
        {
            if(!this.Request.Headers.TryGetValue("organizationId", out var organizationId) 
                || !await ValidateAccountOrganizationConnectionAsync(organizationId!)) 
                return BadRequest("Invalid request");

            return Ok(await _taskItemService.GetTasksByOrganizationAsync(Guid.Parse(organizationId)));
        }

        [HttpGet("all/{projectId}")]
        public async Task<ActionResult<List<TaskItemDto>>> GetTasksInOrganizationProjectAsync(Guid projectId)
        {
            if(!this.Request.Headers.TryGetValue("organizationId", out var organizationId) 
                || !await ValidateAccountOrganizationConnectionAsync(organizationId!)) 
                return BadRequest("Invalid request");

            return Ok(await _taskItemService.GetTasksByProjectAsync(projectId));
        }

        [HttpGet("details/{Id}")]
        public async Task<ActionResult<TaskItemDto>> GetTaskByIdAsync(Guid Id)
        {
            if(!this.Request.Headers.TryGetValue("organizationId", out var organizationId) 
                || !await ValidateAccountOrganizationConnectionAsync(organizationId!)) 
                return BadRequest("Invalid request");

            var task = await _taskItemService.GetTaskByIdAsync(Id);

            if (task is null)
                return NotFound();

            return Ok(task);
        }

        [HttpPost("create")]
        public async Task<ActionResult<TaskItemDto>> CreateTaskAsync(TaskItemDto newTask)
        {
            if(!this.Request.Headers.TryGetValue("organizationId", out var organizationId) 
                || !await ValidateAccountOrganizationConnectionAsync(organizationId!)) 
                return BadRequest("Invalid request");

            var createdTask = await _taskItemService.CreateTaskAsync(newTask);

            if (createdTask is null)
                return BadRequest("Empty title");

            return Ok(createdTask);
        }

        [HttpPost("edit/{Id}")]
        public async Task<ActionResult<TaskItemDto>> EditTaskByIdAsync(Guid Id, TaskItemDto editTask)
        {
            if(!this.Request.Headers.TryGetValue("organizationId", out var organizationId) 
                || !await ValidateAccountOrganizationConnectionAsync(organizationId!)) 
                return BadRequest("Invalid request");

            var taskToEdit = await _taskItemService.EditTaskByIdAsync(Id, editTask);

            if (taskToEdit is null)
                return BadRequest();

            return Ok(taskToEdit);
        }

        [HttpPut("edit/{taskId}/assignee/{assigneeId}")]
        public async Task<ActionResult<TaskItemDto>> ChangeTaskAssigneeByIdAsync(Guid taskId, Guid assigneeId)
        {
            if(!this.Request.Headers.TryGetValue("organizationId", out var organizationId) 
                || !await ValidateAccountOrganizationConnectionAsync(organizationId!)) 
                return BadRequest("Invalid request");

            var taskToEdit = await _taskItemService.ChangeAssigneeAsync(taskId, assigneeId);

            if (taskToEdit is null)
                return BadRequest();

            return Ok(taskToEdit);
        }

        [HttpPut("edit/{taskId}/project/{projectId}")]
        public async Task<ActionResult<TaskItemDto>> ChangeTaskProjectAsync(Guid Id, Guid projectId)
        {
            if(!this.Request.Headers.TryGetValue("organizationId", out var organizationId) 
                || !await ValidateAccountOrganizationConnectionAsync(organizationId!)) 
                return BadRequest("Invalid request");

            var taskToEdit = await _taskItemService.ChangeAssigneeAsync(Id, projectId);

            if (taskToEdit is null)
                return BadRequest();

            return Ok(taskToEdit);
        }

        [HttpPut("edit/{taskId}/task/{statusId}")]
        public async Task<ActionResult<TaskItemDto>> ChangeTaskStatusAsync(Guid Id, int statusId)
        {
            if(!this.Request.Headers.TryGetValue("organizationId", out var organizationId) 
                || !await ValidateAccountOrganizationConnectionAsync(organizationId!)) 
                return BadRequest("Invalid request");

            var taskToEdit = await _taskItemService.ChangeTaskStatusAsync(Id, statusId);

            if (taskToEdit is null)
                return BadRequest();

            return Ok(taskToEdit);
        }

        [HttpDelete("delete/{Id}")]
        public async Task<ActionResult<TaskItemDto>> DeleteTaskByIdAsync(Guid Id)
        {
            if(!this.Request.Headers.TryGetValue("organizationId", out var organizationId) 
                || !await ValidateAccountOrganizationConnectionAsync(organizationId!)) 
                return BadRequest("Invalid request");

            var taskToDelete = await _taskItemService.DeleteTaskAsync(Id);

            if (taskToDelete is null)
                return NotFound();

            return Ok(taskToDelete);
        }

        private async Task<bool> ValidateAccountOrganizationConnectionAsync(string organizationId)
        {
            return (Guid.TryParse(organizationId, out var organizationIdGuid)
                || await GeneralService.VerifyAccountRelatesToOrganization(_context, Guid.Parse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value)
                    , organizationIdGuid) is not null);
        }
    }
}
