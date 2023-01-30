using AutoMapper;
using dffbackend.BusinessLogic.Signatures.DTOs;
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

    public async Task<CheckDuplicatesResponseDto> CheckSignatureSetForDuplicates(string requesterId, SignatureSetDto submittedSignatures)
    {
        var results = new CheckDuplicatesResponseDto();

        var entryWithDuplicateSignature = await _dbContext.Signatures
                .Include(s => s.FactoringCompany)
                .FirstOrDefaultAsync(s => 
                    (s.Signature1 != string.Empty && s.Signature1 == submittedSignatures.Signature1) ||
                        (s.Signature3 != string.Empty && s.Signature2 == submittedSignatures.Signature2) ||
                            (s.Signature3 != string.Empty && s.Signature3 == submittedSignatures.Signature3) ||
                                (s.Signature4 != string.Empty && s.Signature4 == submittedSignatures.Signature4) ||
                                    (s.Signature5 != string.Empty && s.Signature5 == submittedSignatures.Signature5));
        
        if (entryWithDuplicateSignature is not null)
        {
            CheckExactSignatureDuplicationAndCreateResponse(
                SignatureType.Signature1,
                entryWithDuplicateSignature.Signature1, 
                entryWithDuplicateSignature.FactoringCompany,
                submittedSignatures.Signature1,
                results.SignatureDuplicateResponses);
            
            CheckExactSignatureDuplicationAndCreateResponse(
                SignatureType.Signature2,
                entryWithDuplicateSignature.Signature2, 
                entryWithDuplicateSignature.FactoringCompany,
                submittedSignatures.Signature2,
                results.SignatureDuplicateResponses);
            
            CheckExactSignatureDuplicationAndCreateResponse(
                SignatureType.Signature3,
                entryWithDuplicateSignature.Signature3, 
                entryWithDuplicateSignature.FactoringCompany,
                submittedSignatures.Signature3,
                results.SignatureDuplicateResponses);
                
            CheckExactSignatureDuplicationAndCreateResponse(
                SignatureType.Signature4,
                entryWithDuplicateSignature.Signature4, 
                entryWithDuplicateSignature.FactoringCompany,
                submittedSignatures.Signature4,
                results.SignatureDuplicateResponses);

            CheckExactSignatureDuplicationAndCreateResponse(
                SignatureType.Signature5,
                entryWithDuplicateSignature.Signature5, 
                entryWithDuplicateSignature.FactoringCompany,
                submittedSignatures.Signature5,
                results.SignatureDuplicateResponses);    
            
            results.HasDuplicates = true;
        };

        return results;
    }

    public async Task StoreSignatureSet(string requesterId, Signature signatureSet)
    {
        signatureSet.FactoringCompanyId = new Guid(requesterId);
        await _dbContext.Signatures.AddAsync(signatureSet);
    }

    private void CheckExactSignatureDuplicationAndCreateResponse(
        SignatureType signatureType,
        string existingSignature, 
        FactoringCompany existingSignatureFactoringCompany, 
        string candidateSignature,
        List<SignatureDuplicateResponseDto> results)
    {
        if (string.IsNullOrEmpty(existingSignature))
        {
            return;
        }

        if (existingSignature != candidateSignature)
        {
            return;
        }

        _logger.LogWarning($"Pronađen duplikat potpisa tipa {signatureType}. Vrednost: {candidateSignature}!");
        
        results.Add(new SignatureDuplicateResponseDto()
        {
            SignatureType = signatureType,
            DuplicateSignature = candidateSignature,
            FactoringCompanyName = existingSignatureFactoringCompany.Name,
            Email = existingSignatureFactoringCompany.Email
        });
    }
}