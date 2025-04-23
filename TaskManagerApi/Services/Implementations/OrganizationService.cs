using System;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using TaskManagerApi.Data;
using TaskManagerApi.Enitities;
using TaskManagerApi.Models.OrganizationModel;
using TaskManagerApi.Services.Interfaces;
using static TaskManagerApi.Models.Constants;

namespace TaskManagerApi.Services.Implementations;

public class OrganizationService(TaskManagerAPIDbContext context) : IOrganizationService
{
    public async Task<OrganizationDto> Create(ClaimsPrincipal user, OrganizationDto newOgranization)
    {
        if (await DoesOrganizationExistEntity(Id: newOgranization.Id.ToString()) is not null)
            return null;

        newOgranization.Id = Guid.NewGuid();
        newOgranization.Owner = Guid.Parse(user.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID).Value);

        context.OrganizationItem.Add(ConvertToEntity(newOgranization));
        await context.SaveChangesAsync();

        context.OrganizationAccount.Add(new OrganizationAccount { 
            AccountId = newOgranization.Owner,
            OrganizationId = context.OrganizationItem.First(o => o.Id == newOgranization.Id).Id});

        await context.SaveChangesAsync();

        var addedNewOrganization = await context.OrganizationItem.FirstOrDefaultAsync(o => o.Id == newOgranization.Id);

        if (addedNewOrganization is not null)
            return ConvertToDto(addedNewOrganization);

        return null;
    }

    public async Task<OrganizationDto> Delete(OrganizationDto organizationToDelete)
    {
        var toDelete = await DoesOrganizationExistEntity(Id: organizationToDelete.Id.ToString());
        
        if (organizationToDelete.Id == null && organizationToDelete.Name == null && organizationToDelete.Abbreviation == null)
            return null;

        context.OrganizationItem.Remove(toDelete);
        await context.SaveChangesAsync();

        return ConvertToDto(toDelete);                                                                                                                                                                                                                                                                                                                                                                                                                                          
    }

    public async Task<OrganizationDto> Edit(ClaimsPrincipal user, OrganizationDto organizationToEdit)
    {
        var initialOrganization = await DoesOrganizationExistEntity(Id: organizationToEdit.Id.ToString());
        var accountId = Guid.Parse(user.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID).Value);

        if ((organizationToEdit.Id == null 
                && organizationToEdit.Name == null 
                && organizationToEdit.Abbreviation == null) 
            || initialOrganization.Id == null
            || accountId != initialOrganization?.Owner)
            return null;

        initialOrganization.Name = organizationToEdit.Name;
        initialOrganization.Owner = organizationToEdit.Owner;
        initialOrganization.Abbreviation = organizationToEdit.Abbreviation;
        initialOrganization.Description = organizationToEdit.Description;
        initialOrganization.ModifyDate = DateTime.UtcNow;

        context.OrganizationItem.Update(initialOrganization);
        await context.SaveChangesAsync();

        return ConvertToDto(await DoesOrganizationExistEntity(Id: organizationToEdit.Id.ToString()));
    }

    private async Task<OrganizationItem> DoesOrganizationExistEntity(OrganizationItem entity = null, string Id = null)
    {
        if (Id != null)
            return await context.OrganizationItem.FirstOrDefaultAsync(o => o.Id == Guid.Parse(Id));
        
        if (entity.Id != null)
            return await context.OrganizationItem.FirstOrDefaultAsync(o => o.Id == entity.Id);


        if (entity.Abbreviation != null)
            return await context.OrganizationItem.FirstOrDefaultAsync(o => o.Abbreviation == entity.Abbreviation);

            
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
        if (dto.Id == null && dto.Name == null && dto.Abbreviation == null)
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
