using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerApi.Data;
using TaskManagerApi.Enitities;
using TaskManagerApi.Models;
using TaskManagerApi.Models.TaskHistory;
using TaskManagerApi.Models.TicketItem;
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
        private ITicketService _ticketItemService;
        private ITicketHistoryService _ticketHistoryService;
        private IAccountVerification _accountVerification;
        private ILogger<TicketController> _logger;
        private TicketManagerAPIDbContext _context;

        public TicketController(ITicketService ticketItemService,
                                  ITicketHistoryService taskHistoryService,
                                  IAccountVerification accountVerification,
                                  ILogger<TicketController> logger,
                                  TicketManagerAPIDbContext context)

        {
            _ticketItemService = ticketItemService;
            _ticketHistoryService = taskHistoryService;
            _accountVerification = accountVerification;
            _logger = logger;
            _context = context;

        }

        [HttpGet("all/{organizationId}/organization")]
        public async Task<ActionResult<List<TicketDto>>> GetTasksInOrganizationAsync(Guid organizationId,
                                                                                     CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value, out Guid accountId)
                || !await _accountVerification.VerifyAccountInOrganization(accountId, organizationId, cancellationToken))
                return BadRequest();

            var result = await _ticketItemService.GetTasksByOrganizationAsync(organizationId, cancellationToken);
            if (!result.Success)
            {
                if (result.ErrorMessage == LogPhrases.ServiceResult.Error.NOT_FOUND)
                {
                    return NotFound();
                }

                return BadRequest(result.ErrorMessage);
            }

            _logger.LogInformation($"{LogPhrases.PositiveActions.TASKS_SHOWN_LOG}", result.Data.Select(s => s.Id));

            return Ok(result);
        }

        [HttpGet("all/{projectId}/project")]
        public async Task<ActionResult<List<TicketDto>>> GetTasksInOrganizationProjectAsync(Guid projectId,
                                                                                            CancellationToken cancellationToken)
        {
            var verification = await VerifyAccountInOrganization(cancellationToken);
            if (!verification.IsVerified)
                return BadRequest();

            var res = await _ticketItemService.GetTasksByProjectAsync(projectId, cancellationToken);
            if (!res.Success)
            {
                if (res.ErrorMessage == LogPhrases.ServiceResult.Error.NOT_FOUND)
                {
                    return NotFound();
                }

                return BadRequest(res.ErrorMessage);
            }

            return Ok(res.Data);
        }

        [HttpGet("{Id}/details")]
        public async Task<ActionResult<TicketDto>> GetTaskByIdAsync(Guid Id, CancellationToken cancellationToken)
        {
            var verification = await VerifyAccountInOrganization(cancellationToken);
            if (!verification.IsVerified)
                return BadRequest();

            var task = await _ticketItemService.GetTaskByIdAsync(Id, cancellationToken);
            if (!task.Success)
            {
                if (task.ErrorMessage == LogPhrases.ServiceResult.Error.NOT_FOUND)
                {
                    return NotFound();
                }

                return BadRequest(task.ErrorMessage);
            }

            return Ok(task.Data);
        }

        [HttpPost("create")]
        public async Task<ActionResult<TicketDto>> CreateTaskAsync(TicketDto newTask, CancellationToken cancellationToken)
        {
            var verification = await VerifyAccountInOrganization(cancellationToken);
            if (!verification.IsVerified)
                return BadRequest();

            var createdTask = await _ticketItemService.CreateTaskAsync(newTask, cancellationToken);
            if (!createdTask.Success)
            {
                if (createdTask.ErrorMessage == LogPhrases.ServiceResult.Error.NOT_FOUND)
                {
                    return NotFound();
                }

                return BadRequest(createdTask.ErrorMessage);
            }

            return Ok(createdTask.Data);
        }

        [HttpPost("create/ai/list")]
        public async Task<ActionResult<List<TicketDto>>> CreateTaskForAiAsync(TicketForAiDto[] newTasks,
                                                                              CancellationToken cancellationToken)
        {
            var verification = await VerifyAccountInOrganization(cancellationToken);
            if (!verification.IsVerified)
                return BadRequest();

            var createdTask = await _ticketItemService.CreateTicketsForAiAsync(newTasks, cancellationToken);
            if (!createdTask.Success)
            {
                if (createdTask.ErrorMessage == LogPhrases.ServiceResult.Error.NOT_FOUND)
                {
                    return NotFound();
                }

                return BadRequest(createdTask.ErrorMessage);
            }

            return Ok(createdTask.Data);
        }

        [HttpPost("{taskId}/edit")]
        public async Task<ActionResult<TicketDto>> EditTaskByIdAsync(Guid taskId,
                                                                     TicketDto editTask,
                                                                     CancellationToken cancellationToken)
        {
            var verification = await VerifyAccountInOrganization(cancellationToken);
            if (!verification.IsVerified)
                return BadRequest();

            var taskToEdit = await _ticketItemService.EditTaskByIdAsync(taskId, editTask, cancellationToken);
            if (!taskToEdit.Success)
            {
                if (taskToEdit.ErrorMessage == LogPhrases.ServiceResult.Error.NOT_FOUND)
                {
                    return NotFound();
                }

                return BadRequest(taskToEdit.ErrorMessage);
            }

            return Ok(taskToEdit.Data);
        }

        [HttpGet("{taskId}/history")]
        public async Task<ActionResult<List<TicketHistoryDto>>> GetHistoryByTaskId(Guid taskId,
                                                                                   CancellationToken cancellationToken)
        {
            var verification = await VerifyAccountInOrganization(cancellationToken);
            if (!verification.IsVerified)
                return BadRequest();

            var res = await _ticketHistoryService.GetHistoryByTaskId(taskId, cancellationToken);
            if (!res.Success)
            {
                if (res.ErrorMessage == LogPhrases.ServiceResult.Error.NOT_FOUND)
                {
                    return NotFound();
                }

                return BadRequest(res.ErrorMessage);
            }

            return Ok(res.Data);
        }

        [HttpDelete("{Id}/delete")]
        public async Task<ActionResult> DeleteTaskByIdAsync(Guid Id, CancellationToken cancellationToken)
        {
            var verification = await VerifyAccountInOrganization(cancellationToken);
            if (!verification.IsVerified)
                return BadRequest();

            var taskToDelete = await _ticketItemService.DeleteTaskAsync(Id, cancellationToken);
            if (!taskToDelete.Success)
            {
                if (taskToDelete.ErrorMessage == LogPhrases.ServiceResult.Error.NOT_FOUND)
                {
                    return NotFound();
                }

                return BadRequest(taskToDelete.ErrorMessage);
            }

            return Ok();
        }

        private async Task<VerificationOrganizationAccountDto> VerifyAccountInOrganization(CancellationToken cancellationToken)
        {
            if (!this.Request.Headers.TryGetValue("organizationId", out var organizationIdString)
               || !Guid.TryParse(organizationIdString, out Guid organizationId)
               || !Guid.TryParse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value, out Guid accountId)
               || !await _accountVerification.VerifyAccountInOrganization(accountId, organizationId, cancellationToken))
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
