using FluentValidation;
using ECommerce.Shared.Dtos.Auth.Request;

namespace ECommerce.Application.Validators.AuthValidators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email address is required")
            .EmailAddress().WithMessage("Please enter a valid email address");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("A password is required for login")
            .MinimumLength(8).WithMessage("Password must be at minimum 8 characters long");
    }
}