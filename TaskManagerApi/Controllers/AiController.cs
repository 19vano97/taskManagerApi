using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagerApi.Models.AI;
using TaskManagerApi.Models.Verification;
using TaskManagerApi.Services.Interfaces;
using TaskManagerApi.Services.Interfaces.Business;
using static TaskManagerApi.Models.Constants;

namespace TaskManagerApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/ai")]
    [ApiController]
    public class AiController : ControllerBase
    {
        private IAiService _aiService;
        private IAccountVerification _accountVerification;
        private ILogger<AiController> _logger;

        public AiController(IAiService aiService,
                            IAccountVerification accountVerification,
                            ILogger<AiController> logger)
        {
            _aiService = aiService;
            _accountVerification = accountVerification;
            _logger = logger;
        }

        [HttpPost("chat/{threadId}/message")]
        public async Task<ActionResult<ChatMessageDto>> ChattingWithUser([FromBody] ChatMessageDto message, Guid threadId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ChattingWithUser called for threadId: {ThreadId}", threadId);
            var verification = await VerifyAccountInOrganization(cancellationToken);
            if (!verification.IsVerified)
            {
                _logger.LogWarning("Account verification failed for ChattingWithUser. threadId: {ThreadId}", threadId);
                return BadRequest();
            }

            var res = await _aiService.PostChatMessageAsync(message, new AiThreadDetailsDto
            {
                Id = threadId,
                AccountId = verification.AccountId,
                OrganizationId = verification.OrganizationId
            }, cancellationToken);

            if (!res.IsSuccess)
            {
                _logger.LogWarning("Failed to post chat message. threadId: {ThreadId}. Error: {Error}", threadId, res.ErrorMessage);
                if (res.ErrorMessage == LogPhrases.ServiceResult.Error.NOT_FOUND)
                {
                    return NotFound();
                }

                return BadRequest(res.ErrorMessage);
            }

            _logger.LogInformation("Chat message posted successfully. threadId: {ThreadId}", threadId);
            return Ok(res.Data);
        }

        [HttpGet("chat/{aiThread}/history")]
        public async Task<ActionResult<List<ChatMessageDto>>> GetChatHistoryByThreadId(Guid aiThread, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetChatHistoryByThreadId called for aiThread: {ThreadId}", aiThread);
            var verification = await VerifyAccountInOrganization(cancellationToken);
            if (!verification.IsVerified)
            {
                _logger.LogWarning("Account verification failed for GetChatHistoryByThreadId. aiThread: {ThreadId}", aiThread);
                return BadRequest();
            }

            var res = await _aiService.GetChatHistoryByThread(aiThread, cancellationToken);
            if (!res.IsSuccess)
            {
                _logger.LogWarning("Failed to get chat history. aiThread: {ThreadId}. Error: {Error}", aiThread, res.ErrorMessage);
                if (res.ErrorMessage == LogPhrases.ServiceResult.Error.NOT_FOUND)
                {
                    return NotFound();
                }

                return BadRequest(res.ErrorMessage);
            }

            _logger.LogInformation("Chat history fetched successfully. aiThread: {ThreadId}", aiThread);
            return Ok(res.Data);
        }

        [HttpPost("thread/create")]
        public async Task<ActionResult<AiThreadDetailsDto>> CreateNewThread(AiThreadDetailsDto aiThread, CancellationToken cancellationToken)
        {
            _logger.LogInformation("CreateNewThread called");
            var verification = await VerifyAccountInOrganization(cancellationToken);
            if (!verification.IsVerified)
            {
                _logger.LogWarning("Account verification failed for CreateNewThread");
                return BadRequest();
            }

            aiThread.AccountId = verification.AccountId;
            aiThread.OrganizationId = verification.OrganizationId;

            var res = await _aiService.CreateNewThreadAsync(aiThread, cancellationToken);
            if (!res.IsSuccess)
            {
                _logger.LogWarning("Failed to create new thread. Error: {Error}", res.ErrorMessage);
                if (res.ErrorMessage == LogPhrases.ServiceResult.Error.NOT_FOUND)
                {
                    return NotFound();
                }

                return BadRequest(res.ErrorMessage);
            }

            _logger.LogInformation("New thread created successfully. Id: {ThreadId}", res.Data?.Id);
            return Ok(res.Data);
        }

        [HttpDelete("thread/{threadId}/delete")]
        public async Task<ActionResult<AiThreadDetailsDto>> DeleteThread(Guid threadId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("DeleteThread called for threadId: {ThreadId}", threadId);
            var verification = await VerifyAccountInOrganization(cancellationToken);
            if (!verification.IsVerified)
            {
                _logger.LogWarning("Account verification failed for DeleteThread. threadId: {ThreadId}", threadId);
                return BadRequest();
            }

            var res = await _aiService.DeleteThreadAsync(threadId, verification.AccountId, cancellationToken);
            if (!res.IsSuccess)
            {
                _logger.LogWarning("Failed to delete thread. threadId: {ThreadId}. Error: {Error}", threadId, res.ErrorMessage);
                if (res.ErrorMessage == LogPhrases.ServiceResult.Error.NOT_FOUND)
                {
                    return NotFound();
                }

                return BadRequest(res.ErrorMessage);
            }

            _logger.LogInformation("Thread deleted successfully. threadId: {ThreadId}", threadId);
            return Ok();
        }

        [HttpGet("thread/all")]
        public async Task<ActionResult<List<AiThreadDetailsDto>>> GetAllThreads(CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetAllThreads called");
            var verification = await VerifyAccountInOrganization(cancellationToken);
            if (!verification.IsVerified)
            {
                _logger.LogWarning("Account verification failed for GetAllThreads");
                return BadRequest();
            }

            var res = await _aiService.GetAllThreadsByOrganizationAccountAsync(verification.OrganizationId,
                                                                          verification.AccountId,
                                                                          cancellationToken);
            if (!res.IsSuccess)
            {
                _logger.LogWarning("Failed to get all threads. Error: {Error}", res.ErrorMessage);
                if (res.ErrorMessage == LogPhrases.ServiceResult.Error.NOT_FOUND)
                {
                    return NotFound();
                }

                return BadRequest(res.ErrorMessage);
            }

            _logger.LogInformation("All threads fetched successfully for organizationId: {OrgId}, accountId: {AccountId}", verification.OrganizationId, verification.AccountId);
            return Ok(res.Data);
        }

        [HttpGet("thread/{threadId}/info")]
        public async Task<ActionResult<AiThreadDetailsDto>> GetThreadInfo(Guid aiThread, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetThreadInfo called for aiThread: {ThreadId}", aiThread);
            var verification = await VerifyAccountInOrganization(cancellationToken);
            if (!verification.IsVerified)
            {
                _logger.LogWarning("Account verification failed for GetThreadInfo. aiThread: {ThreadId}", aiThread);
                return BadRequest();
            }

            var res = await _aiService.GetThreadInfoAsync(aiThread, cancellationToken);
            if (!res.IsSuccess)
            {
                _logger.LogWarning("Failed to get thread info. aiThread: {ThreadId}. Error: {Error}", aiThread, res.ErrorMessage);
                if (res.ErrorMessage == LogPhrases.ServiceResult.Error.NOT_FOUND)
                {
                    return NotFound();
                }

                return BadRequest(res.ErrorMessage);
            }

            _logger.LogInformation("Thread info fetched successfully. aiThread: {ThreadId}", aiThread);
            return Ok(res.Data);
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
    }
}
