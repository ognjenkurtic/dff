using System.Net;
using AutoMapper;
using dffbackend.BusinessLogic.Signatures.Commands;
using dffbackend.DTOs;
using dffbackend.Filters;
using dffbackend.Models;
using Microsoft.AspNetCore.Mvc;

namespace dffbackend.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiKey]
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
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> CheckSignatures([FromBody] CheckSignaturesDto body)
    {
        var result = await Mediator.Send(new CheckSignaturesCommand
        {
            SignaturesSets = body.SignaturesSets
        });

        return Ok(result);
    }  
}

// 2 endpoint - proverava duplikate i storuje taj entry (checkandstore)
// 3 endpoint - CRUD nad faktorima (flag obrisan) (ovo ide u FactoringCompanyController)