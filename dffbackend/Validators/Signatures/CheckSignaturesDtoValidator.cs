using dffbackend.DTOs;
using FluentValidation;

namespace dffbackend.Validators.Signatures;

public class CheckSignaturesDtoValidator : AbstractValidator<CheckSignaturesDto>
{
    public CheckSignaturesDtoValidator()
    {
        // TODO: do we need to check if one of signatures 1, 2 or 3 exist, then all others from this group must exist?
        // or if at least signature 4 exists, then we can proceed (this is covered in the rule below)

        RuleForEach(x => x.SignaturesSets).ChildRules(signatureSet =>
            {
                signatureSet.RuleFor(s => s).Must(s => !string.IsNullOrEmpty(s.Signature4) ||
                    (!string.IsNullOrEmpty(s.Signature1) &&
                    !string.IsNullOrEmpty(s.Signature2) && !string.IsNullOrEmpty(s.Signature3)))
                .WithMessage("Morate proslediti ili prva tri potpisa odjednom ili četvrti.");
            });
    }
}