using System.Net;
using dffbackend.BusinessLogic.FactoringCompanies.Commands;
using dffbackend.BusinessLogic.FactoringCompanies.DTOs;
using dffbackend.BusinessLogic.FactoringCompanies.Queries;
using dffbackend.Filters;
using Microsoft.AspNetCore.Mvc;

namespace dffbackend.Controllers;

[ApiController]
[Route("api/[controller]")]
[AdminApiKey]
public class FactoringCompanyController : BaseController
{
    private readonly ILogger<FactoringCompanyController> _logger;

    public FactoringCompanyController(ILogger<FactoringCompanyController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get all factoring companies
    /// </summary>
    /// <response code="200">OkObject</response>
    /// <response code="401">User not authorized</response>
    [HttpGet]
    [Route("")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> GetAllFactoringCompanies()
    {
        var result = await Mediator.Send(new GetAllFactoringCompaniesQuery());
        return Ok(result);
    }

    /// <summary>
    /// Creates factoring company
    /// </summary>
    /// <param name="body">CreateFactoringCompanyDto containing company details</param>
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
            RequestBody = body
        });

        return Ok(result);
    }

    /// <summary>
    /// Update factoring company
    /// </summary>
    /// <param name="body">UpdateFactoringCompanyDto containing company details to update</param>
    /// <response code="200">OkObject</response>
    /// <response code="400">Invalid input</response>
    /// <response code="401">User not authorized</response>
    [HttpPut]
    [Route("update")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> UpdateFactoringCompany([FromBody] UpdateFactoringCompanyDto body)
    {
        var result = await Mediator.Send(new UpdateFactoringCompanyCommand
        {
            RequestBody = body
        });

        return Ok(result);
    }

    /// <summary>
    /// Delete factoring company
    /// </summary>
    /// <param name="companyId">Id of the company to delete</param>
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