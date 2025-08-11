using System.Net.Http;
using TaskManagerConvertor.Models;

namespace TaskManagerConvertor.Services.Interfaces;

public interface IHelperService
{
    bool TryParseJsonToDto<T>(string data, out T? type);
}
