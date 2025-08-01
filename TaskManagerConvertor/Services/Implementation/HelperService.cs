using System;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using TaskManagerConvertor.Models;
using TaskManagerConvertor.Models.Ticket;
using TaskManagerConvertor.Models.TicketItem;
using TaskManagerConvertor.Services.Interfaces;

namespace TaskManagerConvertor.Services.Implementation;

public class HelperService : IHelperService
{
    private readonly IAccountService _accountService;
    private readonly ILogger<HelperService> _logger;

    public HelperService(IAccountService accountService, ILogger<HelperService> logger)
    {
        _accountService = accountService;
        _logger = logger;
    }

    public async Task<T?> AddAccountDetails<T>(IHeaderDictionary headers, T? type, CancellationToken cancellationToken)
    {
        var accountsToGet = new List<Guid>();

        if (type is List<TicketDto> ticketList)
        {
            accountsToGet = ticketList.SelectMany(s => new Guid?[] { s.AssigneeId, s.ReporterId })
                                      .Where(id => id.HasValue)
                                      .Select(id => id!.Value)
                                      .Distinct()
                                      .ToList();
        }
        else if (type is List<TicketHistoryDto> historyList)
        {
            accountsToGet = historyList!.SelectMany(s => new Guid?[] { s.AuthorId })
                                        .Where(id => id.HasValue)
                                        .Select(id => id!.Value)
                                        .Distinct()
                                        .ToList();
        }
        else if (type is TicketDto ticket)
        {
            accountsToGet = [(Guid)ticket.AssigneeId!, (Guid)ticket.ReporterId!];
        }

        var accounts = await _accountService.GetAccountDetailsByIds(headers,
                                                                    accountsToGet,
                                                                    cancellationToken);

        if (accounts.IsSuccess)
        {
            if (type is List<TicketDto> ticketListInner)
            {
                foreach (var ticket in ticketListInner!)
                {
                    ticket.Assignee = accounts.Data?.FirstOrDefault(a => a.Id == ticket.AssigneeId);
                    ticket.Reporter = accounts.Data?.FirstOrDefault(a => a.Id == ticket.ReporterId);
                }
            }
            else if (type is TicketDto ticketInner)
            {
                ticketInner.Assignee = accounts.Data?.FirstOrDefault(a => a.Id == ticketInner.AssigneeId);
                ticketInner.Reporter = accounts.Data?.FirstOrDefault(a => a.Id == ticketInner.ReporterId);
            }
            else if (type is List<TicketHistoryDto> historyListInner)
            {
                foreach (var item in historyListInner!)
                {
                    item.Author = accounts.Data?.FirstOrDefault(a => a.Id == item.AuthorId);
                }
            }
        }

        return type;
    }

    public RequestResult<HttpClient> SetupHttpClientForIdentityServer(IHeaderDictionary headers, ref HttpClient httpClient)
    {
        if (headers.TryGetValue(Constants.Settings.Header.AUTHORIZATION, out var authHeader))
        {
            var authHeaderValue = authHeader.ToString();
            var parts = authHeaderValue.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2)
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(parts[0], parts[1]);
            }
            else
            {
                return new RequestResult<HttpClient>
                {
                    IsSuccess = false,
                    ErrorMessage = "Invalid Authorization header format"
                };
            }
        }
        else
        {
            return new RequestResult<HttpClient>
            {
                IsSuccess = false,
                ErrorMessage = "Issue with organzation or token"
            };
        }

        return new RequestResult<HttpClient>
        {
            IsSuccess = true,
            Data = httpClient
        };
    }

    public RequestResult<HttpClient> SetupHttpClientForTaskManager(IHeaderDictionary headers, ref HttpClient httpClient)
    {
        if (headers.TryGetValue(Constants.Settings.Header.AUTHORIZATION, out var authHeader)
            && headers.TryGetValue(Constants.Settings.Header.ORGANIZATION, out var organizationIdString))
        {
            var authHeaderValue = authHeader.ToString();
            var parts = authHeaderValue.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2)
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(parts[0], parts[1]);
            }
            else
            {
                return new RequestResult<HttpClient>
                {
                    IsSuccess = false,
                    ErrorMessage = "Invalid Authorization header format"
                };
            }

            httpClient.DefaultRequestHeaders.Add(Constants.Settings.Header.ORGANIZATION, organizationIdString.ToString());
        }
        else
        {
            return new RequestResult<HttpClient>
            {
                IsSuccess = false,
                ErrorMessage = "Issue with organzation or token"
            };
        }

        return new RequestResult<HttpClient>
        {
            IsSuccess = true,
            Data = httpClient
        };
    }

    public bool TryParseJsonToDto<T>(string data, out T? accountDto)
    {
        try
        {
            accountDto = JsonConvert.DeserializeObject<T>(data)!;
            return true;
        }
        catch (System.Exception ex)
        {
            _logger.LogError("Error with parcing json to ticketDto: {0}", ex.ToString());
            accountDto = default;

            return false;
        }
    }
}
