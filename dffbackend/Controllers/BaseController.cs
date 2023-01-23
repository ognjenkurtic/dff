// using AutoMapper;
using System.Security.Claims;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace dffbackend.Controllers;

public class BaseController : ControllerBase
{
    
#pragma warning disable CA1051 // Do not declare visible instance fields
    protected IMediator mediator;
#pragma warning restore CA1051 // Do not declare visible instance fields

#pragma warning disable CA1051 // Do not declare visible instance fields
    protected IMapper mapper;
#pragma warning restore CA1051 // Do not declare visible instance fields

    protected IMediator Mediator => mediator ??= HttpContext.RequestServices.GetService<IMediator>();
    protected string CurrentFactoringCompanyId => User.FindFirst(ClaimTypes.Sid)?.Value;
    protected string CurrentFactoringCompanyName => User.FindFirst(ClaimTypes.Name)?.Value;
    protected string CurrentFactoringCompanyEmail => User.FindFirst(ClaimTypes.Email)?.Value;
}