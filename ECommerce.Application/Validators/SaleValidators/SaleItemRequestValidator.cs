using FluentValidation;
using ECommerce.Shared.Dtos.Sales.Request;

namespace ECommerce.Application.Validators.SaleValidators;

public class SaleItemRequestValidator : AbstractValidator<SaleItemRequest>
{
    public SaleItemRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product id is required")
            .GreaterThan(0).WithMessage("Product is must be a non-negative integer greater than 0");

        RuleFor(x => x.Quantity)
            .NotEmpty().WithMessage("Item quantity is required")
            .InclusiveBetween(1, 100).WithMessage("Item quantity must be greater than 1 and less than 100");
    }
}