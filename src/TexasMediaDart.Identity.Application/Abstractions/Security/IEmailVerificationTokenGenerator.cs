namespace TexasMediaDart.Identity.Application.Abstractions.Security;

public interface IEmailVerificationTokenGenerator
{
    EmailVerificationToken Generate();
}

public sealed record EmailVerificationToken(
    string Token,
    string TokenHash);