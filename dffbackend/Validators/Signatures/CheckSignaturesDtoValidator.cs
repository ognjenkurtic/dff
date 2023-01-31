using dffbackend.BusinessLogic.Signatures.DTOs;
using FluentValidation;

namespace dffbackend.Validators.Signatures;

public class CheckSignaturesDtoValidator : AbstractValidator<CheckSignaturesDto>
{
    public CheckSignaturesDtoValidator()
    {
        RuleFor(x => x.SignaturesSets)
            .NotNull()
            .WithMessage("Bar jedan potpis mora biti poslat");
        
        RuleFor(x => x.SignaturesSets)
            .NotEmpty()
            .WithMessage("Bar jedan potpis mora biti poslat");

        RuleForEach(x => x.SignaturesSets).ChildRules(signatureSet =>
            {
                signatureSet.RuleFor(s => s).Must(s => !string.IsNullOrEmpty(s.Signature5) ||
                    (!string.IsNullOrEmpty(s.Signature1) &&
                        !string.IsNullOrEmpty(s.Signature2) && 
                            !string.IsNullOrEmpty(s.Signature3) && 
                                !string.IsNullOrEmpty(s.Signature4)))
                .WithMessage("Morate proslediti ili prva četiri potpisa odjednom ili samo peti.");
            });
    }
}