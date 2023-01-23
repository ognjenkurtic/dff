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

    public async Task<CheckDuplicatesResponseDto> CheckSignatureSetForDuplicates(SignatureSetDto signatureSetDto)
    {
        var mappedSignatureSet = _mapper.Map<Signature>(signatureSetDto);
        var signaturesDict = new Dictionary<string, string>()
        {
            { nameof(mappedSignatureSet.Signature1), mappedSignatureSet.Signature1 },
            { nameof(mappedSignatureSet.Signature2), mappedSignatureSet.Signature2 },
            { nameof(mappedSignatureSet.Signature3), mappedSignatureSet.Signature3 },
            { nameof(mappedSignatureSet.Signature4), mappedSignatureSet.Signature4 }
        };

        var result = new CheckDuplicatesResponseDto();

        foreach (var signature in signaturesDict)
        {
            var entryWithDuplicateSignature = await _dbContext.Signatures
                .Include(s => s.FactoringCompany)
                .FirstOrDefaultAsync(s => 
                    s.GetType().GetProperty(signature.Key).GetValue(s, null).ToString() == signature.Value.Trim());

            if (entryWithDuplicateSignature is not null)
            {
                var duplicateSignatureValue = entryWithDuplicateSignature
                    .GetType()
                    .GetProperty(signature.Key)
                    .GetValue(entryWithDuplicateSignature, null)
                    .ToString();

                _logger.LogWarning($"Pronađen duplikat potpisa: {duplicateSignatureValue}!");

                result.SignatureDuplicateResponses.Add(new SignatureDuplicateResponseDto()
                {
                    DuplicateSignature = duplicateSignatureValue,
                    FactoringCompanyName = entryWithDuplicateSignature.FactoringCompany.Name,
                    Email = entryWithDuplicateSignature.FactoringCompany.Email
                });
            }
        }

        result.HasDuplicates = result.SignatureDuplicateResponses.Any();

        return result;
    }
}