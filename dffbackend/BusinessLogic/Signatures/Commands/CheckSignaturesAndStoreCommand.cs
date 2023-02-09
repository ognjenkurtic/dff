using AutoMapper;
using dffbackend.BusinessLogic.Signatures.Agents;
using dffbackend.BusinessLogic.Signatures.DTOs;
using dffbackend.Models;
using MediatR;

namespace dffbackend.BusinessLogic.Signatures.Commands;

public class CheckSignaturesAndStoreCommand : IRequest<List<CheckDuplicatesResponseDto>>
{
    public string RequesterId { get; set; }
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
        var results = new List<CheckDuplicatesResponseDto>();

        foreach (var signatureSet in request.SignaturesSets)
        {
            var checkDuplicatesResponse = await _signaturesAgent.CheckSignatureSetForDuplicates(request.RequesterId, signatureSet);
            
            results.Add(checkDuplicatesResponse);

            // It is ok to store even if duplicates are found since the factor might be financing invoices that are different
            // just by invoice number for example. It is the responsibility of the factor to call the check duplicates endoint
            // prior to deciding if they want to call this one.

            await _signaturesAgent.StoreSignatureSet(request.RequesterId, _mapper.Map<Signature>(signatureSet));
        }

        await _dbContext.SaveChangesAsync();
        
        return results;
    }
}