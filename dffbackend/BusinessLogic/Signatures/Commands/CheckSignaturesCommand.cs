using AutoMapper;
using dffbackend.BusinessLogic.Signatures.Agents;
using dffbackend.DTOs;
using MediatR;

namespace dffbackend.BusinessLogic.Signatures.Commands;

public class CheckSignaturesCommand : IRequest<List<SignatureSetResponseDto>>
{
    public List<SignatureSetDto> SignaturesSets { get; set; }
}

public class CheckSignaturesCommandHandler : IRequestHandler<CheckSignaturesCommand, List<SignatureSetResponseDto>>
{
    private readonly ILogger _logger;
    private readonly ISignaturesAgent _signaturesAgent;

    public CheckSignaturesCommandHandler(ILogger<CheckSignaturesCommand> logger, ISignaturesAgent signaturesAgent)
    {
        _logger = logger;
        _signaturesAgent = signaturesAgent;
    }

    public async Task<List<SignatureSetResponseDto>> Handle(CheckSignaturesCommand request, CancellationToken cancellationToken)
    {
        var result = new List<SignatureSetResponseDto>();
        
        foreach (var signatureSet in request.SignaturesSets)
        {
            result.Add(await _signaturesAgent.CheckSignatureSetForDuplicates(signatureSet));   
        }

        return result;
    }
}