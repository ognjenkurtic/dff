using dffbackend.DTOs;
using dffbackend.Models;
using Microsoft.EntityFrameworkCore;

namespace dffbackend.BusinessLogic.Signatures.Agents;

public class SignaturesAgent : ISignaturesAgent
{
    private readonly ILogger<SignaturesAgent> _logger;
    private readonly DffContext _dbContext;

    public SignaturesAgent(ILogger<SignaturesAgent> logger, DffContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<SignatureSetResponseDto> CheckSignatureSetForDuplicates(SignatureSetDto signatureSet)
    {
        var result = new SignatureSetResponseDto();

        var s1 = await _dbContext.Signatures.Include(s => s.FactoringCompany).FirstOrDefaultAsync(s => s.Signature1.Trim() == signatureSet.Signature1);
        if (s1 is not null)
        {
            result.SignatureResponses.Add(new SignatureResponseDto()
            {
                DuplicateSignature = s1.Signature1,
                FactoringCompanyName = s1.FactoringCompany.Name,
                Email = s1.FactoringCompany.Email
            });
        }
        var s2 = await _dbContext.Signatures.Include(s => s.FactoringCompany).FirstOrDefaultAsync(s => s.Signature2.Trim() == signatureSet.Signature2);
        if (s2 is not null)
        {
            result.SignatureResponses.Add(new SignatureResponseDto()
            {
                DuplicateSignature = s2.Signature2,
                FactoringCompanyName = s2.FactoringCompany.Name,
                Email = s2.FactoringCompany.Email
            });
        }
        var s3 = await _dbContext.Signatures.Include(s => s.FactoringCompany).FirstOrDefaultAsync(s => s.Signature3.Trim() == signatureSet.Signature3);
        if (s3 is not null)
        {
            result.SignatureResponses.Add(new SignatureResponseDto()
            {
                DuplicateSignature = s3.Signature3,
                FactoringCompanyName = s3.FactoringCompany.Name,
                Email = s3.FactoringCompany.Email
            });
        }
        var s4 = await _dbContext.Signatures.Include(s => s.FactoringCompany).FirstOrDefaultAsync(s => s.Signature4.Trim() == signatureSet.Signature4);
        if (s4 is not null)
        {
            result.SignatureResponses.Add(new SignatureResponseDto()
            {
                DuplicateSignature = s4.Signature4,
                FactoringCompanyName = s4.FactoringCompany.Name,
                Email = s4.FactoringCompany.Email
            });
        }

        if (result.SignatureResponses.Any())
        {
            result.SignatureSetHasDuplicates = true;
        }
        else
        {
            result.SignatureSetHasDuplicates = false;
        }

        return result;
    }
}