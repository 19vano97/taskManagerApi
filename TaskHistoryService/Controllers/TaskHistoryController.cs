using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskHistoryService.Models;
using TaskHistoryService.Services.Interfaces;

namespace TaskHistoryService.Controllers
{
    [Route("api/thistory")]
    [ApiController]
    public class TicketHistoryController : ControllerBase
    {
        private IHistoryService _ticketHistoryService;

        public TicketHistoryController(IHistoryService ticketHistoryService)
        {
            _ticketHistoryService = ticketHistoryService;
        }

        [HttpPost("add")]
        public async Task<ActionResult> AddHistory(TaskHistoryDto history)
        {
            if (!IsValidHistory(history))
                return BadRequest("Invalid request");

            await _ticketHistoryService.Write(history);

            return Ok();
        }

        [HttpGet("info/{taskId}")]
        public async Task<ActionResult<List<TaskHistoryDto>>> GetHistoryInfo(Guid taskId)
        {
            if (taskId == Guid.Empty)
                return BadRequest("Invalid taskId");

            var history = await _ticketHistoryService.GetHistoryByTaskId(taskId);

            if (history == null)
                return NoContent();

            return Ok(history);
        }

        private bool IsValidHistory(TaskHistoryDto history)
        {
            if (history.Author == Guid.Empty
                || history.EventName is null
                || history.TaskId == Guid.Empty)
            {
                return false;
            }

            return true;
        }
    }
}
