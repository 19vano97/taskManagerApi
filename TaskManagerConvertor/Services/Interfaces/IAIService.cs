using System;
using TaskManagerConvertor.Models;
using TaskManagerConvertor.Models.AI;

namespace TaskManagerConvertor.Services.Interfaces;

public interface IAIService
{
    Task<RequestResult<ChatMessageDto>> ChatWithUserAsync(IHeaderDictionary headers, ChatMessageDto message, Guid threadId, CancellationToken cancellationToken);
    Task<RequestResult<List<ChatMessageDto>>> GetChatHistoryByThreadIdAsync(IHeaderDictionary headers, Guid aiThread, CancellationToken cancellationToken);
    Task<RequestResult<AiThreadDetailsDto>> CreateNewThreadAsync(IHeaderDictionary headers, AiThreadDetailsDto aiThread, CancellationToken cancellationToken);
    Task<RequestResult<AiThreadDetailsDto>> DeleteThreadAsync(IHeaderDictionary headers, Guid threadId, CancellationToken cancellationToken);
    Task<RequestResult<List<AiThreadDetailsDto>>> GetAllThreadsAsync(IHeaderDictionary headers, CancellationToken cancellationToken);
    Task<RequestResult<AiThreadDetailsDto>> GetThreadInfoAsync(IHeaderDictionary headers, Guid aiThread, CancellationToken cancellationToken);
}
