using TexasMediaDart.Identity.Application.Features.Terms.GetCurrent;

namespace TexasMediaDart.Identity.Application.Abstractions.Persistence;

public interface ITermsRepository
{
    Task<CurrentTermsResponse?> GetCurrentAsync(
        CancellationToken cancellationToken = default);
}