using AutoMapper;
using dffbackend.DTOs;
using dffbackend.Models;
using Microsoft.EntityFrameworkCore;

namespace dffbackend.BusinessLogic.Signatures.Agents;

public class SignaturesAgent : ISignaturesAgent
{
    private readonly ILogger<SignaturesAgent> _logger;
    private readonly DffContext _dbContext;
    private readonly IMapper _mapper;

    public SignaturesAgent(ILogger<SignaturesAgent> logger, DffContext dbContext, IMapper mapper)
    {
        _logger = logger;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<SignatureSetResponseDto> CheckSignatureSetForDuplicates(SignatureSetDto signatureSetDto)
    {
        var mappedSignatureSet = _mapper.Map<Signature>(signatureSetDto);
        var signaturesDict = new Dictionary<string, string>()
        {
            { nameof(mappedSignatureSet.Signature1), mappedSignatureSet.Signature1 },
            { nameof(mappedSignatureSet.Signature2), mappedSignatureSet.Signature2 },
            { nameof(mappedSignatureSet.Signature3), mappedSignatureSet.Signature3 },
            { nameof(mappedSignatureSet.Signature4), mappedSignatureSet.Signature4 }
        };

        var result = new SignatureSetResponseDto();

        foreach (var signature in signaturesDict)
        {
            var foundSignature = await _dbContext.Signatures.Include(s => s.FactoringCompany).FirstOrDefaultAsync(
                s => s.GetType().GetProperty(signature.Key).GetValue(s, null).ToString() == signature.Value.Trim());

            if (foundSignature is not null)
            {
                var duplicateSignature = foundSignature.GetType().GetProperty(signature.Key).GetValue(foundSignature, null).ToString();
                _logger.LogWarning($"Pronađen duplikat potpisa: {duplicateSignature}!");

                result.SignatureDuplicateResponses.Add(new SignatureDuplicateResponseDto()
                {
                    DuplicateSignature = duplicateSignature,
                    FactoringCompanyName = foundSignature.FactoringCompany.Name,
                    Email = foundSignature.FactoringCompany.Email
                });
            }
        }

        result.SignatureSetHasDuplicates = result.SignatureDuplicateResponses.Any();

        return result;
    }
}