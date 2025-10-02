using FluentValidation;
using ECommerce.Shared.Dtos.Addresses.Request;

namespace ECommerce.Application.Validators.AddressValidators;

public class CreateAddressRequestValidator : AbstractValidator<CreateAddressRequest>
{
    public CreateAddressRequestValidator()
    {
        RuleFor(x => x.StreetNumber)
            .NotEmpty().WithMessage("Street number is required")
            .MaximumLength(15).WithMessage("Street number cannot be longer than 15 characters");

        RuleFor(x => x.StreetName)
            .NotEmpty().WithMessage("Street name is required")
            .MaximumLength(30).WithMessage("Street name cannot be longer than 30 characters");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required")
            .MaximumLength(20).WithMessage("City name cannot be longer than 20 characters");

        RuleFor(x => x.State)
            .NotEmpty().WithMessage("State is required")
            .MaximumLength(20).WithMessage("State name cannot be longer than 20 characters");

        RuleFor(x => x.ZipCode)
            .NotEmpty().WithMessage("Zip code is required")
            .Matches("^[0-9]{5}(?:-[0-9]{4})?$").WithMessage("Please enter a valid U.S. zip code format");

        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Country is required")
            .MaximumLength(25).WithMessage("Country name cannot be longer than 25 characters");

        RuleFor(x => x.AddressType)
            .IsInEnum().WithMessage("Invalid address type");
    }
}