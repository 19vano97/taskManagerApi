using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskHistoryService.Models;
using TaskHistoryService.Services.Interfaces;

namespace TaskHistoryService.Controllers
{
    [Route("api/thistory")]
    [ApiController]
    public class TaskHistoryController : ControllerBase
    {
        private IHistoryService _taskHistoryService;

        public TaskHistoryController(IHistoryService taskHistoryService)
        {
            _taskHistoryService = taskHistoryService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddHistory(TaskHistoryDto history)
        {
            if (!IsValidHistory(history))
                return BadRequest("Invalid request");

            await _taskHistoryService.Write(history);

            return Ok();
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
