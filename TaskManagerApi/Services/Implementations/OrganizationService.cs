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
        newOgranization.OwnerId = Guid.Parse(user.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID).Value);

        _context.OrganizationItem.Add(ConvertToEntity(newOgranization));
        await _context.SaveChangesAsync(cancellationToken);

        _context.OrganizationAccount.Add(new OrganizationAccount
        {
            AccountId = newOgranization.OwnerId,
            OrganizationId = _context.OrganizationItem.First(o => o.Id == newOgranization.Id).Id
        });

        await _context.SaveChangesAsync(cancellationToken);

        var addedNewOrganization = await _context.OrganizationItem.FirstOrDefaultAsync(o => o.Id == newOgranization.Id
                                                                                                    , cancellationToken);

        if (addedNewOrganization is not null)
            return new ServiceResult<OrganizationDto>
            {
                IsSuccess = true,
                Data = ConvertToDto(addedNewOrganization)
            };

        return new ServiceResult<OrganizationDto>
        {
            IsSuccess = false,
            ErrorMessage = LogPhrases.ServiceResult.Error.FAILED_UNTRACE
        };
    }

    public async Task<ServiceResult<OrganizationDto>> DeleteAsync(OrganizationDto organizationToDelete, CancellationToken cancellationToken)
    {
        var toDelete = await DoesOrganizationExistEntity(Id: (Guid)organizationToDelete.Id!);

        if (organizationToDelete.Id == Guid.Empty && organizationToDelete.Name == null && organizationToDelete.Abbreviation == null)
            return new ServiceResult<OrganizationDto>
            {
                IsSuccess = false,
                ErrorMessage = LogPhrases.ServiceResult.Error.NOT_FOUND
            };

        if (!toDelete.IsSuccess)
            return new ServiceResult<OrganizationDto>
            {
                IsSuccess = toDelete.IsSuccess,
                ErrorMessage = toDelete.ErrorMessage
            };

        try
        {
            _context.OrganizationItem.Remove(toDelete.Data);
            await _context.SaveChangesAsync(cancellationToken);

            return new ServiceResult<OrganizationDto>
            {
                IsSuccess = true,
            };
        }
        catch (System.Exception ex)
        {
            Log.Error(ex.ToString());
            _logger.LogError(ex.ToString());
            return new ServiceResult<OrganizationDto>
            {
                IsSuccess = false,
                ErrorMessage = ex.ToString()
            };
        }
    }

    public async Task<ServiceResult<OrganizationDto>> EditAsync(ClaimsPrincipal user,
                                                 OrganizationDto organizationToEdit,
                                                 CancellationToken cancellationToken)
    {
        var initialOrganization = await DoesOrganizationExistEntity(Id: (Guid)organizationToEdit.Id!);
        var accountId = Guid.Parse(user.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID).Value);

        if ((organizationToEdit.Id == Guid.Empty
                && organizationToEdit.Name == null
                && organizationToEdit.Abbreviation == null)
            || !initialOrganization.IsSuccess
            || accountId != initialOrganization.Data?.Owner)
            return new ServiceResult<OrganizationDto>
            {
                IsSuccess = false,
                ErrorMessage = LogPhrases.ServiceResult.Error.NOT_FOUND
            };

        initialOrganization.Data.Name = organizationToEdit.Name;
        initialOrganization.Data.Owner = organizationToEdit.OwnerId;
        initialOrganization.Data.Abbreviation = organizationToEdit.Abbreviation;
        initialOrganization.Data.Description = organizationToEdit.Description;
        initialOrganization.Data.ModifyDate = DateTime.UtcNow;

        _context.OrganizationItem.Update(initialOrganization.Data);
        await _context.SaveChangesAsync(cancellationToken);

        var res = await DoesOrganizationExistEntity(Id: (Guid)organizationToEdit.Id!);

        if (!res.IsSuccess)
            return new ServiceResult<OrganizationDto>
            {
                IsSuccess = res.IsSuccess,
                ErrorMessage = res.ErrorMessage
            };

        return new ServiceResult<OrganizationDto>
        {
            IsSuccess = true,
            Data = ConvertToDto(res.Data)
        };
    }

    public async Task<ServiceResult<OrganizationDto>> GetOrganizationByIdAsync(Guid organizationId, CancellationToken cancellationToken)
    {
        var organization = await DoesOrganizationExist(organizationId, cancellationToken);
        if (!organization.IsSuccess)
            return new ServiceResult<OrganizationDto>
            {
                IsSuccess = false,
                ErrorMessage = LogPhrases.ServiceResult.Error.NOT_FOUND
            };

        var projects = await _projectService.GetProjectsByOrganizationIdAsync(organizationId, cancellationToken);

        var accounts = await _context.OrganizationAccount
            .Where(o => o.OrganizationId == organizationId)
            .Select(o => o.AccountId)
            .ToListAsync(cancellationToken);

        return new ServiceResult<OrganizationDto>
        {
            IsSuccess = true,
            Data = new OrganizationDto
            {
                Id = organization.Data!.Id,
                Name = organization.Data!.Name,
                Abbreviation = organization.Data!.Abbreviation,
                OwnerId = organization.Data!.Owner,
                Description = organization.Data!.Description,
                AccountIds = accounts,
                CreateDate = organization.Data!.CreateDate,
                ModifyDate = organization.Data!.ModifyDate,
                Projects = projects.Data!
            }
        };

    }

    public async Task<ServiceResult<List<OrganizationDto>>> GetOrganizationsByAccountAsync(Guid accountId, CancellationToken cancellationToken)
    {
        var organizations = await _context.OrganizationAccount.Where(o => o.AccountId == accountId)
            .Select(o => o.OrganizationId)
            .ToListAsync(cancellationToken);

        var result = new List<OrganizationDto>();
        foreach (var organizationId in organizations)
        {
            var org = await GetOrganizationByIdAsync(organizationId, cancellationToken);
            if (!org.IsSuccess) continue;

            result.Add(org.Data!);
        }

        return new ServiceResult<List<OrganizationDto>>
        {
            IsSuccess = true,
            Data = result
        };
    }

    public async Task<ServiceResult<OrganizationDto>> GetOrganizationAsync(Guid organizationId, CancellationToken cancellationToken)
    {
        var org = await DoesOrganizationExistEntity(Id: organizationId);
        if (!org.IsSuccess)
            return new ServiceResult<OrganizationDto>
            {
                IsSuccess = org.IsSuccess,
                ErrorMessage = org.ErrorMessage
            };

        return new ServiceResult<OrganizationDto>
        {
            IsSuccess = true,
            Data = ConvertToDto(org.Data!)
        };
    }

    public async Task<ServiceResult<OrganizationDto>> GetOrganizationAccountAsync(Guid organizationId, CancellationToken cancellationToken)
    {
        var organization = await DoesOrganizationExistEntity(Id: organizationId);
        if (!organization.IsSuccess)
            return new ServiceResult<OrganizationDto>
            {
                IsSuccess = organization.IsSuccess,
                ErrorMessage = organization.ErrorMessage
            };

        var accounts = await _context.OrganizationAccount
            .Where(o => o.OrganizationId == organization.Data!.Id)
            .Select(o => o.AccountId)
            .ToListAsync(cancellationToken);

        return new ServiceResult<OrganizationDto>
        {
            IsSuccess = true,
            Data = new OrganizationDto
            {
                Id = organization.Data!.Id,
                Name = organization.Data!.Name,
                Abbreviation = organization.Data!.Abbreviation,
                OwnerId = organization.Data!.Owner,
                Description = organization.Data!.Description,
                CreateDate = organization.Data!.CreateDate,
                ModifyDate = organization.Data!.ModifyDate,
                AccountIds = accounts
            }
        };
    }

    public async Task<ServiceResult<OrganizationDto>> AddNewMemberToOrganization(Guid organizationId, Guid accountId, CancellationToken cancellationToken)
    {
        if (await _context.OrganizationAccount.FirstOrDefaultAsync(a => a.AccountId == accountId && a.OrganizationId == organizationId) is not null)
            return await GetOrganizationByIdAsync(organizationId, cancellationToken);

        _context.OrganizationAccount.Add(new OrganizationAccount { AccountId = accountId, OrganizationId = organizationId });
        await _context.SaveChangesAsync(cancellationToken);

        var res = await _context.OrganizationAccount.FirstOrDefaultAsync(a => a.AccountId == accountId && a.OrganizationId == organizationId);
        if (res is null)
            return new ServiceResult<OrganizationDto>
            {
                IsSuccess = false,
                ErrorMessage = LogPhrases.ServiceResult.Error.NOT_FOUND
            };


        return await GetOrganizationByIdAsync(organizationId, cancellationToken);
    }

    private async Task<ServiceResult<OrganizationItem>> DoesOrganizationExist(Guid Id, CancellationToken cancellationToken)
    {
        var res = await _context.OrganizationItem.FirstOrDefaultAsync(o => o.Id == Id, cancellationToken);
        if (res is null)
            return new ServiceResult<OrganizationItem>
            {
                IsSuccess = false,
                ErrorMessage = LogPhrases.ServiceResult.Error.NOT_FOUND
            };

        return new ServiceResult<OrganizationItem>
        {
            IsSuccess = true,
            Data = res
        };
    }

    private async Task<ServiceResult<OrganizationItem>> DoesOrganizationExistEntity(OrganizationItem entity = null, Guid Id = default)
    {
        if (Id != default)
            return new ServiceResult<OrganizationItem>
            {
                IsSuccess = true,
                Data = await _context.OrganizationItem.FirstOrDefaultAsync(o => o.Id == Id)
            };

        if (entity.Id != default)
            return new ServiceResult<OrganizationItem>
            {
                IsSuccess = true,
                Data = await _context.OrganizationItem.FirstOrDefaultAsync(o => o.Id == entity.Id)
            };


        if (entity.Abbreviation != null)
            return new ServiceResult<OrganizationItem>
            {
                IsSuccess = true,
                Data = await _context.OrganizationItem.FirstOrDefaultAsync(o => o.Abbreviation == entity.Abbreviation)
            };


        return new ServiceResult<OrganizationItem>
        {
            IsSuccess = false,
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
            Name = item.Name!,
            OwnerId = item.Owner,
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
            Id = (Guid)dto.Id!,
            Name = dto.Name!,
            Owner = dto.OwnerId,
            Abbreviation = dto.Abbreviation,
            Description = dto.Description,
            CreateDate = dto.CreateDate,
            ModifyDate = dto.ModifyDate
        };
    }
}
