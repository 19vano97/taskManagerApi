using System;
using TaskManagerConvertor.Models;
using TaskManagerConvertor.Models.AI;

namespace TaskManagerConvertor.Services.Interfaces;

public interface IAIService
{
    Task<RequestResult<ChatMessageDto>> ChatWithUserAsync(ChatMessageDto message, Guid threadId, CancellationToken cancellationToken);
    Task<RequestResult<List<ChatMessageDto>>> GetChatHistoryByThreadIdAsync(Guid aiThread, CancellationToken cancellationToken);
    Task<RequestResult<AiThreadDetailsDto>> CreateNewThreadAsync(AiThreadDetailsDto aiThread, CancellationToken cancellationToken);
    Task<RequestResult<AiThreadDetailsDto>> DeleteThreadAsync(Guid threadId, CancellationToken cancellationToken);
    Task<RequestResult<List<AiThreadDetailsDto>>> GetAllThreadsAsync(CancellationToken cancellationToken);
    Task<RequestResult<AiThreadDetailsDto>> GetThreadInfoAsync(Guid aiThread, CancellationToken cancellationToken);
}
