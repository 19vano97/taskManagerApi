using System;

namespace TaskManagerApi.Models.AI;

public class ChatMessageDto
{
    public required string Role { get; set; }
    public required string Content { get; set; }
    public required bool IsAutomatedTicketCreationFlag { get; set; }
    public DateTime? CreateDate { get; set; }
}
