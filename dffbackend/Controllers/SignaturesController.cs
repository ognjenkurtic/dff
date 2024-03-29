using System.Net;
using dffbackend.BusinessLogic.Signatures.Commands;
using dffbackend.BusinessLogic.Signatures.DTOs;
using dffbackend.Filters;
using Microsoft.AspNetCore.Mvc;

namespace dffbackend.Controllers;

[ApiController]
[Route("api/[controller]")]
[FactoringCompanyApiKey]
public class SignaturesController : BaseController
{
    private readonly ILogger<SignaturesController> _logger;

    public SignaturesController(ILogger<SignaturesController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Checks signature sets for duplicates
    /// </summary>
    /// <param name="body">CheckSignaturesDto containing a list of signature sets</param>
    /// <response code="200">Ok</response>
    /// <response code="200">OkObject</response>
    /// <response code="400">Invalid input</response>
    /// <response code="401">User not authorized</response>
    [HttpPost]
    [Route("check")]
    [ProducesResponseType(typeof(List<CheckDuplicatesResponseDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> CheckSignatures([FromBody] CheckSignaturesDto body)
    {
        var result = await Mediator.Send(new CheckSignaturesCommand
        {
            RequesterId = CurrentFactoringCompanyId,
            SignaturesSets = body.SignaturesSets
        });

        return Ok(result);
    }

    /// <summary>
    /// Checks signature sets for duplicates and stores them in db
    /// </summary>
    /// <param name="body">CheckSignaturesDto containing a list of signature sets</param>
    /// <response code="200">Ok</response>
    /// <response code="200">OkObject</response>
    /// <response code="400">Invalid input</response>
    /// <response code="401">User not authorized</response>
    [HttpPost]
    [Route("checkandstore")]
    [ProducesResponseType(typeof(List<CheckDuplicatesResponseDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> CheckSignaturesAndStore([FromBody] CheckSignaturesDto body)
    {
        var result = await Mediator.Send(new CheckSignaturesAndStoreCommand
        {
            RequesterId = CurrentFactoringCompanyId,
            SignaturesSets = body.SignaturesSets
        });

        return Ok(result);
    }
}
