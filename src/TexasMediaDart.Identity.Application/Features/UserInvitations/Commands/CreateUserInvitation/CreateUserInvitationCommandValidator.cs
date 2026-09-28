using FluentValidation;

namespace TexasMediaDart.Identity.Application.Features.UserInvitations.Commands.CreateUserInvitation;

public sealed class CreateUserInvitationCommandValidator
    : AbstractValidator<CreateUserInvitationCommand>
{
    public CreateUserInvitationCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("A valid email address is required.")
            .MaximumLength(320)
            .WithMessage("Email cannot exceed 320 characters.");

        RuleFor(x => x.OrganizationId)
            .NotEmpty()
            .WithMessage("Organization id is required.");

        RuleFor(x => x.InvitedByIdentityUserId)
            .NotEmpty()
            .WithMessage("Invited by identity user id is required.");
    }
}