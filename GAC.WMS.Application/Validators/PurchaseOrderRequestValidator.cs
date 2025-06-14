using FluentValidation;
using GAC.WMS.Core.Request;

namespace GAC.WMS.Application.Validators
{
    public class PurchaseOrderRequestValidator : AbstractValidator<PurchaseOrderRequest>
    {
        public PurchaseOrderRequestValidator()
        {
            RuleFor(c => c.ProcessingDate)
                .NotNull().WithMessage("ProcessingDate is required");
            RuleFor(c => c)
               .Must(x => x.CustomerId.HasValue || x.Customer != null)
               .NotNull().WithMessage("Customer is required");
            RuleFor(c => c.Products)
              .NotEmpty().WithMessage("Products is required");
            RuleForEach(x => x.Products)
            .SetValidator(new OrderItemDtoValidator());
        }
    }
}
