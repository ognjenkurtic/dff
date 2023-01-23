using dffbackend.DTOs;

namespace dffbackend.BusinessLogic.Signatures.Agents;

public interface ISignaturesAgent
{
    Task<CheckDuplicatesResponseDto> CheckSignatureSetForDuplicates (SignatureSetDto signatureSet);
}