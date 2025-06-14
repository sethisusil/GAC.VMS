using FluentValidation;
using GAC.WMS.Core.Request;

namespace GAC.WMS.Application.Validators
{
    public class DimensionsRequestValidators : AbstractValidator<DimensionsRequest>
    {
        public DimensionsRequestValidators()
        {
            RuleFor(d => d.Height)
                .GreaterThan(0).WithMessage("Height is required and must be greater than 0.");
            RuleFor(d => d.Width)
                .GreaterThan(0).WithMessage("Width is required and must be greater than 0.");
            RuleFor(d => d.Length)
                .GreaterThan(0).WithMessage("Length is required and must be greater than 0.");
            RuleFor(d => d.Weight)
                .GreaterThan(0).WithMessage("Weight is required and must be greater than 0.");
        }
    }
}
