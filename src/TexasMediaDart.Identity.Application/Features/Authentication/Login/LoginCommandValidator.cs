using FluentValidation;

namespace TexasMediaDart.Identity.Application.Features.Authentication.Login;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("A valid email address is required.")
            .MaximumLength(320)
            .WithMessage("Email cannot exceed 320 characters.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MaximumLength(128)
            .WithMessage("Password cannot exceed 128 characters.");
    }
}