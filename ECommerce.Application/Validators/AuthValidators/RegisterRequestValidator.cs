using FluentValidation;
using ECommerce.Shared.Dtos.Auth.Request;

namespace ECommerce.Application.Validators.AuthValidators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(30).WithMessage("First name cannot be longer than 30 characters")
            .MinimumLength(2).WithMessage("First name must be longer than 2 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(30).WithMessage("Last name cannot be longer than 30 characters")
            .MinimumLength(2).WithMessage("Last name must be longer than 2 characters");

        RuleFor(x => x.BirthDate)
            .NotEmpty().WithMessage("Birth date is required")
            .Must(BeAValidBirthDate).WithMessage("'{PropertyValue}' is not a valid birth date, cannot be in the future");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email address is required")
            .EmailAddress().WithMessage("Please enter a valid email address");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Please confirm your password")
            .Equal(x => x.Password).WithMessage("Passwords do not match");

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

    private bool BeAValidBirthDate(DateOnly birthDate)
    {
        return birthDate <= DateOnly.FromDateTime(DateTime.Today);
    }
}