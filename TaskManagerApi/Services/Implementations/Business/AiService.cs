using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using TaskManagerApi.Data;
using TaskManagerApi.Models;
using TaskManagerApi.Models.AI;
using TaskManagerApi.Services.Interfaces;
using TaskManagerApi.Services.Interfaces.Business;
using static TaskManagerApi.Models.Constants;

namespace TaskManagerApi.Services.Implementations.Business;

public class AiService : IAiService
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly TicketManagerAPIDbContext _context;
    private readonly ILogger<AiService> _logger;

    public AiService(IConfiguration configuration,
                     IHttpClientFactory httpClientFactory,
                     TicketManagerAPIDbContext context,
                     ILogger<AiService> logger)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _context = context;
        _logger = logger;
    }

    public async Task<ServiceResult<AiThreadDetailsDto>> CreateNewThreadAsync(AiThreadDetailsDto aiThread, CancellationToken cancellationToken)
    {
        var organizationAccountMap = await _context.OrganizationAccount.FirstOrDefaultAsync(o => o.AccountId == aiThread.AccountId
                                                                                            && o.OrganizationId == aiThread.OrganizationId
                                                                                            , cancellationToken);

        if (organizationAccountMap is null)
            return new ServiceResult<AiThreadDetailsDto>
            {
                IsSuccess = false,
                ErrorMessage = LogPhrases.ServiceResult.Error.NOT_FOUND
            };

        var token = _configuration["OpenAI:ApiKey"];
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
        client.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v2");

        var threadResponse = await client.PostAsJsonAsync("https://api.openai.com/v1/threads", (object)null, cancellationToken);
        var threadParce = await threadResponse.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        var threadId = threadParce["id"].ToString();

        _context.AiThreads.Add(new Enitities.Ai.AiThreads
        {
            OrganizationAccountId = organizationAccountMap.Id,
            Name = aiThread.Name,
            Thread = threadId
        });

        await _context.SaveChangesAsync(cancellationToken);

        var thread = await _context.AiThreads.FirstOrDefaultAsync(t => t.Thread == threadId, cancellationToken);
        if (thread is null)
            return new ServiceResult<AiThreadDetailsDto>
            {
                IsSuccess = false,
                ErrorMessage = LogPhrases.ServiceResult.Error.NOT_FOUND
            };

        var organizationAccountIdThread = await _context.OrganizationAccount.FirstAsync(o => o.Id == thread.OrganizationAccountId
                                                                                                                , cancellationToken);

        return new ServiceResult<AiThreadDetailsDto>
        {
            IsSuccess = true,
            Data = new AiThreadDetailsDto
            {
                Id = thread.Id,
                Name = thread.Name,
                OrganizationId = organizationAccountIdThread.OrganizationId,
                AccountId = organizationAccountIdThread.AccountId,
                CreateDate = thread.CreateDate
            }
        };
    }

    public async Task<ServiceResult<bool>> DeleteThreadAsync(Guid threadId, Guid accountId, CancellationToken cancellationToken)
    {
        var threadInformation = await _context.AiThreads.AsNoTracking()
                                                        .FirstOrDefaultAsync(t => t.Id == threadId, cancellationToken);
        if (threadInformation is null)
            return new ServiceResult<bool>
            {
                IsSuccess = false,
                Data = false,
                ErrorMessage = LogPhrases.ServiceResult.Error.NOT_FOUND
            };

        var accountOrganization = await _context.OrganizationAccount.AsNoTracking().FirstOrDefaultAsync(oa => oa.AccountId == accountId && oa.Id == threadInformation.OrganizationAccountId);
        if (threadInformation.OrganizationAccountId != accountOrganization?.Id)
            return new ServiceResult<bool>
            {
                IsSuccess = false,
                Data = false,
                ErrorMessage = LogPhrases.ServiceResult.Error.NOT_FOUND
            };

        try
        {
            _context.AiThreads.Remove(threadInformation);
            await _context.SaveChangesAsync(cancellationToken);
            return new ServiceResult<bool>
            {
                IsSuccess = true,
                Data = true
            };
        }
        catch (System.Exception err)
        {
            _logger.LogError(err.ToString());
            return new ServiceResult<bool>
            {
                IsSuccess = false,
                ErrorMessage = LogPhrases.ServiceResult.Error.FAILED_UNTRACE
            };
        }


    }

    public async Task<ServiceResult<List<AiThreadDetailsDto>>> GetAllThreadsByOrganizationAccountAsync(Guid organizationId,
                                                                                   Guid accountId,
                                                                                   CancellationToken cancellationToken)
    {
        var organizationAccountMap = await _context.OrganizationAccount
            .FirstOrDefaultAsync(o => o.AccountId == accountId
                                   && o.OrganizationId == organizationId
                                   , cancellationToken);

        if (organizationAccountMap is null)
        {
            return new ServiceResult<List<AiThreadDetailsDto>>
            {
                IsSuccess = false,
                ErrorMessage = LogPhrases.ServiceResult.Error.NOT_FOUND
            };
        }

        var threads = await _context.AiThreads.Where(t => t.OrganizationAccountId == organizationAccountMap.Id)
                                              .Select(t => new AiThreadDetailsDto
                                              {
                                                  Id = t.Id,
                                                  Name = t.Name,
                                                  OrganizationId = organizationAccountMap.OrganizationId,
                                                  AccountId = organizationAccountMap.AccountId,
                                                  CreateDate = t.CreateDate
                                              })
                                              .ToListAsync(cancellationToken);
        if (threads is null)
            return new ServiceResult<List<AiThreadDetailsDto>>
            {
                IsSuccess = false,
                ErrorMessage = LogPhrases.ServiceResult.Error.NOT_FOUND
            };

        return new ServiceResult<List<AiThreadDetailsDto>>
        {
            IsSuccess = true,
            Data = threads
        };
    }

    public async Task<ServiceResult<List<ChatMessageDto>>> GetChatHistoryByThread(Guid aiThread, CancellationToken cancellationToken)
    {
        var threadInformation = await _context.AiThreads.FirstOrDefaultAsync(t => t.Id == aiThread, cancellationToken);
        if (threadInformation is null)
            return new ServiceResult<List<ChatMessageDto>>
            {
                IsSuccess = false,
                ErrorMessage = LogPhrases.ServiceResult.Error.NOT_FOUND
            };

        var token = _configuration["OpenAI:ApiKey"];
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
        client.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v2");

        var messageResponse = await client.GetAsync($"https://api.openai.com/v1/threads/{threadInformation.Thread}/messages", cancellationToken);
        var jsonString = await messageResponse.Content.ReadAsStringAsync();
        var messagesData = JObject.Parse(jsonString);
        var messages = (JArray)messagesData["data"];

        return new ServiceResult<List<ChatMessageDto>>
        {
            IsSuccess = true,
            Data = ParseMessages(messages)
        };
    }

    public async Task<ServiceResult<AiThreadDetailsDto>> GetThreadInfoAsync(Guid aiThread, CancellationToken cancellationToken)
    {
        var thread = await _context.AiThreads.FirstOrDefaultAsync(t => t.Id == aiThread, cancellationToken);
        if (thread is null)
            return new ServiceResult<AiThreadDetailsDto>
            {
                IsSuccess = false,
                ErrorMessage = LogPhrases.ServiceResult.Error.NOT_FOUND
            };

        var organizationAccountIdThread = await _context.OrganizationAccount.FirstAsync(o => o.Id == thread.OrganizationAccountId
                                                                                                                , cancellationToken);
        if (thread.OrganizationAccountId != organizationAccountIdThread.Id)
            return null!;

        return new ServiceResult<AiThreadDetailsDto>
        {
            IsSuccess = true,
            Data = new AiThreadDetailsDto
            {
                Id = thread.Id,
                Name = thread.Name,
                OrganizationId = organizationAccountIdThread.OrganizationId,
                AccountId = organizationAccountIdThread.AccountId,
                CreateDate = thread.CreateDate
            }
        };
    }

    public async Task<ServiceResult<ChatMessageDto>> PostChatMessageAsync(ChatMessageDto userMessage,
                                                           AiThreadDetailsDto aiThread,
                                                           CancellationToken cancellationToken)
    {
        var thread = await _context.AiThreads.FirstOrDefaultAsync(t => t.Id == aiThread.Id);
        if (thread is null)
            return new ServiceResult<ChatMessageDto>
            {
                IsSuccess = false,
                ErrorMessage = LogPhrases.ServiceResult.Error.NOT_FOUND
            };

        var organizationAccountIdThread = await _context.OrganizationAccount.FirstAsync(o => o.Id == thread.OrganizationAccountId
                                                                                                                 , cancellationToken);

        var token = _configuration["OpenAI:ApiKey"];
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
        client.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v2");

        await client.PostAsJsonAsync($"https://api.openai.com/v1/threads/{thread.Thread}/messages", new
        {
            role = "user",
            content = userMessage.Content
        }, cancellationToken);

        var runResponse = await client.PostAsJsonAsync($"https://api.openai.com/v1/threads/{thread.Thread}/runs", new
        {
            assistant_id = _configuration["OpenAI:AssistantId"]
        }, cancellationToken);

        var run = await runResponse.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        var runId = run["id"].ToString();

        string status;
        do
        {
            await Task.Delay(500);
            var statusResponse = await client.GetAsync($"https://api.openai.com/v1/threads/{thread.Thread}/runs/{runId}", cancellationToken);
            var statusData = await statusResponse.Content.ReadFromJsonAsync<Dictionary<string, object>>(cancellationToken);
            status = statusData["status"].ToString();
        }
        while (status == "in_progress" || status == "queued");

        var messageResponse = await client.GetAsync($"https://api.openai.com/v1/threads/{thread.Thread}/messages", cancellationToken);
        var jsonString = await messageResponse.Content.ReadAsStringAsync(cancellationToken);
        var messagesData = JObject.Parse(jsonString);
        var messages = (JArray)messagesData["data"];

        if (messages is null)
            return new ServiceResult<ChatMessageDto>
            {
                IsSuccess = false,
                ErrorMessage = LogPhrases.ServiceResult.Error.FAILED_PARSED_JSON
            };

        var firstMessage = messages.First;
        var content = firstMessage["content"]?[0]?["text"]?["value"]?.ToString();

        return new ServiceResult<ChatMessageDto>
        {
            IsSuccess = true,
            Data = new ChatMessageDto
            {
                Role = "assistant",
                Content = content,
                IsAutomatedTicketCreationFlag = false
            }
        };
    }

    private List<ChatMessageDto> ParseMessages(JArray json)
    {
        var result = new List<ChatMessageDto>();

        foreach (var message in json)
        {
            string role = message["role"]?.ToString();
            long createdAtUnix = message["created_at"]?.Value<long>() ?? 0;
            DateTime? createdAt = DateTimeOffset.FromUnixTimeSeconds(createdAtUnix).UtcDateTime;

            var contentArray = message["content"] as JArray;
            string content = contentArray?.FirstOrDefault()?["text"]?["value"]?.ToString();

            if (!string.IsNullOrWhiteSpace(role) && !string.IsNullOrWhiteSpace(content))
            {
                result.Add(new ChatMessageDto
                {
                    Role = role,
                    Content = content,
                    IsAutomatedTicketCreationFlag = false,
                    CreateDate = createdAt
                });
            }
        }

        return result;
    }
}
