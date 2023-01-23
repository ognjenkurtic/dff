using dffbackend.Models;

namespace dffbackend.BusinessLogic.Signatures.DTOs;

public class CheckDuplicatesResponseDto
{
    public CheckDuplicatesResponseDto()
    {
        SignatureDuplicateResponses = new List<SignatureDuplicateResponseDto>();
    }

    public bool HasDuplicates { get; set; }

    public List<SignatureDuplicateResponseDto> SignatureDuplicateResponses { get; set; }
}

public class SignatureDuplicateResponseDto
{
    public SignatureType SignatureType { get; set; }

    public string DuplicateSignature { get; set; }

    public string FactoringCompanyName { get; set; }

    public string Email { get; set; }
}