using System.Net.Http.Headers;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagerConvertor.Models;
using TaskManagerConvertor.Services.Interfaces;

namespace TaskManagerConvertor.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAccountService accountService,
                              ILogger<AuthController> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        [HttpGet("details")]
        public async Task<ActionResult<AccountDto>> GetAccountDetails(CancellationToken cancellationToken)
        {
            if (Request.Headers is null)
                return BadRequest();

            var response = await _accountService.GetOwnAccountDetails(Request.Headers, cancellationToken);
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
