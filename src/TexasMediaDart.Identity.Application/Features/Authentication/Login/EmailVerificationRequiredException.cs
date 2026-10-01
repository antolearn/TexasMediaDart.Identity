namespace TexasMediaDart.Identity.Application.Features.Authentication.Login;

public sealed class EmailVerificationRequiredException : Exception
{
    public EmailVerificationRequiredException()
        : base("Please verify your email address before signing in.")
    {
    }
}