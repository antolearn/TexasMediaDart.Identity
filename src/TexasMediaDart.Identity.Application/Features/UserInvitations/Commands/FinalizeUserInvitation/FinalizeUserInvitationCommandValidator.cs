using FluentValidation;

namespace TexasMediaDart.Identity.Application.Features.UserInvitations.Commands.FinalizeUserInvitation;

public sealed class FinalizeUserInvitationCommandValidator
    : AbstractValidator<FinalizeUserInvitationCommand>
{
    public FinalizeUserInvitationCommandValidator()
    {
        RuleFor(x => x.InvitationId)
            .NotEmpty()
            .WithMessage("Invitation id is required.");

        RuleFor(x => x.IdentityUserId)
            .NotEmpty()
            .WithMessage("Identity user id is required.");
    }
}