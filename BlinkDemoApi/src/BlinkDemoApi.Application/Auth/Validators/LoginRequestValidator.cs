using BlinkDemoApi.Application.Auth.Dtos;
using FluentValidation;

namespace BlinkDemoApi.Application.Auth.Validators;

public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Mobile).NotEmpty().Matches("^[0-9]{10}$");
        RuleFor(x => x.Password).NotEmpty();
    }
}
