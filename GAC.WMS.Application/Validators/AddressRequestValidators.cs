using FluentValidation;
using GAC.WMS.Core.Request;

namespace GAC.WMS.Application.Validators
{
    public class AddressRequestValidators : AbstractValidator<AddressRequest>
    {
        public AddressRequestValidators()
        {
            RuleFor(a => a.Street)
                .NotEmpty().WithMessage("Street should not be null")
                .MaximumLength(256).WithMessage("Street should not exceed 256 character length.");
            RuleFor(a => a.State)
                .NotEmpty().WithMessage("State should not be null")
                .MaximumLength(128).WithMessage("State should not exceed 128 character length.");
            RuleFor(a => a.City)
                .NotEmpty().WithMessage("City should not be null")
                .MaximumLength(256).WithMessage("City should not exceed 256 character length.");
            RuleFor(a => a.Country)
                .NotEmpty().WithMessage("Country should not be null")
                .MaximumLength(128).WithMessage("Country should not exceed 128 character length.");
            RuleFor(a => a.ZipCode)
                .NotEmpty().WithMessage("ZipCode should not be null")
                .MaximumLength(10).WithMessage("ZipCode should not exceed 10 character length.");

        }
    }
}
