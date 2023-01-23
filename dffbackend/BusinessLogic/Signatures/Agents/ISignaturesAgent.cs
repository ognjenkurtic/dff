using dffbackend.BusinessLogic.Signatures.DTOs;
using dffbackend.Models;

namespace dffbackend.BusinessLogic.Signatures.Agents;

public interface ISignaturesAgent
{
    Task<CheckDuplicatesResponseDto> CheckSignatureSetForDuplicates(string requesterId, SignatureSetDto signatureSet);
    Task StoreSignatureSet(string requesterId, Signature signatureSet);
}