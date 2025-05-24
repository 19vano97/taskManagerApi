using System;
using TaskHistoryService.Data;
using TaskHistoryService.Enitities;
using TaskHistoryService.Models;
using TaskHistoryService.Services.Interfaces;

namespace TaskHistoryService.Services.Implementations;

public class HistoryService : IHistoryService
{
    private readonly TaskHistoryAPIDbContext _context;
    private readonly ILogger<HistoryService> _logger;

    public HistoryService(TaskHistoryAPIDbContext context, ILogger<HistoryService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Write(TaskHistoryDto history)
    {
        try
        {
            _context.TaskHistories.AddRange(ConvertFromDtoToEntityNoId(history));
            await _context.SaveChangesAsync();
        }
        catch (System.Exception ex)
        {
            _logger.LogError("Write history Service {0}", ex);
        }
    }

    private static TaskHistory ConvertFromDtoToEntityNoId(TaskHistoryDto historyDto)
    {
        return new TaskHistory
        {
            TaskId = historyDto.TaskId,
            Author = historyDto.Author,
            EventName = historyDto.EventName,
            PreviousState = historyDto.PreviousState,
            NewState = historyDto.NewState
        };
    }

    public void WriteNoSaveToDb(TaskHistoryDto history)
    {
        try
        {
            _context.TaskHistories.AddRange(ConvertFromDtoToEntityNoId(history));
        }
        catch (System.Exception ex)
        {
            _logger.LogError($"Write history Service {ex}");
        }
    }
}
