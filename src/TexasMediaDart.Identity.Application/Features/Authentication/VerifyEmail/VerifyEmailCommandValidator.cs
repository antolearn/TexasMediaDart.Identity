using FluentValidation;

namespace TexasMediaDart.Identity.Application.Features.Authentication.VerifyEmail;

public sealed class VerifyEmailCommandValidator
    : AbstractValidator<VerifyEmailCommand>
{
    public VerifyEmailCommandValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty()
            .WithMessage("Email verification token is required.")
            .MaximumLength(500)
            .WithMessage("Email verification token is invalid.");
    }
}