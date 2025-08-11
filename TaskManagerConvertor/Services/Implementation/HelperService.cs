using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using TaskManagerConvertor.Models;
using TaskManagerConvertor.Services.Interfaces;

namespace TaskManagerConvertor.Services.Implementation;

public class HelperService : IHelperService
{
    private readonly ILogger<HelperService> _logger;

    public HelperService(ILogger<HelperService> logger)
    {
        _logger = logger;
    }

    public bool TryParseJsonToDto<T>(string data, out T? type)
    {
        try
        {
            type = JsonConvert.DeserializeObject<T>(data)!;
            return true;
        }
        catch (System.Exception ex)
        {
            _logger.LogError($"Error with parcing json to {typeof(T)}: {ex.ToString()}");
            type = default;

            return false;
        }
    }
}
