namespace dffbackend.DTOs;

public class CheckDuplicatesResponseDto
{
    public bool HasDuplicates { get; set; }
    public List<SignatureDuplicateResponseDto> SignatureDuplicateResponses { get; set; }
}

public class SignatureDuplicateResponseDto
{
    public string DuplicateSignature { get; set; }

    public string FactoringCompanyName { get; set; }

    public string Email { get; set; }
}