using FluentValidation;
using ECommerce.Shared.Dtos.Products.Request;

namespace ECommerce.Application.Validators.ProductValidators;

public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Product name cannot be greater than 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Product description cannot be greater than 1000 characters")
            .Must(desc => desc is null || desc.Length > 0).WithMessage("Description cannot be an empty string");

        RuleFor(x => x.StockKeepingUnit)
            .MaximumLength(20)
            .WithMessage("Stock keeping unit cannot be greater than 20 characters")
            .Must(sku => sku is null || sku.Length > 0).WithMessage("Stock keeping unit cannot be an empty string");

        RuleFor(x => x.Price)
            .NotEmpty().WithMessage("Price is required")
            .InclusiveBetween(0.01m, 1000000m).WithMessage("Price must be greater than 0 and less than 1000000");

        RuleFor(x => x.Discount)
            .IsInEnum().WithMessage("Invalid discount status");

        RuleFor(x => x.StockQuantity)
            .NotEmpty().WithMessage("Stock quantity is required")
            .InclusiveBetween(1, int.MaxValue).WithMessage("There must be at least 1 of this product in stock");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category id is required")
            .GreaterThan(0).WithMessage("Category id must a non-negative integer greater than 0");
    }
}