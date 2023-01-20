using dffbackend.DTOs;

namespace dffbackend.BusinessLogic.Signatures.Agents;

public interface ISignaturesAgent
{
    Task<SignatureSetResponseDto> CheckSignatureSetForDuplicates (SignatureSetDto signatureSet);
}