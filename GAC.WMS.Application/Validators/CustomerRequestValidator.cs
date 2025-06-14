using FluentValidation;
using GAC.WMS.Core.Request;

namespace GAC.WMS.Application.Validators
{
    public class CustomerRequestValidator : AbstractValidator<CustomerRequest>
    {
        public CustomerRequestValidator()
        {
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Customer name is required")
                .MaximumLength(256).WithMessage("Customer name should not exceed 256 character length.");
            RuleFor(c => c.Email)
               .NotEmpty().WithMessage("Customer email is required")
               .MaximumLength(100).WithMessage("Customer email should not exceed 100 character length.")
               .EmailAddress().WithMessage("Invalid email address format.");
            RuleFor(c => c.Address)
                .NotNull().WithMessage("Customer address should not be null")
                .SetValidator(new AddressRequestValidators());
        }
    }
}
