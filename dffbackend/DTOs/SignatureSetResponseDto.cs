namespace dffbackend.DTOs;

public class SignatureSetResponseDto
{
    public bool SignatureSetHasDuplicates { get; set; }
    public List<SignatureResponseDto> SignatureResponses { get; set; }
}

public class SignatureResponseDto
{
    public string DuplicateSignature { get; set; }

    public string FactoringCompanyName { get; set; }

    public string Email { get; set; }
}