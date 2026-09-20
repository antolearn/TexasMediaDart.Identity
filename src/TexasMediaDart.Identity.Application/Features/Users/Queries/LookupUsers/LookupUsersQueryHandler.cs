using TexasMediaDart.Identity.Application.Abstractions.Persistence;
using TexasMediaDart.Identity.Application.Features.Users.Models;

namespace TexasMediaDart.Identity.Application.Features.Users.Queries.LookupUsers;

public sealed class LookupUsersQueryHandler
{
    private readonly IUserRepository _userRepository;

    public LookupUsersQueryHandler(
        IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IReadOnlyList<UserLookupDto>> HandleAsync(
        LookupUsersQuery query,
        CancellationToken cancellationToken = default)
    {
        if (query.UserIds.Count == 0)
        {
            return Array.Empty<UserLookupDto>();
        }

        var users = await _userRepository.GetByIdsAsync(
            query.UserIds,
            cancellationToken);

        return users
            .Where(user => !user.IsDeleted)
            .Select(user => new UserLookupDto
            {
                UserId = user.UserId,
                Email = user.Email,
                IsActive = user.IsActive,
                IsEmailVerified = user.IsEmailVerified
            })
            .ToList();
    }
}