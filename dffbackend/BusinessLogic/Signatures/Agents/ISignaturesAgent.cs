using dffbackend.BusinessLogic.Signatures.DTOs;
using dffbackend.Models;

namespace dffbackend.BusinessLogic.Signatures.Agents;

public interface ISignaturesAgent
{
    Task<CheckDuplicatesResponseDto> CheckSignatureSetForDuplicates(string requesterId, SignatureSetDto signatureSet);
    Task<List<CheckDuplicatesResponseDto>> CheckSignatureSetsForDuplicates(string requesterId, List<SignatureSetDto> signatureSets);
    Task StoreSignatureSet(string requesterId, Signature signatureSet);
}