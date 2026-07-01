using BlinkDemoApi.Application.Auth.Dtos;
using FluentValidation;

namespace BlinkDemoApi.Application.Auth.Validators;

/// <summary>
/// Validates register payload.
/// FluentValidation keeps controllers clean and provides consistent error messages.
/// </summary>
public sealed class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Mobile).NotEmpty().Matches("^[0-9]{10}$").WithMessage("Mobile must be 10 digits.");
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
    }
}
