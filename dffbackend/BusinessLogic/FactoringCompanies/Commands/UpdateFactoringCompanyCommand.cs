using AutoMapper;
using dffbackend.BusinessLogic.FactoringCompanies.DTOs;
using dffbackend.BusinessLogic.Signatures.Agents;
using dffbackend.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace dffbackend.BusinessLogic.FactoringCompanies.Commands;

public class UpdateFactoringCompanyCommand : IRequest
{
    public UpdateFactoringCompanyDto RequestBody { get; set; }
}

public class UpdateFactoringCompanyCommandHandler : IRequestHandler<UpdateFactoringCompanyCommand>
{
    private readonly ILogger _logger;
    private readonly ISignaturesAgent _signaturesAgent;
    private readonly DffContext _dbContext;
    private readonly IMapper _mapper;

    public UpdateFactoringCompanyCommandHandler(ILogger<UpdateFactoringCompanyCommand> logger,
        ISignaturesAgent signaturesAgent,
        DffContext dbContext,
        IMapper mapper)
    {
        _logger = logger;
        _signaturesAgent = signaturesAgent;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(UpdateFactoringCompanyCommand request, CancellationToken cancellationToken)
    {
        var existingCompany = await _dbContext.FactoringCompanies.FirstOrDefaultAsync(c => c.Id == request.RequestBody.Id);

        if (existingCompany is null)
        {
            // TODO: Implement custom exceptions or agree on the way we return error results
            _logger.LogError($"Kompanija sa ID-em {request.RequestBody.Id} nije pronađena.");
            throw new Exception($"Kompanija sa ID-em {request.RequestBody.Id} nije pronađena.");
        }

        existingCompany.Name = !string.IsNullOrEmpty(request.RequestBody.Name) ? request.RequestBody.Name : existingCompany.Name;
        existingCompany.Email = !string.IsNullOrEmpty(request.RequestBody.Email) ? request.RequestBody.Email : existingCompany.Email;
        existingCompany.ApiKey = !string.IsNullOrEmpty(request.RequestBody.ApiKey) ? request.RequestBody.ApiKey : existingCompany.ApiKey;

        _dbContext.Entry(existingCompany).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();

        return Unit.Value;
    }
}