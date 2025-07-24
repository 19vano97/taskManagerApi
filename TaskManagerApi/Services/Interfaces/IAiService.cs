using System;
using TaskManagerApi.Models;
using TaskManagerApi.Models.AI;

namespace TaskManagerApi.Services.Interfaces;

public interface IAiService
{
    Task<ServiceResult<ChatMessageDto>> PostChatMessageAsync(ChatMessageDto userMessage, AiThreadDetailsDto aiThread, CancellationToken cancellationToken);
    Task<ServiceResult<List<ChatMessageDto>>> GetChatHistoryByThread(Guid aiThread, CancellationToken cancellationToken);
    Task<ServiceResult<AiThreadDetailsDto>> CreateNewThreadAsync(AiThreadDetailsDto aiThread, CancellationToken cancellationToken);
    Task<ServiceResult<bool>> DeleteThreadAsync(Guid threadId, Guid accountId, CancellationToken cancellationToken);
    Task<ServiceResult<AiThreadDetailsDto>> GetThreadInfoAsync(Guid aiThread, CancellationToken cancellationToken);
    Task<ServiceResult<List<AiThreadDetailsDto>>> GetAllThreadsByOrganizationAccountAsync(Guid organizationId, Guid accountId, CancellationToken cancellationToken);
}
