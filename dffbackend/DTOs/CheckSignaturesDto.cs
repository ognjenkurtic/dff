namespace dffbackend.DTOs;

public class CheckSignaturesDto
{
    public List<SignatureSetDto> SignaturesSets { get; set; } = new List<SignatureSetDto>();
}

public class SignatureSetDto
{
    public string Signature1 { get; set; }

    public string Signature2 { get; set; }

    public string Signature3 { get; set; }

    public string Signature4 { get; set; }
}