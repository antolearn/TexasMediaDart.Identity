using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TexasMediaDart.Identity.Api.Models.Users;
using TexasMediaDart.Identity.Application.Features.Users.Models;
using TexasMediaDart.Identity.Application.Features.Users.Queries.LookupUsers;

namespace TexasMediaDart.Identity.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public sealed class UsersController : ControllerBase
{
    private readonly LookupUsersQueryHandler _lookupUsersHandler;

    public UsersController(
        LookupUsersQueryHandler lookupUsersHandler)
    {
        _lookupUsersHandler = lookupUsersHandler;
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
}