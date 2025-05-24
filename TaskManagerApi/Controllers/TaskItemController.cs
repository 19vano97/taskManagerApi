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
        private ILogger<TaskItemController> _logger;
        private TaskManagerAPIDbContext _context;

        public TaskItemController(ITaskItemService taskItemService,
                                  ILogger<TaskItemController> logger,
                                  TaskManagerAPIDbContext context)
        {
            _taskItemService = taskItemService;
            _logger = logger;
            _context = context;
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<TaskItemDto>>> GetTasksInOrganizationAsync()
        {
            if(!this.Request.Headers.TryGetValue("organizationId", out var organizationId) 
                || !await ValidateAccountOrganizationConnectionAsync(organizationId!)) 
            {
                _logger.LogError(LogPhrases.ApiLogs.API_AUTHORIZATION_FAILED_LOG);
                return BadRequest("Invalid request");
            }

            var result = await _taskItemService.GetTasksByOrganizationAsync(Guid.Parse(organizationId));
            _logger.LogInformation($"{LogPhrases.PositiveActions.TASKS_SHOWN_LOG}", result.Select(s => s.Id));

            return Ok(result);
        }

        [HttpGet("all/{projectId}")]
        public async Task<ActionResult<List<TaskItemDto>>> GetTasksInOrganizationProjectAsync(Guid projectId)
        {
            if(!this.Request.Headers.TryGetValue("organizationId", out var organizationId) 
                || !await ValidateAccountOrganizationConnectionAsync(organizationId!)) 
            {
                _logger.LogError(LogPhrases.ApiLogs.API_AUTHORIZATION_FAILED_LOG);
                return BadRequest("Invalid request");
            }

            return Ok(await _taskItemService.GetTasksByProjectAsync(projectId));
        }

        [HttpGet("details/{Id}")]
        public async Task<ActionResult<TaskItemDto>> GetTaskByIdAsync(Guid Id)
        {
            if(!this.Request.Headers.TryGetValue("organizationId", out var organizationId) 
                || !await ValidateAccountOrganizationConnectionAsync(organizationId!)) 
            {
                _logger.LogError(LogPhrases.ApiLogs.API_AUTHORIZATION_FAILED_LOG);
                return BadRequest("Invalid request");
            }

            var task = await _taskItemService.GetTaskByIdAsync(Id);

            if (task is null)
            {
                _logger.LogError($"{LogPhrases.NegativeActions.TASK_NOT_FOUND_LOG}", Id);
                return BadRequest("Invalid request");
            }

            return Ok(task);
        }

        [HttpPost("create")]
        public async Task<ActionResult<TaskItemDto>> CreateTaskAsync(TaskItemDto newTask)
        {
            if(!this.Request.Headers.TryGetValue("organizationId", out var organizationId) 
                || !await ValidateAccountOrganizationConnectionAsync(organizationId!)) 
            {
                _logger.LogError(LogPhrases.ApiLogs.API_AUTHORIZATION_FAILED_LOG);
                return BadRequest("Invalid request");
            }

            var createdTask = await _taskItemService.CreateTaskAsync(newTask);

            if (createdTask is null)
            {
                _logger.LogError($"{LogPhrases.NegativeActions.TASK_CREATION_FAILED_LOG}", newTask);
                return BadRequest("Invalid request");
            }

            return Ok(createdTask);
        }

        [HttpPost("edit/{taskId}")]
        public async Task<ActionResult<TaskItemDto>> EditTaskByIdAsync(Guid taskId, TaskItemDto editTask)
        {
            if(!this.Request.Headers.TryGetValue("organizationId", out var organizationId) 
                || !await ValidateAccountOrganizationConnectionAsync(organizationId!)) 
            {
                _logger.LogError(LogPhrases.ApiLogs.API_AUTHORIZATION_FAILED_LOG);
                return BadRequest("Invalid request");
            }

            var taskToEdit = await _taskItemService.EditTaskByIdAsync(taskId, editTask);

            if (taskToEdit is null)
            {
                _logger.LogError($"{LogPhrases.NegativeActions.TASK_UPDATE_FAILED_LOG}", taskId);
                return BadRequest("Invalid request");
            }

            return Ok(taskToEdit);
        }

        [HttpPut("edit/{taskId}/task/{statusId}")]
        public async Task<ActionResult<TaskItemDto>> ChangeTaskStatusAsync(Guid taskId, int statusId)
        {
            if(!this.Request.Headers.TryGetValue("organizationId", out var organizationId) 
                || !await ValidateAccountOrganizationConnectionAsync(organizationId!)) 
            {
                _logger.LogError(LogPhrases.ApiLogs.API_AUTHORIZATION_FAILED_LOG);
                return BadRequest("Invalid request");
            }

            var taskToEdit = await _taskItemService.ChangeTaskStatusAsync(taskId, statusId);

            if (taskToEdit is null)
            {
                _logger.LogError($"{LogPhrases.NegativeActions.TASK_UPDATE_FAILED_LOG}", taskId);
                return BadRequest("Invalid request");
            }

            return Ok(taskToEdit);
        }

        [HttpPost("parent")]
        public async Task<ActionResult<TaskItemDto>> AddParentToTask(TaskParentDto task)
        {
            if(!this.Request.Headers.TryGetValue("organizationId", out var organizationId) 
                || !await ValidateAccountOrganizationConnectionAsync(organizationId!)) 
            {
                _logger.LogError(LogPhrases.ApiLogs.API_AUTHORIZATION_FAILED_LOG);
                return BadRequest("Invalid request");
            }

            var taskEdit = await _taskItemService.AddParentTicket(task.ParentId, task.ParentId);

            if(taskEdit is null)
            {
                _logger.LogError($"{LogPhrases.NegativeActions.TASK_UPDATE_FAILED_LOG}", task);
                return BadRequest("Invalid request");
            }

            return Ok(taskEdit);
        }

        [HttpDelete("delete/{Id}")]
        public async Task<ActionResult<TaskItemDto>> DeleteTaskByIdAsync(Guid Id)
        {
            if(!this.Request.Headers.TryGetValue("organizationId", out var organizationId) 
                || !await ValidateAccountOrganizationConnectionAsync(organizationId!)) 
            {
                _logger.LogError(LogPhrases.ApiLogs.API_AUTHORIZATION_FAILED_LOG);
                return BadRequest("Invalid request");
            }

            var taskToDelete = await _taskItemService.DeleteTaskAsync(Id);

            if (taskToDelete is null)
            {
                _logger.LogError($"{LogPhrases.NegativeActions.TASK_NOT_FOUND_LOG}", Id);
                return BadRequest("Invalid request");
            }

            return Ok(taskToDelete);
        }

        private async Task<bool> ValidateAccountOrganizationConnectionAsync(string organizationId)
        {
            return Guid.TryParse(organizationId, out var organizationIdGuid)
                & await GeneralService.VerifyAccountRelatesToOrganization(_context, Guid.Parse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value)
                    , organizationIdGuid) is not null;
        }
    }
}
