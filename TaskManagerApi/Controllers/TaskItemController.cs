using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerApi.Data;
using TaskManagerApi.Enitities;
using TaskManagerApi.Models;
using TaskManagerApi.Models.TaskHistory;
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
        private ITaskHistoryService _taskHistoryService;
        private IAccountVerification _accountVerification;
        private ILogger<TaskItemController> _logger;
        private TaskManagerAPIDbContext _context;

        public TaskItemController(ITaskItemService taskItemService,
                                  ITaskHistoryService taskHistoryService,
                                  IAccountVerification accountVerification,
                                  ILogger<TaskItemController> logger,
                                  TaskManagerAPIDbContext context)

        {
            _taskItemService = taskItemService;
            _taskHistoryService = taskHistoryService;
            _accountVerification = accountVerification;
            _logger = logger;
            _context = context;
            
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<TaskItemDto>>> GetTasksInOrganizationAsync()
        {
            if (this.Request.Headers.TryGetValue("organizationId", out var organizationIdString)
                || !Guid.TryParse(organizationIdString, out Guid organizationId)
                || !Guid.TryParse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value, out Guid accountId)
                || !await _accountVerification.VerifyAccountInOrganization(accountId, organizationId))
                return BadRequest();

            var result = await _taskItemService.GetTasksByOrganizationAsync(organizationId);
            _logger.LogInformation($"{LogPhrases.PositiveActions.TASKS_SHOWN_LOG}", result.Select(s => s.Id));

            return Ok(result);
        }

        [HttpGet("all/{projectId}")]
        public async Task<ActionResult<List<TaskItemDto>>> GetTasksInOrganizationProjectAsync(Guid projectId)
        {
            return Ok(await _taskItemService.GetTasksByProjectAsync(projectId));
        }

        [HttpGet("details/{Id}")]
        public async Task<ActionResult<TaskItemDto>> GetTaskByIdAsync(Guid Id)
        {
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
            var taskToEdit = await _taskItemService.EditTaskByIdAsync(taskId, editTask);

            if (taskToEdit is null)
            {
                _logger.LogError($"{LogPhrases.NegativeActions.TASK_UPDATE_FAILED_LOG}", taskId);
                return BadRequest("Invalid request");
            }

            return Ok(taskToEdit);
        }

        [HttpGet("history/{taskId}")]
        public async Task<ActionResult<List<TaskHistoryDto>>> GetHistoryByTaskId(Guid taskId)
        {
            try
            {
                return Ok(await _taskHistoryService.GetHistoryByTaskId(taskId));
            }
            catch (System.Exception ex)
            {
                _logger.LogWarning(ex.ToString());
                return BadRequest();
            }
        }

        [HttpDelete("delete/{Id}")]
        public async Task<ActionResult<TaskItemDto>> DeleteTaskByIdAsync(Guid Id)
        {
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
