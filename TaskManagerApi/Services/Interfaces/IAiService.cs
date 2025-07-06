using System;
using TaskManagerApi.Models.AI;

namespace TaskManagerApi.Services.Interfaces;

public interface IAiService
{
    Task<ChatMessageDto> PostChatMessageAsync(ChatMessageDto userMessage, AiThreadDetailsDto aiThread);
    Task<List<ChatMessageDto>> GetChatHistoryByThread(Guid aiThread);
    Task<AiThreadDetailsDto> CreateNewThread(AiThreadDetailsDto aiThread);
    Task<AiThreadDetailsDto> GetThreadInfo(Guid aiThread);
    Task<List<AiThreadDetailsDto>> GetAllThreadsByOrganizationAccount(Guid organizationId, Guid accountId);
}
