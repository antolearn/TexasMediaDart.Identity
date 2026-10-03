namespace TexasMediaDart.Identity.Application.Features.Authentication.VerifyEmail;

public enum EmailVerificationFailureReason
{
    InvalidToken,
    InvalidatedToken,
    AlreadyUsed,
    Expired,
    UserUnavailable
}

public sealed class EmailVerificationException : Exception
{
    public EmailVerificationException(
        EmailVerificationFailureReason reason,
        string message)
        : base(message)
    {
        Reason = reason;
    }

    public EmailVerificationFailureReason Reason { get; }
}