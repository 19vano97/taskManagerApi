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
using TaskManagerApi.Services.Interfaces.Business;
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

        public TicketController(ITicketService ticketItemService,
                                  ITicketHistoryService taskHistoryService,
                                  IAccountVerification accountVerification,
                                  ILogger<TicketController> logger)

        {
            _ticketItemService = ticketItemService;
            _ticketHistoryService = taskHistoryService;
            _accountVerification = accountVerification;
            _logger = logger;
        }

        [HttpGet("all/{organizationId}/organization")]
        public async Task<ActionResult<List<TicketDto>>> GetTasksInOrganizationAsync(Guid organizationId,
                                                                             CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetTasksInOrganizationAsync called with organizationId: {OrganizationId}", organizationId);

            var verification = await VerifyAccountInOrganization(cancellationToken);
            if (!verification.IsVerified)
            {
                _logger.LogWarning("Account verification failed for organizationId: {OrganizationId}", organizationId);
            }

            var result = await _ticketItemService.GetTasksByOrganizationAsync(organizationId, cancellationToken);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to get tasks for organizationId: {OrganizationId}. Error: {Error}", organizationId, result.ErrorMessage);

                if (result.ErrorMessage == LogPhrases.ServiceResult.Error.NOT_FOUND)
                {
                    return NotFound();
                }

                return BadRequest(result.ErrorMessage);
            }

            if (!verification.IsVerified)
            {
                var verification2 = await VerifyAccountInOrganization(result.Data!.Select(t => t.OrganizationId).First()!.Value, cancellationToken);
                if (!verification2.IsVerified)
                {
                    _logger.LogWarning("Account verification failed for organizationId: {OrganizationId}", organizationId);
                    return BadRequest();
                }
            }

            _logger.LogInformation($"{LogPhrases.PositiveActions.TASKS_SHOWN_LOG}", result.Data.Select(s => s.Id));

            return Ok(result.Data);
        }

        [HttpGet("all/{projectId}/project")]
        public async Task<ActionResult<List<TicketDto>>> GetTasksInOrganizationProjectAsync(Guid projectId,
                                                                                    CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetTasksInOrganizationProjectAsync called with projectId: {ProjectId}", projectId);

            var verification = await VerifyAccountInOrganization(cancellationToken);
            if (!verification.IsVerified)
            {
                _logger.LogWarning("Account verification failed for projectId: {ProjectId}", projectId);
            }

            var res = await _ticketItemService.GetTasksByProjectAsync(projectId, cancellationToken);
            if (!res.IsSuccess)
            {
                _logger.LogWarning("Failed to get tasks for projectId: {ProjectId}. Error: {Error}", projectId, res.ErrorMessage);

                if (res.ErrorMessage == LogPhrases.ServiceResult.Error.NOT_FOUND)
                {
                    return NotFound();
                }

                return BadRequest(res.ErrorMessage);
            }

            if (!verification.IsVerified)
            {
                var verification2 = await VerifyAccountInOrganization(res.Data!.Select(t => t.OrganizationId).First()!.Value, cancellationToken);
                if (!verification2.IsVerified)
                {
                    _logger.LogWarning("Account verification failed for projectId: {ProjectId}", projectId);
                    return BadRequest();
                }
            }

            return Ok(res.Data);
        }

        [HttpGet("{Id}/details")]
        public async Task<ActionResult<TicketDto>> GetTaskByIdAsync(Guid Id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetTaskByIdAsync called with Id: {TaskId}", Id);

            var verification = await VerifyAccountInOrganization(cancellationToken);
            if (!verification.IsVerified)
            {
                _logger.LogWarning("Account verification failed for taskId: {TaskId}", Id);
            }

            var task = await _ticketItemService.GetTaskByIdAsync(Id, cancellationToken);
            if (!task.IsSuccess)
            {
                _logger.LogWarning("Failed to get task by Id: {TaskId}. Error: {Error}", Id, task.ErrorMessage);

                if (task.ErrorMessage == LogPhrases.ServiceResult.Error.NOT_FOUND)
                {
                    return NotFound();
                }

                return BadRequest(task.ErrorMessage);
            }

            if (!verification.IsVerified)
            {
                var verification2 = await VerifyAccountInOrganization((Guid)task.Data!.OrganizationId!, cancellationToken);
                if (!verification2.IsVerified)
                {
                    _logger.LogWarning("Account verification failed for taskId: {TaskId}", Id);
                    return BadRequest();
                }
            }

            return Ok(task.Data);
        }

        [HttpPost("create")]
        public async Task<ActionResult<TicketDto>> CreateTaskAsync(TicketDto newTask, CancellationToken cancellationToken)
        {
            _logger.LogInformation("CreateTaskAsync called");

            var verification = await VerifyAccountInOrganization(cancellationToken);
            if (!verification.IsVerified)
            {
                _logger.LogWarning("Account verification failed for CreateTaskAsync");
                return BadRequest();
            }

            var createdTask = await _ticketItemService.CreateTaskAsync(newTask, cancellationToken);
            if (!createdTask.IsSuccess)
            {
                _logger.LogWarning("Failed to create task. Error: {Error}", createdTask.ErrorMessage);

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
            _logger.LogInformation("CreateTaskForAiAsync called");

            var verification = await VerifyAccountInOrganization(cancellationToken);
            if (!verification.IsVerified)
            {
                _logger.LogWarning("Account verification failed for CreateTaskForAiAsync");
                return BadRequest();
            }

            var createdTask = await _ticketItemService.CreateTicketsForAiAsync(newTasks, cancellationToken);
            if (!createdTask.IsSuccess)
            {
                _logger.LogWarning("Failed to create AI tasks. Error: {Error}", createdTask.ErrorMessage);

                if (createdTask.ErrorMessage == LogPhrases.ServiceResult.Error.NOT_FOUND)
                {
                    return NotFound();
                }

                return BadRequest(createdTask.ErrorMessage);
            }

            return Ok(createdTask.Data);
        }

        [HttpPost("{taskId}/edit")]
        public async Task<ActionResult<TicketDto>> EditTaskAsync(Guid taskId,
                                                             TicketDto editTask,
                                                             CancellationToken cancellationToken)
        {
            _logger.LogInformation("EditTaskByIdAsync called with taskId: {TaskId}", taskId);

            var verification = await VerifyAccountInOrganization(cancellationToken);
            if (!verification.IsVerified)
            {
                _logger.LogWarning("Account verification failed for EditTaskByIdAsync, taskId: {TaskId}", taskId);
                return BadRequest();
            }

            var taskToEdit = await _ticketItemService.EditTaskByIdAsync(taskId, editTask, cancellationToken);
            if (!taskToEdit.IsSuccess)
            {
                _logger.LogWarning("Failed to edit task. taskId: {TaskId}, Error: {Error}", taskId, taskToEdit.ErrorMessage);

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
            _logger.LogInformation("GetHistoryByTaskId called with taskId: {TaskId}", taskId);

            var verification = await VerifyAccountInOrganization(cancellationToken);
            if (!verification.IsVerified)
            {
                _logger.LogWarning("Account verification failed for GetHistoryByTaskId, taskId: {TaskId}", taskId);
                return BadRequest();
            }

            var res = await _ticketHistoryService.GetHistoryByTaskId(taskId, cancellationToken);
            if (!res.IsSuccess)
            {
                _logger.LogWarning("Failed to get history for taskId: {TaskId}. Error: {Error}", taskId, res.ErrorMessage);

                if (res.ErrorMessage == LogPhrases.ServiceResult.Error.NOT_FOUND)
                {
                    return NoContent();
                }

                return BadRequest(res.ErrorMessage);
            }

            return Ok(res.Data);
        }

        [HttpPost("{ticketId}/comment/new")]
        public async Task<ActionResult> AddNewComment(Guid ticketId, TicketCommentDto comment, CancellationToken cancellationToken)
        {
            _logger.LogInformation("AddNewComment called with Id: {TaskId}", comment.TicketId);

            var verification = await VerifyAccountInOrganization(cancellationToken);
            if (!verification.IsVerified)
            {
                _logger.LogWarning("Account verification failed for AddNewComment, taskId: {TaskId}", comment.TicketId);
                return BadRequest();
            }

            var addComment = await _ticketItemService.PostNewComment(comment, cancellationToken);

            if (!addComment.IsSuccess)
            {
                _logger.LogWarning("Failed to add comment. taskId: {TaskId}, Error: {Error}", ticketId, addComment.ErrorMessage);

                return BadRequest(addComment.ErrorMessage);
            }

            return Ok();
        }

        [HttpPost("{ticketId}/comment/{commentId}")]
        public async Task<ActionResult> EditComment(Guid ticketId, Guid commentId, TicketCommentDto comment, CancellationToken cancellationToken)
        {
            _logger.LogInformation("EditComment called with Id: {TaskId}", ticketId);

            var verification = await VerifyAccountInOrganization(cancellationToken);
            if (!verification.IsVerified || ticketId != comment.TicketId || commentId != comment.Id)
            {
                _logger.LogWarning("Account verification failed for EditComment, taskId: {TaskId}", ticketId);
                return BadRequest();
            }

            var addComment = await _ticketItemService.EditComment(comment, cancellationToken);

            if (!addComment.IsSuccess)
            {
                _logger.LogWarning("Failed to edit comment. taskId: {TaskId}, Error: {Error}", ticketId, addComment.ErrorMessage);

                return BadRequest(addComment.ErrorMessage);
            }

            return Ok();
        }

        [HttpDelete("{ticketId}/comment/{commentId}")]
        public async Task<ActionResult> DeteleComment(Guid ticketId, Guid commentId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("DeteleComment called with Id: {TaskId}", ticketId);

            var verification = await VerifyAccountInOrganization(cancellationToken);
            if (!verification.IsVerified)
            {
                _logger.LogWarning("Account verification failed for EditComment, taskId: {TaskId}", ticketId);
                return BadRequest();
            }

            var addComment = await _ticketItemService.DeleteComment(commentId, cancellationToken);

            if (!addComment.IsSuccess)
            {
                _logger.LogWarning("Failed to delete comment. taskId: {TaskId}, Error: {Error}", ticketId, addComment.ErrorMessage);

                return BadRequest(addComment.ErrorMessage);
            }

            return Ok();
        }

        [HttpGet("{ticketId}/comment/all")]
        public async Task<ActionResult> GetAllComments(Guid ticketId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetAllComments called with Id: {TaskId}", ticketId);

            var verification = await VerifyAccountInOrganization(cancellationToken);
            if (!verification.IsVerified)
            {
                _logger.LogWarning("Account verification failed for GetAllComments, taskId: {TaskId}", ticketId);
                return BadRequest();
            }

            var getComments = await _ticketItemService.GetCommentsByTicketId(ticketId, cancellationToken);

            if (!getComments.IsSuccess)
            {
                _logger.LogWarning("Failed to get comments. taskId: {TaskId}, Error: {Error}", ticketId, getComments.ErrorMessage);

                return BadRequest(getComments.ErrorMessage);
            }

            return Ok(getComments.Data);
        }

        [HttpDelete("{Id}/delete")]
        public async Task<ActionResult> DeleteTaskByIdAsync(Guid Id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("DeleteTaskByIdAsync called with Id: {TaskId}", Id);

            var verification = await VerifyAccountInOrganization(cancellationToken);
            if (!verification.IsVerified)
            {
                _logger.LogWarning("Account verification failed for DeleteTaskByIdAsync, taskId: {TaskId}", Id);
                return BadRequest();
            }

            var taskToDelete = await _ticketItemService.DeleteTaskAsync(Id, cancellationToken);
            if (!taskToDelete.IsSuccess)
            {
                _logger.LogWarning("Failed to delete task. taskId: {TaskId}, Error: {Error}", Id, taskToDelete.ErrorMessage);

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
            _logger.LogInformation("VerifyAccountInOrganization called");

            if (!this.Request.Headers.TryGetValue("organizationId", out var organizationIdString)
               || !Guid.TryParse(organizationIdString, out Guid organizationId)
               || !Guid.TryParse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value, out Guid accountId)
               || !await _accountVerification.VerifyAccountInOrganization(accountId, organizationId, cancellationToken))
            {
                _logger.LogWarning("Account verification failed in VerifyAccountInOrganization");
                return new VerificationOrganizationAccountDto
                {
                    IsVerified = false,
                    OrganizationId = Guid.Empty,
                    AccountId = Guid.Empty
                };
            }

            return new VerificationOrganizationAccountDto
            {
                IsVerified = true,
                OrganizationId = organizationId,
                AccountId = accountId
            };
        }
        
        private async Task<VerificationOrganizationAccountDto> VerifyAccountInOrganization(Guid organizationId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("VerifyAccountInOrganization called");

            if (!Guid.TryParse(User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)!.Value, out Guid accountId)
               || !await _accountVerification.VerifyAccountInOrganization(accountId, organizationId, cancellationToken))
            {
                _logger.LogWarning("Account verification failed in VerifyAccountInOrganization");
                return new VerificationOrganizationAccountDto
                {
                    IsVerified = false,
                    OrganizationId = Guid.Empty,
                    AccountId = Guid.Empty
                };
            }

            return new VerificationOrganizationAccountDto
            {
                IsVerified = true,
                OrganizationId = organizationId,
                AccountId = accountId
            };
        }
    }
}
