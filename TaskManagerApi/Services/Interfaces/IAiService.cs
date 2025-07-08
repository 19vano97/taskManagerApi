using System;
using TaskManagerApi.Models;
using TaskManagerApi.Models.AI;

namespace TaskManagerApi.Services.Interfaces;

public interface IAiService
{
    Task<ServiceResult<ChatMessageDto>> PostChatMessageAsync(ChatMessageDto userMessage, AiThreadDetailsDto aiThread, CancellationToken cancellationToken);
    Task<ServiceResult<List<ChatMessageDto>>> GetChatHistoryByThread(Guid aiThread, CancellationToken cancellationToken);
    Task<ServiceResult<AiThreadDetailsDto>> CreateNewThread(AiThreadDetailsDto aiThread, CancellationToken cancellationToken);
    Task<ServiceResult<AiThreadDetailsDto>> GetThreadInfo(Guid aiThread, CancellationToken cancellationToken);
    Task<ServiceResult<List<AiThreadDetailsDto>>> GetAllThreadsByOrganizationAccount(Guid organizationId, Guid accountId, CancellationToken cancellationToken);
}
