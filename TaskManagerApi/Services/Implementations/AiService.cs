using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using TaskManagerApi.Data;
using TaskManagerApi.Models.AI;
using TaskManagerApi.Services.Interfaces;

namespace TaskManagerApi.Services.Implementations;

public class AiService : IAiService
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly TaskManagerAPIDbContext _context;
    private readonly ILogger<AiService> _logger;

    public AiService(IConfiguration configuration,
                     IHttpClientFactory httpClientFactory,
                     TaskManagerAPIDbContext context,
                     ILogger<AiService> logger)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _context = context;
        _logger = logger;
    }

    public async Task<AiThreadDetailsDto> CreateNewThread(AiThreadDetailsDto aiThread)
    {
        var organizationAccountMap = await _context.OrganizationAccount
            .FirstOrDefaultAsync(o => o.AccountId == aiThread.AccountId
                                   && o.OrganizationId == aiThread.OrganizationId);

        if (organizationAccountMap is null)
            return null!;

        var token = _configuration["OpenAI:ApiKey"];
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
        client.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v2");

        var threadResponse = await client.PostAsJsonAsync("https://api.openai.com/v1/threads", (object)null);
        var threadParce = await threadResponse.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        var threadId = threadParce["id"].ToString();

        _context.AiThreads.Add(new Enitities.Ai.AiThreads
        {
            OrganizationAccountId = organizationAccountMap.Id,
            Name = aiThread.Name,
            Thread = threadId
        });

        await _context.SaveChangesAsync();

        var thread = await _context.AiThreads.FirstOrDefaultAsync(t => t.Thread == threadId);
        if (thread is null)
            return null;

        var organizationAccountIdThread = await _context.OrganizationAccount.FirstAsync(o => o.Id == thread.OrganizationAccountId);

        return new AiThreadDetailsDto
        {
            Id = thread.Id,
            Name = thread.Name,
            OrganizationId = organizationAccountIdThread.OrganizationId,
            AccountId = organizationAccountIdThread.AccountId,
            CreateDate = thread.CreateDate
        };
    }

    public async Task<List<AiThreadDetailsDto>> GetAllThreadsByOrganizationAccount(Guid organizationId, Guid accountId)
    {
        var organizationAccountMap = await _context.OrganizationAccount
            .FirstOrDefaultAsync(o => o.AccountId == accountId
                                   && o.OrganizationId == organizationId);

        if (organizationAccountMap is null)
            return null!;

        var threads = await _context.AiThreads.Where(t => t.OrganizationAccountId == organizationAccountMap.Id)
                                              .Select(t => new AiThreadDetailsDto
                                              {
                                                  Id = t.Id,
                                                  Name = t.Name,
                                                  OrganizationId = organizationAccountMap.OrganizationId,
                                                  AccountId = organizationAccountMap.AccountId,
                                                  CreateDate = t.CreateDate
                                              })
                                              .ToListAsync();
        if (threads is null)
            return null!;

        return threads;
    }

    public async Task<List<ChatMessageDto>> GetChatHistoryByThread(Guid aiThread)
    {
        var threadInformation = await _context.AiThreads.FirstOrDefaultAsync(t => t.Id == aiThread);
        if (threadInformation is null)
            return null!;

        var token = _configuration["OpenAI:ApiKey"];
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
        client.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v2");

        var messageResponse = await client.GetAsync($"https://api.openai.com/v1/threads/{threadInformation.Thread}/messages");
        var jsonString = await messageResponse.Content.ReadAsStringAsync();
        var messagesData = JObject.Parse(jsonString);
        var messages = (JArray)messagesData["data"];

        return ParseMessages(messages);
    }

    public async Task<AiThreadDetailsDto> GetThreadInfo(Guid aiThread)
    {
        var thread = await _context.AiThreads.FirstOrDefaultAsync(t => t.Id == aiThread);
        if (thread is null)
            return null;

        var organizationAccountIdThread = await _context.OrganizationAccount.FirstAsync(o => o.Id == thread.OrganizationAccountId);
        if (thread.OrganizationAccountId != organizationAccountIdThread.Id)
            return null!;

        return new AiThreadDetailsDto
        {
            Id = thread.Id,
            Name = thread.Name,
            OrganizationId = organizationAccountIdThread.OrganizationId,
            AccountId = organizationAccountIdThread.AccountId,
            CreateDate = thread.CreateDate
        };
    }

    public async Task<ChatMessageDto> PostChatMessageAsync(ChatMessageDto userMessage, AiThreadDetailsDto aiThread)
    {
        var thread = await _context.AiThreads.FirstOrDefaultAsync(t => t.Id == aiThread.Id);
        if (thread is null)
            return null;

        var organizationAccountIdThread = await _context.OrganizationAccount.FirstAsync(o => o.Id == thread.OrganizationAccountId);

        var token = _configuration["OpenAI:ApiKey"];
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
        client.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v2");

        await client.PostAsJsonAsync($"https://api.openai.com/v1/threads/{thread.Thread}/messages", new
        {
            role = "user",
            content = userMessage.Content
        });

        var runResponse = await client.PostAsJsonAsync($"https://api.openai.com/v1/threads/{thread.Thread}/runs", new
        {
            assistant_id = _configuration["OpenAI:AssistantId"]
        });

        var run = await runResponse.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        var runId = run["id"].ToString();

        string status;
        do
        {
            await Task.Delay(500);
            var statusResponse = await client.GetAsync($"https://api.openai.com/v1/threads/{thread.Thread}/runs/{runId}");
            var statusData = await statusResponse.Content.ReadFromJsonAsync<Dictionary<string, object>>();
            status = statusData["status"].ToString();
        }
        while (status == "in_progress" || status == "queued");

        var messageResponse = await client.GetAsync($"https://api.openai.com/v1/threads/{thread.Thread}/messages");
        var jsonString = await messageResponse.Content.ReadAsStringAsync();
        var messagesData = JObject.Parse(jsonString);
        var messages = (JArray)messagesData["data"];

        var firstMessage = messages.First;
        var content = firstMessage["content"]?[0]?["text"]?["value"]?.ToString();

        return new ChatMessageDto
        {
            Role = "assistant",
            Content = content,
            IsAutomatedTicketCreationFlag = false
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
