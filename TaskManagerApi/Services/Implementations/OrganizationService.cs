using System;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TaskManagerApi.Data;
using TaskManagerApi.Enitities.Organization;
using TaskManagerApi.Models;
using TaskManagerApi.Models.OrganizationModel;
using TaskManagerApi.Models.Project;
using TaskManagerApi.Services.Interfaces;
using static TaskManagerApi.Models.Constants;

namespace TaskManagerApi.Services.Implementations;

public class OrganizationService : IOrganizationService
{
    private readonly TicketManagerAPIDbContext _context;
    private readonly ILogger<OrganizationService> _logger;
    private IProjectService _projectService;

    public OrganizationService(TicketManagerAPIDbContext context,
                               IProjectService projectService,
                               ILogger<OrganizationService> logger)
    {
        _context = context;
        _projectService = projectService;
        _logger = logger;
    }

    public async Task<ServiceResult<OrganizationDto>> CreateAsync(ClaimsPrincipal user,
                                                   OrganizationDto newOgranization,
                                                   CancellationToken cancellationToken)
    {
        newOgranization.Id = Guid.NewGuid();
        newOgranization.Owner = Guid.Parse(user.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID).Value);

        _context.OrganizationItem.Add(ConvertToEntity(newOgranization));
        await _context.SaveChangesAsync(cancellationToken);

        _context.OrganizationAccount.Add(new OrganizationAccount
        {
            AccountId = newOgranization.Owner,
            OrganizationId = _context.OrganizationItem.First(o => o.Id == newOgranization.Id).Id
        });

        await _context.SaveChangesAsync(cancellationToken);

        var addedNewOrganization = await _context.OrganizationItem.FirstOrDefaultAsync(o => o.Id == newOgranization.Id
                                                                                                    , cancellationToken);

        if (addedNewOrganization is not null)
            return new ServiceResult<OrganizationDto>
            {
                Success = true,
                Data = ConvertToDto(addedNewOrganization)
            };

