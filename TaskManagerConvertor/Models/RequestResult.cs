using System;

namespace TaskManagerConvertor.Models;

public class RequestResult<T>
{
    public required bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }
}
