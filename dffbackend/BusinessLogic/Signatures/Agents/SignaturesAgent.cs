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
                    (!string.IsNullOrEmpty(s.Signature1) && s.Signature1 == submittedSignatures.Signature1) ||
                        (!string.IsNullOrEmpty(s.Signature2) && s.Signature2 == submittedSignatures.Signature2) ||
                            (!string.IsNullOrEmpty(s.Signature3) && s.Signature3 == submittedSignatures.Signature3) ||
                                (!string.IsNullOrEmpty(s.Signature4) && s.Signature4 == submittedSignatures.Signature4) ||
                                    (!string.IsNullOrEmpty(s.Signature5) && s.Signature5 == submittedSignatures.Signature5));
        
        results.OwnDuplicate = entryWithDuplicateSignature is not null ? entryWithDuplicateSignature.FactoringCompanyId.ToString() == requesterId : false;
        
        if (entryWithDuplicateSignature is not null && !results.OwnDuplicate)
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

    public async Task<List<CheckDuplicatesResponseDto>> CheckSignatureSetsForDuplicates(string requesterId, List<SignatureSetDto> signatureSets)
    {
        var sig1Values = signatureSets.Where(s => !string.IsNullOrEmpty(s.Signature1)).Select(s => s.Signature1).Distinct().ToList();
        var sig2Values = signatureSets.Where(s => !string.IsNullOrEmpty(s.Signature2)).Select(s => s.Signature2).Distinct().ToList();
        var sig3Values = signatureSets.Where(s => !string.IsNullOrEmpty(s.Signature3)).Select(s => s.Signature3).Distinct().ToList();
        var sig4Values = signatureSets.Where(s => !string.IsNullOrEmpty(s.Signature4)).Select(s => s.Signature4).Distinct().ToList();
        var sig5Values = signatureSets.Where(s => !string.IsNullOrEmpty(s.Signature5)).Select(s => s.Signature5).Distinct().ToList();

        var matchingSignatures = await _dbContext.Signatures
            .Include(s => s.FactoringCompany)
            .Where(s =>
                (sig1Values.Count > 0 && !string.IsNullOrEmpty(s.Signature1) && sig1Values.Contains(s.Signature1)) ||
                (sig2Values.Count > 0 && !string.IsNullOrEmpty(s.Signature2) && sig2Values.Contains(s.Signature2)) ||
                (sig3Values.Count > 0 && !string.IsNullOrEmpty(s.Signature3) && sig3Values.Contains(s.Signature3)) ||
                (sig4Values.Count > 0 && !string.IsNullOrEmpty(s.Signature4) && sig4Values.Contains(s.Signature4)) ||
                (sig5Values.Count > 0 && !string.IsNullOrEmpty(s.Signature5) && sig5Values.Contains(s.Signature5)))
            .ToListAsync();

        var results = new List<CheckDuplicatesResponseDto>();

        foreach (var submitted in signatureSets)
        {
            var result = new CheckDuplicatesResponseDto();

            var match = matchingSignatures.FirstOrDefault(s =>
                (!string.IsNullOrEmpty(s.Signature1) && s.Signature1 == submitted.Signature1) ||
                (!string.IsNullOrEmpty(s.Signature2) && s.Signature2 == submitted.Signature2) ||
                (!string.IsNullOrEmpty(s.Signature3) && s.Signature3 == submitted.Signature3) ||
                (!string.IsNullOrEmpty(s.Signature4) && s.Signature4 == submitted.Signature4) ||
                (!string.IsNullOrEmpty(s.Signature5) && s.Signature5 == submitted.Signature5));

            result.OwnDuplicate = match is not null && match.FactoringCompanyId.ToString() == requesterId;

            if (match is not null && !result.OwnDuplicate)
            {
                CheckExactSignatureDuplicationAndCreateResponse(SignatureType.Signature1, match.Signature1, match.FactoringCompany, submitted.Signature1, result.SignatureDuplicateResponses);
                CheckExactSignatureDuplicationAndCreateResponse(SignatureType.Signature2, match.Signature2, match.FactoringCompany, submitted.Signature2, result.SignatureDuplicateResponses);
                CheckExactSignatureDuplicationAndCreateResponse(SignatureType.Signature3, match.Signature3, match.FactoringCompany, submitted.Signature3, result.SignatureDuplicateResponses);
                CheckExactSignatureDuplicationAndCreateResponse(SignatureType.Signature4, match.Signature4, match.FactoringCompany, submitted.Signature4, result.SignatureDuplicateResponses);
                CheckExactSignatureDuplicationAndCreateResponse(SignatureType.Signature5, match.Signature5, match.FactoringCompany, submitted.Signature5, result.SignatureDuplicateResponses);
                result.HasDuplicates = true;
            }

            results.Add(result);
        }

        return results;
    }

    public async Task StoreSignatureSet(string requesterId, Signature signatureSet)
    {
        signatureSet.FactoringCompanyId = new Guid(requesterId);
        signatureSet.CreationDate = DateTime.Now;
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