        return new ServiceResult<OrganizationDto>
        {
            Success = false,
            ErrorMessage = LogPhrases.ServiceResult.Error.FAILED_UNTRACE
        };
    }

    public async Task<ServiceResult<OrganizationDto>> DeleteAsync(OrganizationDto organizationToDelete, CancellationToken cancellationToken)
    {
        var toDelete = await DoesOrganizationExistEntity(Id: organizationToDelete.Id.ToString());

        if (organizationToDelete.Id == Guid.Empty && organizationToDelete.Name == null && organizationToDelete.Abbreviation == null)
            return new ServiceResult<OrganizationDto>
            {
                Success = false,
                ErrorMessage = LogPhrases.ServiceResult.Error.NOT_FOUND
            };

        if (!toDelete.Success)
            return new ServiceResult<OrganizationDto>
            {
                Success = toDelete.Success,
                ErrorMessage = toDelete.ErrorMessage
            };

        try
        {
            _context.OrganizationItem.Remove(toDelete.Data);
            await _context.SaveChangesAsync(cancellationToken);

            return new ServiceResult<OrganizationDto>
            {
                Success = true,
            };
        }
        catch (System.Exception ex)
        {
            Log.Error(ex.ToString());
            _logger.LogError(ex.ToString());
            return new ServiceResult<OrganizationDto>
            {
                Success = false,
                ErrorMessage = ex.ToString()
            };
        }
    }

    public async Task<ServiceResult<OrganizationDto>> EditAsync(ClaimsPrincipal user,
                                                 OrganizationDto organizationToEdit,
                                                 CancellationToken cancellationToken)
    {
        var initialOrganization = await DoesOrganizationExistEntity(Id: organizationToEdit.Id.ToString());
        var accountId = Guid.Parse(user.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID).Value);

        if ((organizationToEdit.Id == Guid.Empty
                && organizationToEdit.Name == null
                && organizationToEdit.Abbreviation == null)
            || !initialOrganization.Success
            || accountId != initialOrganization.Data?.Owner)
            return new ServiceResult<OrganizationDto>
            {
                Success = false,
                ErrorMessage = LogPhrases.ServiceResult.Error.NOT_FOUND
            };

        initialOrganization.Data.Name = organizationToEdit.Name;
        initialOrganization.Data.Owner = organizationToEdit.Owner;
        initialOrganization.Data.Abbreviation = organizationToEdit.Abbreviation;
        initialOrganization.Data.Description = organizationToEdit.Description;
        initialOrganization.Data.ModifyDate = DateTime.UtcNow;

        _context.OrganizationItem.Update(initialOrganization.Data);
        await _context.SaveChangesAsync(cancellationToken);

        var res = await DoesOrganizationExistEntity(Id: organizationToEdit.Id.ToString());

        if (!res.Success)
            return new ServiceResult<OrganizationDto>
            {
                Success = res.Success,
                ErrorMessage = res.ErrorMessage
            };

        return new ServiceResult<OrganizationDto>
        {
            Success = true,
            Data = ConvertToDto(res.Data)
        };
    }

    public async Task<ServiceResult<OrganizationProjectDto>> GetOrganizationProjectsAsync(Guid organizationId, CancellationToken cancellationToken)
    {
        var organization = await DoesOrganizationExist(organizationId, cancellationToken);
        if (!organization.Success)
            return new ServiceResult<OrganizationProjectDto>
            {
                Success = false,
                ErrorMessage = LogPhrases.ServiceResult.Error.NOT_FOUND
            };

        var projects = await _projectService.GetProjectsByOrganizationIdAsync(organizationId, cancellationToken);

        var accounts = await _context.OrganizationAccount
            .Where(o => o.OrganizationId == organizationId)
            .Select(o => o.AccountId)
            .ToListAsync(cancellationToken);

        return new ServiceResult<OrganizationProjectDto>
        {
            Success = true,
            Data = new OrganizationProjectDto
            {
                Id = organization.Data!.Id,
                Name = organization.Data!.Name,
                Abbreviation = organization.Data!.Abbreviation,
                Owner = organization.Data!.Owner,
                Description = organization.Data!.Description,
                Accounts = accounts,
                CreateDate = organization.Data!.CreateDate,
                ModifyDate = organization.Data!.ModifyDate,
                Projects = projects.Data!
            }
        };

    }

    public async Task<ServiceResult<List<OrganizationProjectDto>>> GetOrganizationsByAccountAsync(Guid accountId, CancellationToken cancellationToken)
    {
        var organizations = await _context.OrganizationAccount.Where(o => o.AccountId == accountId)
            .Select(o => o.OrganizationId)
            .ToListAsync(cancellationToken);

        var result = new List<OrganizationProjectDto>();
        foreach (var organizationId in organizations)
        {
            var org = await GetOrganizationProjectsAsync(organizationId, cancellationToken);
            if (!org.Success) continue;

            result.Add(org.Data!);
        }

        return new ServiceResult<List<OrganizationProjectDto>>
        {
            Success = true,
            Data = result
        };
    }

    public async Task<ServiceResult<OrganizationDto>> GetOrganizationAsync(Guid organizationId, CancellationToken cancellationToken)
    {
        var org = await DoesOrganizationExistEntity(Id: organizationId.ToString());
        if (!org.Success)
            return new ServiceResult<OrganizationDto>
            {
                Success = org.Success,
                ErrorMessage = org.ErrorMessage
            };

        return new ServiceResult<OrganizationDto>
        {
            Success = true,
            Data = ConvertToDto(org.Data!)
        };
    }

    public async Task<ServiceResult<OrganizationAccountsDto>> GetOrganizationAccountAsync(string organizationId, CancellationToken cancellationToken)
    {
        var organization = await DoesOrganizationExistEntity(Id: organizationId);
        if (!organization.Success)
            return new ServiceResult<OrganizationAccountsDto>
            {
                Success = organization.Success,
                ErrorMessage = organization.ErrorMessage
            };

        var accounts = await _context.OrganizationAccount
            .Where(o => o.OrganizationId == organization.Data.Id)
            .Select(o => o.AccountId)
            .ToListAsync(cancellationToken);

        return new ServiceResult<OrganizationAccountsDto>
        {
            Success = true,
            Data = new OrganizationAccountsDto
            {
                Id = organization.Data!.Id,
                Name = organization.Data!.Name,
                Abbreviation = organization.Data!.Abbreviation,
                Owner = organization.Data!.Owner,
                Description = organization.Data!.Description,
                CreateDate = organization.Data!.CreateDate,
                ModifyDate = organization.Data!.ModifyDate,
                Accounts = accounts
            }
        };
    }

    public async Task<ServiceResult<OrganizationProjectDto>> AddNewMemberToOrganization(Guid organizationId, Guid accountId, CancellationToken cancellationToken)
    {
        if (await _context.OrganizationAccount.FirstOrDefaultAsync(a => a.AccountId == accountId && a.OrganizationId == organizationId) is not null)
            return await GetOrganizationProjectsAsync(organizationId, cancellationToken);

        _context.OrganizationAccount.Add(new OrganizationAccount { AccountId = accountId, OrganizationId = organizationId });
        await _context.SaveChangesAsync(cancellationToken);

        var res = await _context.OrganizationAccount.FirstOrDefaultAsync(a => a.AccountId == accountId && a.OrganizationId == organizationId);
        if (res is null)
            return new ServiceResult<OrganizationProjectDto>
            {
                Success = false,
                ErrorMessage = LogPhrases.ServiceResult.Error.NOT_FOUND
            };


        return await GetOrganizationProjectsAsync(organizationId, cancellationToken);
    }

    private async Task<ServiceResult<OrganizationItem>> DoesOrganizationExist(Guid Id, CancellationToken cancellationToken)
    {
        var res = await _context.OrganizationItem.FirstOrDefaultAsync(o => o.Id == Id, cancellationToken);
        if (res is null)
            return new ServiceResult<OrganizationItem>
            {
                Success = false,
                ErrorMessage = LogPhrases.ServiceResult.Error.NOT_FOUND
            };

        return new ServiceResult<OrganizationItem>
        {
            Success = true,
            Data = res
        };
    }

    private async Task<ServiceResult<OrganizationItem>> DoesOrganizationExistEntity(OrganizationItem entity = null, string Id = null)
    {
        if (Id != null)
            return new ServiceResult<OrganizationItem>
            {
                Success = true,
                Data = await _context.OrganizationItem.FirstOrDefaultAsync(o => o.Id == Guid.Parse(Id))
            };

        if (entity.Id != null)
            return new ServiceResult<OrganizationItem>
            {
                Success = true,
                Data = await _context.OrganizationItem.FirstOrDefaultAsync(o => o.Id == entity.Id)
            };


        if (entity.Abbreviation != null)
            return new ServiceResult<OrganizationItem>
            {
                Success = true,
                Data = await _context.OrganizationItem.FirstOrDefaultAsync(o => o.Abbreviation == entity.Abbreviation)
            };


        return new ServiceResult<OrganizationItem>
        {
            Success = false,
            ErrorMessage = LogPhrases.ServiceResult.Error.NOT_FOUND
        };
    }

    private OrganizationDto ConvertToDto(OrganizationItem item)
    {
        if (item.Id == null && item.Name == null && item.Abbreviation == null)
            return null;

        return new OrganizationDto
        {
            Id = item.Id,
            Name = item.Name,
            Owner = item.Owner,
            Abbreviation = item.Abbreviation,
            Description = item.Description,
            CreateDate = item.CreateDate,
            ModifyDate = item.ModifyDate
        };
    }

    private OrganizationItem ConvertToEntity(OrganizationDto dto)
    {
        if (dto.Id == Guid.Empty && dto.Name == null && dto.Abbreviation == null)
            return null;

        return new OrganizationItem
        {
            Id = dto.Id,
            Name = dto.Name,
            Owner = dto.Owner,
            Abbreviation = dto.Abbreviation,
            Description = dto.Description,
            CreateDate = dto.CreateDate,
            ModifyDate = dto.ModifyDate
        };
    }
}
