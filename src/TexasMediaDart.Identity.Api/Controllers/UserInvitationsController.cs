using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TexasMediaDart.Identity.Api.Models.UserInvitations;
using TexasMediaDart.Identity.Application.Features.UserInvitations.Commands.CreateUserInvitation;
using TexasMediaDart.Identity.Application.Features.UserInvitations.Queries.ValidateUserInvitation;

namespace TexasMediaDart.Identity.Api.Controllers;

[ApiController]
[Route("api/user-invitations")]
[Authorize]
public sealed class UserInvitationsController : ControllerBase
{
    private readonly CreateUserInvitationCommandHandler _createHandler;
    private readonly IValidator<CreateUserInvitationCommand> _createValidator;
    private readonly ValidateUserInvitationQueryHandler _validateHandler;
    public UserInvitationsController(
        CreateUserInvitationCommandHandler createHandler,
        IValidator<CreateUserInvitationCommand> createValidator,
        ValidateUserInvitationQueryHandler validateHandler)
    {
        _createHandler = createHandler;
        _createValidator = createValidator;
        _validateHandler = validateHandler;
    }

    [HttpPost]
    [ProducesResponseType(
        typeof(CreateUserInvitationResult),
        StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreateUserInvitationRequest request,
        CancellationToken cancellationToken)
    {
        var identityUserIdValue =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (!Guid.TryParse(
                identityUserIdValue,
                out var invitedByIdentityUserId))
        {
            return Unauthorized();
        }

        var command = new CreateUserInvitationCommand(
            request.Email,
            request.OrganizationId,
            invitedByIdentityUserId);

        var validationResult =
            await _createValidator.ValidateAsync(
                command,
                cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(error => error.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group
                        .Select(error => error.ErrorMessage)
                        .ToArray());

            return ValidationProblem(
                new ValidationProblemDetails(errors)
                {
                    Title = "Invitation validation failed.",
                    Status = StatusCodes.Status400BadRequest
                });
        }

        try
        {
            var result = await _createHandler.HandleAsync(
                command,
                cancellationToken);

            return StatusCode(
                StatusCodes.Status201Created,
                result);
        }
        catch (InvalidOperationException ex)
        {
            return Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Invitation failed.",
                detail: ex.Message);
        }
    }

    [HttpGet("validate")]
    [AllowAnonymous]
    [ProducesResponseType(
        typeof(ValidateUserInvitationResult),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> Validate(
        [FromQuery] string? token,
        CancellationToken cancellationToken)
    {
        var query = new ValidateUserInvitationQuery(
            token ?? string.Empty);

        var result = await _validateHandler.HandleAsync(
            query,
            cancellationToken);

        return Ok(result);
    }

}