using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TexasMediaDart.Identity.Api.Models.Users;
using TexasMediaDart.Identity.Application.Features.Users.Models;
using TexasMediaDart.Identity.Application.Features.Users.Queries.GetUserByEmail;
using TexasMediaDart.Identity.Application.Features.Users.Queries.LookupUsers;
using TexasMediaDart.Identity.Application.Features.Users.Queries.SearchUsersByEmailAndIds;

namespace TexasMediaDart.Identity.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public sealed class UsersController : ControllerBase
{
    private readonly LookupUsersQueryHandler _lookupUsersHandler;
    private readonly SearchUsersByEmailAndIdsQueryHandler
        _searchUsersByEmailAndIdsHandler;
    private readonly GetUserByEmailQueryHandler _getUserByEmailHandler;

    public UsersController(
        LookupUsersQueryHandler lookupUsersHandler,
        SearchUsersByEmailAndIdsQueryHandler searchUsersByEmailAndIdsHandler,
        GetUserByEmailQueryHandler getUserByEmailHandler)
    {
        _lookupUsersHandler = lookupUsersHandler;
        _searchUsersByEmailAndIdsHandler =
            searchUsersByEmailAndIdsHandler;
        _getUserByEmailHandler = getUserByEmailHandler;
    }

    [HttpGet("by-email")]
    [ProducesResponseType(
        typeof(UserLookupDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByEmail(
        [FromQuery] string? email,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return BadRequest(new
            {
                message = "Email is required."
            });
        }

        var query = new GetUserByEmailQuery(
            email.Trim());

        var result = await _getUserByEmailHandler.HandleAsync(
            query,
            cancellationToken);

        if (result is null)
        {
            return NotFound(new
            {
                message = "User not found."
            });
        }

        return Ok(result);
    }

    [HttpPost("lookup")]
    [ProducesResponseType(
        typeof(IReadOnlyList<UserLookupDto>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Lookup(
        [FromBody] UserLookupRequest request,
        CancellationToken cancellationToken)
    {
        if (request.UserIds.Count == 0)
        {
            return BadRequest(new
            {
                message = "At least one user id is required."
            });
        }

        var userIds = request.UserIds
            .Distinct()
            .ToArray();

        var query = new LookupUsersQuery(userIds);

        var result = await _lookupUsersHandler.HandleAsync(
            query,
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("search-by-email")]
    [ProducesResponseType(
        typeof(IReadOnlyList<UserLookupDto>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SearchByEmail(
        [FromBody] UserEmailSearchRequest request,
        CancellationToken cancellationToken)
    {
        if (request.UserIds.Count == 0)
        {
            return BadRequest(new
            {
                message = "At least one user id is required."
            });
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return BadRequest(new
            {
                message = "Email is required."
            });
        }

        var userIds = request.UserIds
            .Distinct()
            .ToArray();

        var email = request.Email.Trim();

        var query = new SearchUsersByEmailAndIdsQuery(
            userIds,
            email);

        var result =
            await _searchUsersByEmailAndIdsHandler.HandleAsync(
                query,
                cancellationToken);

        return Ok(result);
    }
}