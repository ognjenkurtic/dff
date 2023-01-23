using System.Net;
using dffbackend.BusinessLogic.FactoringCompanies.Commands;
using dffbackend.BusinessLogic.FactoringCompanies.DTOs;
using dffbackend.Filters;
using Microsoft.AspNetCore.Mvc;

namespace dffbackend.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiKey]
public class FactoringCompanyController : BaseController
{
    private readonly ILogger<FactoringCompanyController> _logger;

    public FactoringCompanyController(ILogger<FactoringCompanyController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Creates factoring company
    /// </summary>
    /// <param name="body">CreateFactoringCompanyDto containing company details</param>
    /// <response code="200">Ok</response>
    /// <response code="200">OkObject</response>
    /// <response code="400">Invalid input</response>
    /// <response code="401">User not authorized</response>
    [HttpPost]
    [Route("create")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> CreateFactoringCompany([FromBody] CreateFactoringCompanyDto body)
    {
        var result = await Mediator.Send(new CreateFactoringCompanyCommand
        {
            Name = body.Name,
            Email = body.Email,
            // TODO: Do we add api key this way?
            ApiKey = body.ApiKey
        });

        return Ok(result);
    }

    /// <summary>
    /// Update factoring company
    /// </summary>
    /// <param name="body">UpdateFactoringCompanyDto containing company details to update</param>
    /// <response code="200">Ok</response>
    /// <response code="200">OkObject</response>
    /// <response code="400">Invalid input</response>
    /// <response code="401">User not authorized</response>
    [HttpPost]
    [Route("update")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> UpdateFactoringCompany([FromBody] UpdateFactoringCompanyDto body)
    {
        var result = await Mediator.Send(new UpdateFactoringCompanyCommand
        {
            Id = body.Id,
            Name = body.Name,
            Email = body.Email,
            // TODO: Do we update api key this way?
            ApiKey = body.ApiKey
        });

        return Ok(result);
    }

    /// <summary>
    /// Delete factoring company
    /// </summary>
    /// <param name="companyId">Id of the company to delete</param>
    /// <response code="200">Ok</response>
    /// <response code="200">OkObject</response>
    /// <response code="400">Invalid input</response>
    /// <response code="401">User not authorized</response>
    [HttpDelete]
    [Route("{companyId}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> DeleteFactoringCompany([FromRoute] Guid companyId)
    {
        var result = await Mediator.Send(new DeleteFactoringCompanyCommand
        {
            Id = companyId
        });

        return Ok(result);
    }
}