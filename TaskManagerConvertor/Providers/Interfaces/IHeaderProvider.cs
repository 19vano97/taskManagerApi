using System;

namespace TaskManagerConvertor.Providers.Interfaces;

public interface IHeaderProvider
{
    IDictionary<string, string> GetHeaders();
}
