namespace TexasMediaDart.Identity.Application.Features.Authentication.VerifyEmail;

public sealed record VerifyEmailCommand(
    string Token);