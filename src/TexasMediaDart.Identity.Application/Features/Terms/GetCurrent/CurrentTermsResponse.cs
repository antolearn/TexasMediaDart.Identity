namespace TexasMediaDart.Identity.Application.Features.Terms.GetCurrent;

public sealed record CurrentTermsResponse(
    string Version,
    string Title,
    string Content,
    DateTime EffectiveUtc);