using FluentValidation;
using GAC.WMS.Core.Dtos;

namespace GAC.WMS.Application.Validators
{
    public class OrderItemDtoValidator : AbstractValidator<OrderItemDto>
    {
        public OrderItemDtoValidator()
        {
            RuleFor(x => x.ProductCode)
                .NotEmpty().WithMessage("ProductCode is required");
            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than 0");
        }
    }
}
