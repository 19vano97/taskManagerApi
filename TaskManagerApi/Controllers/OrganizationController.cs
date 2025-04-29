using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagerApi.Models.OrganizationModel;
using TaskManagerApi.Services.Interfaces;

namespace TaskManagerApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/organization")]
    [ApiController]
    public class OrganizationController(IOrganizationService organizationService) : ControllerBase
    {
        [HttpPost("create")]
        public async Task<ActionResult<OrganizationDto>> CreateOrganizationAsync(OrganizationDto newOrganization)
        {
            var newOrgToAdd = await organizationService.CreateAsync(User, newOrganization);

            if (newOrgToAdd.Id == null)
                return BadRequest();

            return Ok(newOrgToAdd);
        }

        [HttpPost("edit")]
        public async Task<ActionResult<OrganizationDto>> EditOrganizationAsync(OrganizationDto editOrganization)
        {
            var editOrgToAdd = organizationService.EditAsync(User, editOrganization);

            if (editOrgToAdd.Id == null)
                return BadRequest();

            return Ok(editOrgToAdd);
        }
    }
}
