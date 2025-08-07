using System;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using TaskManagerConvertor.Models;
using TaskManagerConvertor.Models.Organization;
using TaskManagerConvertor.Models.Project;
using TaskManagerConvertor.Models.Ticket;
using TaskManagerConvertor.Models.TicketItem;
using TaskManagerConvertor.Services.Interfaces;

namespace TaskManagerConvertor.Services.Implementation;

public class AccountHelperService : IAccountHelperService
{
    private readonly IAccountService _accountService;
    private readonly ILogger<AccountHelperService> _logger;

    public AccountHelperService(IAccountService accountService, ILogger<AccountHelperService> logger)
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
            if (ticket.ChildIssues is not null)
            {
                accountsToGet = ticket.ChildIssues.SelectMany(s => new Guid?[] { s.AssigneeId, s.ReporterId })
                                                .Where(id => id.HasValue)
                                                .Select(id => id!.Value)
                                                .Distinct()
                                                .ToList();
                accountsToGet.AddRange([(Guid)ticket.AssigneeId!, (Guid)ticket.ReporterId!]);
            }
            else
            {
                accountsToGet = [(Guid)ticket.AssigneeId!, (Guid)ticket.ReporterId!];
            }
        }
        else if (type is OrganizationDto organization)
        {
            accountsToGet = organization.AccountIds;
        }
        else if (type is ProjectItemDto project)
        {
            accountsToGet = [project.OwnerId];
        }
        else if (type is List<TicketCommentDto> ticketCommentDto)
        {
            accountsToGet = ticketCommentDto!.SelectMany(s => new Guid?[] { s.AccountId })
                                             .Where(id => id.HasValue)
                                             .Select(id => id!.Value)
                                             .Distinct()
                                             .ToList();
        }
        else
        {
            return type;
        }

        if (accountsToGet is null || accountsToGet.Count == 0)
            return type;

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

                if (ticketInner.ChildIssues is not null)
                {
                    foreach (var ticket in ticketInner.ChildIssues)
                    {
                        ticket.Assignee = accounts.Data?.FirstOrDefault(a => a.Id == ticket.AssigneeId);
                        ticket.Reporter = accounts.Data?.FirstOrDefault(a => a.Id == ticket.ReporterId);
                    }
                }
            }
            else if (type is List<TicketHistoryDto> historyListInner)
            {
                foreach (var item in historyListInner!)
                {
                    item.Author = accounts.Data?.FirstOrDefault(a => a.Id == item.AuthorId);
                }
            }
            else if (type is OrganizationDto OrganizationInner)
            {
                OrganizationInner.Accounts = accounts.Data!;
                OrganizationInner.Owner = accounts.Data!.First(s => s.Id == OrganizationInner.OwnerId);
            }
            else if (type is ProjectItemDto projectInner)
            {
                projectInner.Owner = accounts.Data!.First(o => o.Id == projectInner.OwnerId);
            }
            else if (type is List<TicketCommentDto> ticketCommentDtoInner)
            {
                foreach (var item in ticketCommentDtoInner)
                {
                    item.Account = accounts.Data!.First(o => o.Id == item.AccountId);
                }
            }
        }

        return type;
    }
}
