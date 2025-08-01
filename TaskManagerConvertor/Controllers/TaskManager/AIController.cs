using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagerConvertor.Models.AI;
using TaskManagerConvertor.Services.Interfaces;

namespace TaskManagerConvertor.Controllers.TaskManager
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/ai")]
    [ApiController]
    public class AIController : ControllerBase
    {
        private readonly IAIService _aIService;
        private readonly ILogger<AIController> _logger;

        public AIController(IAIService aIService, ILogger<AIController> logger)
        {
            _aIService = aIService;
            _logger = logger;
        }

        [HttpPost("chat/{threadId}/message")]
        public async Task<ActionResult<ChatMessageDto>> ChattingWithUser([FromBody] ChatMessageDto message, Guid threadId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ChattingWithUser called for threadId: {ThreadId}", threadId);

            var response = await _aIService.ChatWithUserAsync(Request.Headers, message, threadId, cancellationToken);
            if (response.IsSuccess)
            {
                _logger.LogInformation(response.Data!.ToString());
                return Ok(response.Data!);
            }

            _logger.LogWarning(response.ErrorMessage);

            return BadRequest();
        }

        [HttpGet("chat/{aiThread}/history")]
        public async Task<ActionResult<List<ChatMessageDto>>> GetChatHistoryByThreadId(Guid aiThread, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetChatHistoryByThreadId called for aiThread: {ThreadId}", aiThread);

            var response = await _aIService.GetChatHistoryByThreadIdAsync(Request.Headers, aiThread, cancellationToken);
            if (response.IsSuccess)
            {
                _logger.LogInformation(response.Data!.ToString());
                return Ok(response.Data!);
            }

            _logger.LogWarning(response.ErrorMessage);

            return BadRequest();
        }

        [HttpPost("thread/create")]
        public async Task<ActionResult<AiThreadDetailsDto>> CreateNewThread(AiThreadDetailsDto aiThread, CancellationToken cancellationToken)
        {
            _logger.LogInformation("CreateNewThread called");

            var response = await _aIService.CreateNewThreadAsync(Request.Headers, aiThread, cancellationToken);
            if (response.IsSuccess)
            {
                _logger.LogInformation(response.Data!.ToString());
                return Ok(response.Data!);
            }

            _logger.LogWarning(response.ErrorMessage);

            return BadRequest();
        }

        [HttpDelete("thread/{threadId}/delete")]
        public async Task<ActionResult<AiThreadDetailsDto>> DeleteThread(Guid threadId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("DeleteThread called for threadId: {ThreadId}", threadId);

            var response = await _aIService.DeleteThreadAsync(Request.Headers, threadId, cancellationToken);
            if (response.IsSuccess)
            {
                _logger.LogInformation(response.Data!.ToString());
                return Ok(response.Data!);
            }

            _logger.LogWarning(response.ErrorMessage);

            return BadRequest();
        }

        [HttpGet("thread/all")]
        public async Task<ActionResult<List<AiThreadDetailsDto>>> GetAllThreads(CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetAllThreads called");

            var response = await _aIService.GetAllThreadsAsync(Request.Headers, cancellationToken);
            if (response.IsSuccess)
            {
                _logger.LogInformation(response.Data!.ToString());
                return Ok(response.Data!);
            }

            _logger.LogWarning(response.ErrorMessage);

            return BadRequest();
        }

        [HttpGet("thread/{threadId}/info")]
        public async Task<ActionResult<AiThreadDetailsDto>> GetThreadInfo(Guid aiThread, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetThreadInfo called for aiThread: {ThreadId}", aiThread);

            var response = await _aIService.GetThreadInfoAsync(Request.Headers, aiThread, cancellationToken);
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
