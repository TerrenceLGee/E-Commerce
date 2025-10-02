using FluentValidation;
using ECommerce.Shared.Dtos.Categories.Request;

namespace ECommerce.Application.Validators.CategoryValidators;

public class UpdateCategoryRequestValidator : AbstractValidator<UpdateCategoryRequest>
{
    public UpdateCategoryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50).WithMessage("Category name cannot be more than 50 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Category description cannot be more than 500 characters")
            .Must(desc => desc is null || desc.Length > 0).WithMessage("Description cannot be an empty string");
    }
}