namespace TexasMediaDart.Identity.Api.Models.Authentication;

public sealed record VerifyEmailRequest(
    string Token);