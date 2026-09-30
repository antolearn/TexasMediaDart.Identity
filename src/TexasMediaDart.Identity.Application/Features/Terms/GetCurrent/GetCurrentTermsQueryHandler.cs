using TexasMediaDart.Identity.Application.Abstractions.Persistence;

namespace TexasMediaDart.Identity.Application.Features.Terms.GetCurrent;

public sealed class GetCurrentTermsQueryHandler
{
    private readonly ITermsRepository _termsRepository;

    public GetCurrentTermsQueryHandler(
        ITermsRepository termsRepository)
    {
        _termsRepository = termsRepository;
    }

    public async Task<CurrentTermsResponse?> HandleAsync(
        GetCurrentTermsQuery query,
        CancellationToken cancellationToken = default)
    {
        return await _termsRepository.GetCurrentAsync(
            cancellationToken);
    }
}