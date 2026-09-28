using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TexasMediaDart.Identity.Api.Models.UserInvitations;
using TexasMediaDart.Identity.Application.Features.UserInvitations.Commands.AcceptUserInvitation;
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
    private readonly AcceptUserInvitationCommandHandler _acceptHandler;
    private readonly IValidator<AcceptUserInvitationCommand> _acceptValidator;

    public UserInvitationsController(
        CreateUserInvitationCommandHandler createHandler,
        IValidator<CreateUserInvitationCommand> createValidator,
        ValidateUserInvitationQueryHandler validateHandler,
        AcceptUserInvitationCommandHandler acceptHandler,
        IValidator<AcceptUserInvitationCommand> acceptValidator)
    {
        _createHandler = createHandler;
        _createValidator = createValidator;
        _validateHandler = validateHandler;
        _acceptHandler = acceptHandler;
        _acceptValidator = acceptValidator;
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

    [HttpPost("accept")]
    [AllowAnonymous]
    [ProducesResponseType(
        typeof(AcceptUserInvitationResult),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    public async Task<IActionResult> Accept(
        [FromBody] AcceptUserInvitationRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AcceptUserInvitationCommand(
            request.Token,
            request.Password,
            request.ConfirmPassword);

        var validationResult =
            await _acceptValidator.ValidateAsync(
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
                    Title = "Invitation acceptance validation failed.",
                    Status = StatusCodes.Status400BadRequest
                });
        }

        try
        {
            var result = await _acceptHandler.HandleAsync(
                command,
                cancellationToken);

            return Ok(result);
        }
        catch (SqlException ex) when (ex.Number == 51017)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invitation acceptance failed.",
                detail: "The invitation is invalid.");
        }
        catch (SqlException ex) when (ex.Number == 51018)
        {
            return Problem(
                statusCode: StatusCodes.Status410Gone,
                title: "Invitation acceptance failed.",
                detail: "The invitation has been revoked.");
        }
        catch (SqlException ex) when (ex.Number == 51019)
        {
            return Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Invitation acceptance failed.",
                detail: "The invitation has already been accepted.");
        }
        catch (SqlException ex) when (ex.Number == 51020)
        {
            return Problem(
                statusCode: StatusCodes.Status410Gone,
                title: "Invitation acceptance failed.",
                detail: "The invitation has expired.");
        }
        catch (SqlException ex) when (ex.Number == 51021)
        {
            return Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Invitation acceptance failed.",
                detail: "A user with this email already exists.");
        }
    }
}