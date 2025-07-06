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
using TaskManagerApi.Models.Tickets;
using TaskManagerApi.Models.Verification;
using TaskManagerApi.Services.Implementations;
using TaskManagerApi.Services.Interfaces;
using static TaskManagerApi.Models.Constants;

namespace TaskManagerApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/task")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private ITicketService _taskItemService;
        private ITicketHistoryService _taskHistoryService;
        private IAccountVerification _accountVerification;
        private ILogger<TicketController> _logger;
        private TaskManagerAPIDbContext _context;

        public TicketController(ITicketService taskItemService,
                                  ITicketHistoryService taskHistoryService,
                                  IAccountVerification accountVerification,
                                  ILogger<TicketController> logger,
                                  TaskManagerAPIDbContext context)

        {
            _taskItemService = taskItemService;
            _taskHistoryService = taskHistoryService;
            _accountVerification = accountVerification;
            _logger = logger;
            _context = context;

        }

        [HttpGet("all/{organizationId}/organization")]
        public async Task<ActionResult<List<TicketDto>>> GetTasksInOrganizationAsync(Guid organizationId)
        {
            if (!Guid.TryParse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value, out Guid accountId)
                || !await _accountVerification.VerifyAccountInOrganization(accountId, organizationId))
                return BadRequest();

            var result = await _taskItemService.GetTasksByOrganizationAsync(organizationId);
            _logger.LogInformation($"{LogPhrases.PositiveActions.TASKS_SHOWN_LOG}", result.Select(s => s.Id));

            return Ok(result);
        }

        [HttpGet("all/{projectId}/project")]
        public async Task<ActionResult<List<TicketDto>>> GetTasksInOrganizationProjectAsync(Guid projectId)
        {
            var verification = await VerifyAccountInOrganization();
            if (!verification.IsVerified)
                return BadRequest();

            return Ok(await _taskItemService.GetTasksByProjectAsync(projectId));
        }

        [HttpGet("{Id}/details")]
        public async Task<ActionResult<TicketDto>> GetTaskByIdAsync(Guid Id)
        {
            var verification = await VerifyAccountInOrganization();
            if (!verification.IsVerified)
                return BadRequest();

            var task = await _taskItemService.GetTaskByIdAsync(Id);

            if (task is null)
            {
                _logger.LogError($"{LogPhrases.NegativeActions.TASK_NOT_FOUND_LOG}", Id);
                return BadRequest("Invalid request");
            }

            return Ok(task);
        }

        [HttpPost("create")]
        public async Task<ActionResult<TicketDto>> CreateTaskAsync(TicketDto newTask)
        {
            var verification = await VerifyAccountInOrganization();
            if (!verification.IsVerified)
                return BadRequest();

            var createdTask = await _taskItemService.CreateTaskAsync(newTask);

            if (createdTask is null)
            {
                _logger.LogError($"{LogPhrases.NegativeActions.TASK_CREATION_FAILED_LOG}", newTask);
                return BadRequest("Invalid request");
            }

            return Ok(createdTask);
        }

        [HttpPost("create/ai/list")]
        public async Task<ActionResult<List<TicketDto>>> CreateTaskForAiAsync(TicketForAiDto[] newTasks)
        {
            var verification = await VerifyAccountInOrganization();
            if (!verification.IsVerified)
                return BadRequest();

            var createdTask = await _taskItemService.CreateTicketsForAiAsync(newTasks);

            if (createdTask is null)
            {
                _logger.LogError($"{LogPhrases.NegativeActions.TASK_CREATION_FAILED_LOG}", newTasks);
                return BadRequest("Invalid request");
            }

            return Ok(createdTask);
        }

        [HttpPost("{taskId}/edit")]
        public async Task<ActionResult<TicketDto>> EditTaskByIdAsync(Guid taskId, TicketDto editTask)
        {
            var verification = await VerifyAccountInOrganization();
            if (!verification.IsVerified)
                return BadRequest();

            var taskToEdit = await _taskItemService.EditTaskByIdAsync(taskId, editTask);

            if (taskToEdit is null)
            {
                _logger.LogError($"{LogPhrases.NegativeActions.TASK_UPDATE_FAILED_LOG}", taskId);
                return BadRequest("Invalid request");
            }

            return Ok(taskToEdit);
        }

        [HttpGet("{taskId}/history")]
        public async Task<ActionResult<List<TicketHistoryDto>>> GetHistoryByTaskId(Guid taskId)
        {
            var verification = await VerifyAccountInOrganization();
            if (!verification.IsVerified)
                return BadRequest();

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

        [HttpDelete("{Id}/delete")]
        public async Task<ActionResult> DeleteTaskByIdAsync(Guid Id)
        {
            var verification = await VerifyAccountInOrganization();
            if (!verification.IsVerified)
                return BadRequest();

            var taskToDelete = await _taskItemService.DeleteTaskAsync(Id);

            if (taskToDelete is null)
            {
                _logger.LogError($"{LogPhrases.NegativeActions.TASK_NOT_FOUND_LOG}", Id);
                return BadRequest("Invalid request");
            }

            return Ok();
        }
        
        private async Task<VerificationOrganizationAccountDto> VerifyAccountInOrganization()
        {
            if (!this.Request.Headers.TryGetValue("organizationId", out var organizationIdString)
               || !Guid.TryParse(organizationIdString, out Guid organizationId)
               || !Guid.TryParse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value, out Guid accountId)
               || !await _accountVerification.VerifyAccountInOrganization(accountId, organizationId))
                return new VerificationOrganizationAccountDto
                {
                    IsVerified = false,
                    OrganizationId = Guid.Empty,
                    AccountId = Guid.Empty
                };

            return new VerificationOrganizationAccountDto
            {
                IsVerified = true,
                OrganizationId = organizationId,
                AccountId = accountId
            };
        }
    }
}
