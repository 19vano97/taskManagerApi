using System;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TaskManagerApi.Data;
using TaskManagerApi.Enitities.Organization;
using TaskManagerApi.Models.OrganizationModel;
using TaskManagerApi.Models.Project;
using TaskManagerApi.Services.Interfaces;
using static TaskManagerApi.Models.Constants;

namespace TaskManagerApi.Services.Implementations;

public class OrganizationService : IOrganizationService
{
    private readonly TaskManagerAPIDbContext _context;
    private readonly ILogger<OrganizationService> _logger;
    
    public OrganizationService(TaskManagerAPIDbContext context, ILogger<OrganizationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<OrganizationDto> CreateAsync(ClaimsPrincipal user, OrganizationDto newOgranization)
    {
        if (await DoesOrganizationExistEntity(Id: newOgranization.Id.ToString()) is not null)
            return null;

        newOgranization.Id = Guid.NewGuid();
        newOgranization.Owner = Guid.Parse(user.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID).Value);

        _context.OrganizationItem.Add(ConvertToEntity(newOgranization));
        await _context.SaveChangesAsync();

        _context.OrganizationAccount.Add(new OrganizationAccount { 
            AccountId = newOgranization.Owner,
            OrganizationId = _context.OrganizationItem.First(o => o.Id == newOgranization.Id).Id});

        await _context.SaveChangesAsync();

        var addedNewOrganization = await _context.OrganizationItem.FirstOrDefaultAsync(o => o.Id == newOgranization.Id);

        if (addedNewOrganization is not null)
            return ConvertToDto(addedNewOrganization);

        return null;
    }

    public async Task<OrganizationDto> DeleteAsync(OrganizationDto organizationToDelete)
    {
        var toDelete = await DoesOrganizationExistEntity(Id: organizationToDelete.Id.ToString());
        
        if (organizationToDelete.Id == Guid.Empty && organizationToDelete.Name == null && organizationToDelete.Abbreviation == null)
            return null!;

        try
        {
            _context.OrganizationItem.Remove(toDelete);
            await _context.SaveChangesAsync();
        }
        catch (System.Exception ex)
        {
            Log.Error(ex.ToString());
            _logger.LogError(ex.ToString());
        }

        return ConvertToDto(toDelete);                                                                                                                                                                                                                                                                                                                                                                                                                                          
    }

    public async Task<OrganizationDto> EditAsync(ClaimsPrincipal user, OrganizationDto organizationToEdit)
    {
        var initialOrganization = await DoesOrganizationExistEntity(Id: organizationToEdit.Id.ToString());
        var accountId = Guid.Parse(user.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID).Value);

        if ((organizationToEdit.Id == Guid.Empty 
                && organizationToEdit.Name == null 
                && organizationToEdit.Abbreviation == null) 
            || initialOrganization.Id == Guid.Empty
            || accountId != initialOrganization?.Owner)
            return null;

        initialOrganization.Name = organizationToEdit.Name;
        initialOrganization.Owner = organizationToEdit.Owner;
        initialOrganization.Abbreviation = organizationToEdit.Abbreviation;
        initialOrganization.Description = organizationToEdit.Description;
        initialOrganization.ModifyDate = DateTime.UtcNow;

        _context.OrganizationItem.Update(initialOrganization);
        await _context.SaveChangesAsync();

        return ConvertToDto(await DoesOrganizationExistEntity(Id: organizationToEdit.Id.ToString()));
    }

    public async Task<OrganizationProjectDto> GetOrganizationProjectsAsync(Guid organizationId)
    {
        var organization = await DoesOrganizationExist(organizationId);
        if (organization == null)
            return null;

        var projects = await _context.ProjectItems.
            Where(p => p.OrganizationId == organizationId)
            .ToListAsync();

        return new OrganizationProjectDto {
            Id = organization.Id,
            Name = organization.Name,
            Abbreviation = organization.Abbreviation,
            Owner = organization.Owner,
            Description = organization.Description,
            CreateDate = organization.CreateDate,
            ModifyDate = organization.ModifyDate,
            Projects = projects.Select(p => GeneralService.ConvertProjectToOutput(p)).ToList()
        };
    }
    
    public async Task<List<OrganizationProjectDto>> GetOrganizationsAsync(Guid accountId)
    {
        var organizations = await _context.OrganizationAccount.Where(o => o.AccountId == accountId)
            .Select(o => o.OrganizationId)
            .ToListAsync();

        var result = new List<OrganizationProjectDto>();
        foreach (var organizationId in organizations)
        {
            var organization = await GetOrganizationProjectsAsync(organizationId);
            if (organization != null)
            {
                result.Add(organization);
            }
        }

        return result;
    }

    public Task<OrganizationDto> GetOrganizationAsync(Guid organizationId)
    {
        throw new NotImplementedException();
    }

    private async Task<OrganizationItem> DoesOrganizationExist(Guid Id)
    {
        if (Id != null)
            return await _context.OrganizationItem.FirstOrDefaultAsync(o => o.Id == Id);

        return null;
    }

    private async Task<OrganizationItem> DoesOrganizationExistEntity(OrganizationItem entity = null, string Id = null)
    {
        if (Id != null)
            return await _context.OrganizationItem.FirstOrDefaultAsync(o => o.Id == Guid.Parse(Id));

        if (entity.Id != null)
            return await _context.OrganizationItem.FirstOrDefaultAsync(o => o.Id == entity.Id);


        if (entity.Abbreviation != null)
            return await _context.OrganizationItem.FirstOrDefaultAsync(o => o.Abbreviation == entity.Abbreviation);


        return null;
    }

    private OrganizationDto ConvertToDto(OrganizationItem item)
    {
        if (item.Id == null && item.Name == null && item.Abbreviation == null)
            return null;

        return new OrganizationDto {
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

        return new OrganizationItem {
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
