using System;
using Microsoft.EntityFrameworkCore;
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

    public async Task<List<TaskHistoryDto>> GetHistoryByTaskId(Guid taskId)
    {
        var historyList = new List<TaskHistoryDto>();
        var history = await _context.TaskHistories.Where(t => t.TaskId == taskId)
                                           .ToListAsync();

        foreach (var item in history)
        {
            historyList.Add(ConvertFromEntityToDto(item));
        }

        return historyList;
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

    private static TaskHistoryDto ConvertFromEntityToDto(TaskHistory taskHistory)
    {
        return new TaskHistoryDto
        {
            Id = taskHistory.Id,
            TaskId = taskHistory.TaskId,
            EventName = taskHistory.EventName,
            PreviousState = taskHistory.PreviousState,
            NewState = taskHistory.NewState,
            Author = taskHistory.Author,
            CreateDate = taskHistory.CreateDate,
            ModifyDate = taskHistory.ModifyDate
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
