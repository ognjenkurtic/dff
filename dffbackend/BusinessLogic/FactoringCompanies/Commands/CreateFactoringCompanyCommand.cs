using AutoMapper;
using dffbackend.BusinessLogic.FactoringCompanies.DTOs;
using dffbackend.BusinessLogic.Signatures.Agents;
using dffbackend.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace dffbackend.BusinessLogic.FactoringCompanies.Commands;

public class CreateFactoringCompanyCommand : IRequest<Guid>
{
    public CreateFactoringCompanyDto RequestBody { get; set; }
}

public class CreateFactoringCompanyCommandHandler : IRequestHandler<CreateFactoringCompanyCommand, Guid>
{
    private readonly ILogger _logger;
    private readonly ISignaturesAgent _signaturesAgent;
    private readonly DffContext _dbContext;
    private readonly IMapper _mapper;

    public CreateFactoringCompanyCommandHandler(ILogger<CreateFactoringCompanyCommand> logger,
        ISignaturesAgent signaturesAgent,
        DffContext dbContext,
        IMapper mapper)
    {
        _logger = logger;
        _signaturesAgent = signaturesAgent;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateFactoringCompanyCommand request, CancellationToken cancellationToken)
    {
        if (await _dbContext.FactoringCompanies.AnyAsync(c => c.Email == request.RequestBody.Email.Trim()))
        {
            // TODO: Implement custom exceptions or agree on the way we return error results
            _logger.LogError($"Kompanija {request.RequestBody.Name} već postoji.");
            throw new Exception($"Kompanija {request.RequestBody.Name} već postoji.");
        }

        var companyToAdd = _mapper.Map<FactoringCompany>(request.RequestBody);

        await _dbContext.FactoringCompanies.AddAsync(companyToAdd);
        await _dbContext.SaveChangesAsync();

        return companyToAdd.Id;
    }
}