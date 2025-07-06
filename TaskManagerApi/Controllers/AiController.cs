using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagerApi.Models.AI;
using TaskManagerApi.Models.Verification;
using TaskManagerApi.Services.Interfaces;
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

        public AiController(IAiService aiService,
                            IAccountVerification accountVerification)
        {
            _aiService = aiService;
            _accountVerification = accountVerification;
        }

        [HttpPost("chat/{threadId}/message")]
        public async Task<ActionResult<ChatMessageDto>> ChattingWithUser([FromBody] ChatMessageDto message, Guid threadId)
        {
            var verification = await VerifyAccountInOrganization();
            if (!verification.IsVerified)
                return BadRequest();

            return Ok(await _aiService.PostChatMessageAsync(message, new AiThreadDetailsDto
            {
                Id = threadId,
                AccountId = verification.AccountId,
                OrganizationId = verification.OrganizationId
            }));
        }

        [HttpGet("chat/{aiThread}/history")]
        public async Task<ActionResult<List<ChatMessageDto>>> GetChatHistoryByThreadId(Guid aiThread)
        {
            var verification = await VerifyAccountInOrganization();
            if (!verification.IsVerified)
                return BadRequest();

            return Ok(await _aiService.GetChatHistoryByThread(aiThread));
        }

        [HttpPost("thread/create")]
        public async Task<ActionResult<AiThreadDetailsDto>> CreateNewThread(AiThreadDetailsDto aiThread)
        {
            var verification = await VerifyAccountInOrganization();
            if (!verification.IsVerified)
                return BadRequest();

            aiThread.AccountId = verification.AccountId;
            aiThread.OrganizationId = verification.OrganizationId;

            return Ok(await _aiService.CreateNewThread(aiThread));
        }

        [HttpGet("thread/all")]
        public async Task<ActionResult<List<AiThreadDetailsDto>>> GetAllThreads()
        {
            var verification = await VerifyAccountInOrganization();
            if (!verification.IsVerified)
                return BadRequest();

            return Ok(await _aiService.GetAllThreadsByOrganizationAccount(verification.OrganizationId,
                                                                          verification.AccountId));
        }

        [HttpGet("thread/{threadId}/info")]
        public async Task<ActionResult<AiThreadDetailsDto>> GetThreadInfo(Guid aiThread)
        {
            var verification = await VerifyAccountInOrganization();
            if (!verification.IsVerified)
                return BadRequest();

            return Ok(await _aiService.GetThreadInfo(aiThread));
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
