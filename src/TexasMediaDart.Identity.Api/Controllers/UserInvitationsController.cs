using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TexasMediaDart.Identity.Api.Models.UserInvitations;
using TexasMediaDart.Identity.Application.Features.UserInvitations.Commands.AcceptUserInvitation;
using TexasMediaDart.Identity.Application.Features.UserInvitations.Commands.CreateUserInvitation;
using TexasMediaDart.Identity.Application.Features.UserInvitations.Queries.ValidateUserInvitation;
using TexasMediaDart.Identity.Application.Features.UserInvitations.Commands.FinalizeUserInvitation;

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
    private readonly FinalizeUserInvitationCommandHandler _finalizeHandler;
    private readonly IValidator<FinalizeUserInvitationCommand> _finalizeValidator;


    public UserInvitationsController(
        CreateUserInvitationCommandHandler createHandler,
        IValidator<CreateUserInvitationCommand> createValidator,
        ValidateUserInvitationQueryHandler validateHandler,
        AcceptUserInvitationCommandHandler acceptHandler,
        IValidator<AcceptUserInvitationCommand> acceptValidator,
        FinalizeUserInvitationCommandHandler finalizeHandler,
        IValidator<FinalizeUserInvitationCommand> finalizeValidator)
    {
        _createHandler = createHandler;
        _createValidator = createValidator;
        _validateHandler = validateHandler;
        _acceptHandler = acceptHandler;
        _acceptValidator = acceptValidator;
        _finalizeHandler = finalizeHandler;
        _finalizeValidator = finalizeValidator;
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
    [HttpPost("{invitationId:guid}/finalize")]
    [ProducesResponseType(
        typeof(FinalizeUserInvitationResult),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    public async Task<IActionResult> Finalize(
        Guid invitationId,
        [FromBody] FinalizeUserInvitationRequest request,
        CancellationToken cancellationToken)
    {
        var command = new FinalizeUserInvitationCommand(
            invitationId,
            request.IdentityUserId);

        var validationResult =
            await _finalizeValidator.ValidateAsync(
                command,
                cancellationToken);

        if (!validationResult.IsValid)
        {
            return ValidationProblem(
                new ValidationProblemDetails(
                    validationResult
                        .ToDictionary())
                {
                    Title = "Invitation finalization validation failed.",
                    Status = StatusCodes.Status400BadRequest
                });
        }

        try
        {
            var result =
                await _finalizeHandler.HandleAsync(
                    command,
                    cancellationToken);

            return Ok(result);
        }
        catch (SqlException ex) when (ex.Number == 51024)
        {
            return Problem(
                title: "Invitation finalization failed.",
                detail: "The invitation was not found.",
                statusCode: StatusCodes.Status404NotFound);
        }
        catch (SqlException ex) when (ex.Number == 51025)
        {
            return Problem(
                title: "Invitation finalization failed.",
                detail:
                    "The identity account has not been created for this invitation.",
                statusCode: StatusCodes.Status409Conflict);
        }
        catch (SqlException ex) when (ex.Number == 51026)
        {
            return Problem(
                title: "Invitation finalization failed.",
                detail:
                    "The identity user does not match the invitation.",
                statusCode: StatusCodes.Status409Conflict);
        }
        catch (SqlException ex) when (ex.Number == 51027)
        {
            return Problem(
                title: "Invitation finalization failed.",
                detail: "The invitation has been revoked.",
                statusCode: StatusCodes.Status410Gone);
        }
    }
}