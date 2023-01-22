using AutoMapper;
using dffbackend.BusinessLogic.Signatures.Agents;
using dffbackend.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace dffbackend.BusinessLogic.FactoringCompanies.Commands;

public class UpdateFactoringCompanyCommand : IRequest
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Email { get; set; }

    public string ApiKey { get; set; }
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
        var existingCompany = await _dbContext.FactoringCompanies.FirstOrDefaultAsync(c => c.Id == request.Id);

        if (existingCompany is null)
        {
            // TODO: Implement custom exceptions or agree on the way we return error results
            _logger.LogError($"Kompanija sa ID-em {request.Id} nije pronađena.");
            throw new Exception($"Kompanija sa ID-em {request.Id} nije pronađena.");
        }

        existingCompany.Name = !string.IsNullOrEmpty(request.Name) ? request.Name : existingCompany.Name;
        existingCompany.Email = !string.IsNullOrEmpty(request.Email) ? request.Email : existingCompany.Email;
        existingCompany.ApiKey = !string.IsNullOrEmpty(request.ApiKey) ? request.ApiKey : existingCompany.ApiKey;

        _dbContext.Entry(existingCompany).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();

        return Unit.Value;
    }
}