using FluentValidation;
using GAC.WMS.Core.Request;

namespace GAC.WMS.Application.Validators
{
    public class ProductRequestValidator : AbstractValidator<ProductRequest>
    {
        public ProductRequestValidator()
        {
            RuleFor(c => c.Code)
                .NotEmpty().WithMessage("Code is required")
                .MaximumLength(100).WithMessage("Code should not exceed 100 character length.");
            RuleFor(c => c.Title)
               .NotEmpty().WithMessage("Title is required")
               .MaximumLength(156).WithMessage("Title should not exceed 156 character length.");
            RuleFor(c => c.Description)
              .NotEmpty().WithMessage("Description is required")
              .MaximumLength(256).WithMessage("Description should not exceed 256 character length.");
            RuleFor(c => c.Dimensions)
                .NotNull().WithMessage("Dimensions should not be null")
                .SetValidator(new DimensionsRequestValidators());
        }
    }
}
