using FluentValidation;
using GAC.WMS.Core.Request;

namespace GAC.WMS.Application.Validators
{
    public class SalesOrderRequestValidator : AbstractValidator<SalesOrderRequest>
    {
        public SalesOrderRequestValidator()
        {
            RuleFor(c => c.ProcessingDate)
                .NotNull().WithMessage("ProcessingDate is required");
            RuleFor(c => c)
                        .Must(x => x.CustomerId>0 || x.Customer != null)
                        .NotNull().WithMessage("Customer is required");
            RuleFor(c => c.Products)
              .NotEmpty().WithMessage("Products is required");
            RuleForEach(x => x.Products)
            .SetValidator(new OrderItemDtoValidator());
            RuleFor(c => c.ShipmentAddress)
               .NotNull().WithMessage("Customer address should not be null")
                .SetValidator(new AddressRequestValidators());
        }
    }
}
