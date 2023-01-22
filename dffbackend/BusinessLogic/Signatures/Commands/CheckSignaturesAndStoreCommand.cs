using AutoMapper;
using dffbackend.BusinessLogic.Signatures.Agents;
using dffbackend.DTOs;
using dffbackend.Models;
using MediatR;

namespace dffbackend.BusinessLogic.Signatures.Commands;

public class CheckSignaturesAndStoreCommand : IRequest<List<SignatureSetResponseDto>>
{
    // TODO: After implementing issue #4 (fetching factoring company making the request), expand inputs with the factoring company object (or id)
    public List<SignatureSetDto> SignaturesSets { get; set; }
}

public class CheckSignaturesAndStoreCommandHandler : IRequestHandler<CheckSignaturesAndStoreCommand, List<SignatureSetResponseDto>>
{
    private readonly ILogger _logger;
    private readonly ISignaturesAgent _signaturesAgent;
    private readonly DffContext _dbContext;
    private readonly IMapper _mapper;

    public CheckSignaturesAndStoreCommandHandler(ILogger<CheckSignaturesAndStoreCommand> logger,
        ISignaturesAgent signaturesAgent,
        DffContext dbContext,
        IMapper mapper)
    {
        _logger = logger;
        _signaturesAgent = signaturesAgent;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<List<SignatureSetResponseDto>> Handle(CheckSignaturesAndStoreCommand request, CancellationToken cancellationToken)
    {
        var result = new List<SignatureSetResponseDto>();

        foreach (var signatureSet in request.SignaturesSets)
        {
            result.Add(await _signaturesAgent.CheckSignatureSetForDuplicates(signatureSet));

            var signatureToAdd = _mapper.Map<Signature>(signatureSet);
            // TODO: Should we store all signatures no matter if duplicates are found for some of them, or we store only non-duplicate ones?
            // TODO: Assign here the factoring company to the signature
            await _dbContext.Signatures.AddAsync(signatureToAdd);
        }

        await _dbContext.SaveChangesAsync();
        return result;
    }
}