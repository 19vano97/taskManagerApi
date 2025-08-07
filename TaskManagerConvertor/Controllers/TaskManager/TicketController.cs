using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using TaskManagerConvertor.Services.Interfaces;
using TaskManagerConvertor.Models.TicketItem;
using Newtonsoft.Json;
using System.Text;
using TaskManagerConvertor.Models.Ticket;

namespace TaskManagerConvertor.Controllers.TaskManager
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/task")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        private readonly ILogger<TicketController> _logger;

        public TicketController(ITicketService ticketService,
                                ILogger<TicketController> logger)
        {
            _ticketService = ticketService;
            _logger = logger;
        }

        [HttpGet("{id}/details")]
        public async Task<ActionResult<string>> GetTaskByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetTaskByIdAsync called with Id: {TaskId}", id);

            var response = await _ticketService.GetTicketById(Request.Headers, id, cancellationToken);
            if (response.IsSuccess)
            {
                _logger.LogInformation(response.Data!.ToString());
                return Ok(response.Data!);
            }

            _logger.LogWarning(response.ErrorMessage);

            return BadRequest();
        }

        [HttpPost("create")]
        public async Task<ActionResult<string>> CreateTask([FromBody] TicketDto data,
                                                           CancellationToken cancellationToken)
        {
            _logger.LogInformation("CreateTaskAsync called");

            var response = await _ticketService.CreateTicketAsync(Request.Headers, data, cancellationToken);
            if (response.IsSuccess)
            {
                _logger.LogInformation(response.Data!.ToString());
                return Ok(response.Data!);
            }

            _logger.LogWarning(response.ErrorMessage);

            return BadRequest();
        }

        [HttpGet("all/{organizationId}/organization")]
        public async Task<ActionResult<List<TicketDto>>> GetTasksInOrganizationAsync(Guid organizationId,
                                                                            CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetTasksInOrganizationAsync called with organizationId: {OrganizationId}", organizationId);
            var response = await _ticketService.GetTasksAsync(Request.Headers, Guid.Empty, cancellationToken);

            if (response.IsSuccess)
            {
                _logger.LogInformation(response.Data!.ToString());
                return Ok(response.Data!);
            }

            _logger.LogWarning(response.ErrorMessage);

            return BadRequest();
        }

        [HttpGet("all/{projectId}/project")]
        public async Task<ActionResult<List<TicketDto>>> GetTasksInOrganizationProjectAsync(Guid projectId,
                                                                                    CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetTasksInOrganizationAsync called with organizationId: {OrganizationId}", projectId);
            var response = await _ticketService.GetTasksAsync(Request.Headers, projectId, cancellationToken);

            if (response.IsSuccess)
            {
                _logger.LogInformation(response.Data!.ToString());
                return Ok(response.Data!);
            }

            _logger.LogWarning(response.ErrorMessage);

            return BadRequest();
        }

        [HttpPost("create/ai/list")]
        public async Task<ActionResult<List<TicketDto>>> CreateTaskForAiAsync(TicketForAiDto[] newTasks,
                                                                      CancellationToken cancellationToken)
        {
            _logger.LogInformation("CreateTaskForAiAsync called");

            var response = await _ticketService.CreateTaskForAiAsync(Request.Headers, newTasks, cancellationToken);

            if (response.IsSuccess)
            {
                _logger.LogInformation(response.Data!.ToString());
                return Ok(response.Data!);
            }

            _logger.LogWarning(response.ErrorMessage);

            return BadRequest();
        }

        [HttpPost("{taskId}/edit")]
        public async Task<ActionResult<TicketDto>> EditTaskAsync(Guid taskId,
                                                             TicketDto editTask,
                                                             CancellationToken cancellationToken)
        {
            _logger.LogInformation("EditTaskByIdAsync called with taskId: {TaskId}", taskId);

            if (taskId != editTask.Id)
                return BadRequest();

            var response = await _ticketService.EditTaskByIdAsync(Request.Headers, editTask, cancellationToken);

            if (response.IsSuccess)
            {
                _logger.LogInformation(response.Data!.ToString());
                return Ok(response.Data!);
            }

            _logger.LogWarning(response.ErrorMessage);

            return BadRequest();
        }

        [HttpGet("{taskId}/history")]
        public async Task<ActionResult<List<TicketHistoryDto>>> GetHistoryByTaskId(Guid taskId,
                                                                          CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetHistoryByTaskId called with taskId: {TaskId}", taskId);

            var response = await _ticketService.GetTicketHistory(Request.Headers, taskId, cancellationToken);

            if (response.IsSuccess)
            {
                _logger.LogInformation(response.Data?.ToString());
                return Ok(response.Data!);
            }

            _logger.LogWarning(response.ErrorMessage);

            return BadRequest();
        }

        [HttpDelete("{Id}/delete")]
        public async Task<ActionResult> DeleteTaskByIdAsync(Guid Id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("DeleteTaskByIdAsync called with Id: {TaskId}", Id);

            var response = await _ticketService.DeleteTaskByIdAsync(Request.Headers, Id, cancellationToken);

            if (response.IsSuccess)
            {
                _logger.LogInformation(response.Data!.ToString());
                return Ok(response.Data!);
            }

            _logger.LogWarning(response.ErrorMessage);

            return BadRequest();
        }

        [HttpPost("{ticketId}/comment/new")]
        public async Task<ActionResult> AddNewComment(Guid ticketId, TicketCommentDto comment, CancellationToken cancellationToken)
        {
            _logger.LogInformation("AddNewComment called with Id: {TaskId}", comment.TicketId);

            var response = await _ticketService.PostNewComment(Request.Headers, ticketId, comment, cancellationToken);

            if (response.IsSuccess)
            {
                _logger.LogInformation(response.Data!.ToString());
                return Ok(response.Data!);
            }

            _logger.LogWarning(response.ErrorMessage);

            return BadRequest();
        }

        [HttpPost("{ticketId}/comment/{commentId}")]
        public async Task<ActionResult> EditComment(Guid ticketId, Guid commentId, TicketCommentDto comment, CancellationToken cancellationToken)
        {
            _logger.LogInformation("EditComment called with Id: {TaskId}", ticketId);

            var response = await _ticketService.EditComment(Request.Headers, ticketId, commentId, comment, cancellationToken);

            if (response.IsSuccess)
            {
                _logger.LogInformation(response.Data!.ToString());
                return Ok(response.Data!);
            }

            _logger.LogWarning(response.ErrorMessage);

            return BadRequest();
        }

        [HttpDelete("{ticketId}/comment/{commentId}")]
        public async Task<ActionResult> DeteleComment(Guid ticketId, Guid commentId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("DeteleComment called with Id: {TaskId}", ticketId);

            var response = await _ticketService.DeleteComment(Request.Headers, ticketId, commentId, cancellationToken);

            if (response.IsSuccess)
            {
                _logger.LogInformation(response.Data!.ToString());
                return Ok(response.Data!);
            }

            _logger.LogWarning(response.ErrorMessage);

            return BadRequest();
        }

        [HttpGet("{ticketId}/comment/all")]
        public async Task<ActionResult> GetAllComments(Guid ticketId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetAllComments called with Id: {TaskId}", ticketId);

            var response = await _ticketService.GetCommentsByTicketId(Request.Headers, ticketId, cancellationToken);

            if (response.IsSuccess)
            {
                _logger.LogInformation(response.Data!.ToString());
                return Ok(response.Data!);
            }

            _logger.LogWarning(response.ErrorMessage);

            return BadRequest();
        }
    }
}
