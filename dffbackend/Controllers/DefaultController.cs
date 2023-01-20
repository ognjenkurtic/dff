using dffbackend.Filters;
using dffbackend.Models;
using Microsoft.AspNetCore.Mvc;

namespace dffbackend.Controllers;

[ApiController]
[Route("api/[controller]s")]
[ApiKey]
public class DefaultController : ControllerBase
{
    private readonly ILogger<DefaultController> _logger;

    public DefaultController(ILogger<DefaultController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetTest")]
    public string Get()
    {
        return "Works";
    }
}
