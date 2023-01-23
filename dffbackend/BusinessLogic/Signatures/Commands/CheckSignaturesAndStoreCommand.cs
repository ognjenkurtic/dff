using AutoMapper;
using dffbackend.BusinessLogic.Signatures.Agents;
using dffbackend.DTOs;
using dffbackend.Models;
using MediatR;

namespace dffbackend.BusinessLogic.Signatures.Commands;

public class CheckSignaturesAndStoreCommand : IRequest<List<CheckDuplicatesResponseDto>>
{
    // TODO: After implementing issue #4 (fetching factoring company making the request), expand inputs with the factoring company object (or id)
    public List<SignatureSetDto> SignaturesSets { get; set; }
}

public class CheckSignaturesAndStoreCommandHandler : IRequestHandler<CheckSignaturesAndStoreCommand, List<CheckDuplicatesResponseDto>>
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

    public async Task<List<CheckDuplicatesResponseDto>> Handle(CheckSignaturesAndStoreCommand request, CancellationToken cancellationToken)
    {
        var result = new List<CheckDuplicatesResponseDto>();

        foreach (var signatureSet in request.SignaturesSets)
        {
            var checkDuplicatesResponse = await _signaturesAgent.CheckSignatureSetForDuplicates(signatureSet);
            result.Add(checkDuplicatesResponse);

            if (checkDuplicatesResponse.HasDuplicates)
            {
                // We do not store duplicates in the db becuase they are not going to be financed
                // Db should contain only signatures of financed invoices
                continue;
            }

            // TODO: Assign here the factoring company to the signature
            await _dbContext.Signatures.AddAsync(_mapper.Map<Signature>(signatureSet));
        }

        await _dbContext.SaveChangesAsync();
        return result;
    }
}