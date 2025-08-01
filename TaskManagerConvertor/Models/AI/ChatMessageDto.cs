using System;
using System.ComponentModel.DataAnnotations;

namespace TaskManagerConvertor.Models.AI;

public class ChatMessageDto
{
    [Required]
    [StringLength(50)]
    public string Role { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Content { get; set; } = string.Empty;

    [Required]
    public bool IsAutomatedTicketCreationFlag { get; set; }

    public DateTime? CreateDate { get; set; }
}
