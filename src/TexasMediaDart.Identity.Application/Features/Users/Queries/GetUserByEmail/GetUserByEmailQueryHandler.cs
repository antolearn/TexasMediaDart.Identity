using TexasMediaDart.Identity.Application.Abstractions.Persistence;
using TexasMediaDart.Identity.Application.Features.Users.Models;

namespace TexasMediaDart.Identity.Application.Features.Users.Queries.GetUserByEmail;

public sealed class GetUserByEmailQueryHandler
{
    private readonly IUserRepository _userRepository;

    public GetUserByEmailQueryHandler(
        IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserLookupDto?> HandleAsync(
        GetUserByEmailQuery query,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query.Email))
        {
            return null;
        }

        var user = await _userRepository.GetByEmailAsync(
            query.Email.Trim(),
            cancellationToken);

        if (user is null || user.IsDeleted)
        {
            return null;
        }

        return new UserLookupDto
        {
            UserId = user.UserId,
            Email = user.Email,
            IsActive = user.IsActive,
            IsEmailVerified = user.IsEmailVerified
        };
    }
}