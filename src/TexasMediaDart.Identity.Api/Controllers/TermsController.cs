using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TexasMediaDart.Identity.Application.Features.Terms.GetCurrent;

namespace TexasMediaDart.Identity.Api.Controllers;

[ApiController]
[Route("api/terms")]
public sealed class TermsController : ControllerBase
{
    private readonly GetCurrentTermsQueryHandler _handler;

    public TermsController(
        GetCurrentTermsQueryHandler handler)
    {
        _handler = handler;
    }

    [AllowAnonymous]
    [HttpGet("current")]
    [ProducesResponseType(
        typeof(CurrentTermsResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CurrentTermsResponse>> GetCurrent(
        CancellationToken cancellationToken)
    {
        var query = new GetCurrentTermsQuery();

        var terms = await _handler.HandleAsync(
            query,
            cancellationToken);

        if (terms is null)
        {
            return NotFound();
        }

        return Ok(terms);
    }
}