namespace dffbackend.DTOs;

public class SignatureSetResponseDto
{
    public bool SignatureSetHasDuplicates { get; set; }
    public List<SignatureDuplicateResponseDto> SignatureDuplicateResponses { get; set; }
}

public class SignatureDuplicateResponseDto
{
    public string DuplicateSignature { get; set; }

    public string FactoringCompanyName { get; set; }

    public string Email { get; set; }
}