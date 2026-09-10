using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TexasMediaDart.Identity.Application.Features.Authentication.Login;
using TexasMediaDart.Identity.Application.Features.Authentication.Register;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using TexasMediaDart.Identity.Application.Features.Authentication.Refresh;
using TexasMediaDart.Identity.Application.Features.Authentication.Logout;

namespace TexasMediaDart.Identity.Api.Controllers;

[ApiController]

[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly RegisterCommandHandler _registerHandler;
    private readonly IValidator<RegisterCommand> _registerValidator;

    private readonly LoginCommandHandler _loginHandler;
    private readonly IValidator<LoginCommand> _loginValidator;
    private readonly RefreshCommandHandler _refreshHandler;
    private readonly LogoutCommandHandler _logoutHandler;

    public AuthController(
        RegisterCommandHandler registerHandler,
        IValidator<RegisterCommand> registerValidator,
        LoginCommandHandler loginHandler,
        IValidator<LoginCommand> loginValidator,
        RefreshCommandHandler refreshHandler,
        LogoutCommandHandler logoutHandler)
    {
        _registerHandler = registerHandler;
        _registerValidator = registerValidator;
        _loginHandler = loginHandler;
        _loginValidator = loginValidator;
        _refreshHandler = refreshHandler;
        _logoutHandler = logoutHandler;
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var userId =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        var email =
            User.FindFirstValue(ClaimTypes.Email)
            ?? User.FindFirstValue("email");

        return Ok(new
        {
            UserId = userId,
            Email = email
        });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult =
            await _registerValidator.ValidateAsync(
                command,
                cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group
                        .Select(x => x.ErrorMessage)
                        .ToArray());

            return ValidationProblem(
                new ValidationProblemDetails(errors)
                {
                    Title = "Registration validation failed.",
                    Status = StatusCodes.Status400BadRequest
                });
        }

        try
        {
            var result = await _registerHandler.HandleAsync(
                command,
                cancellationToken);

            return Created(
                $"/api/auth/users/{result.UserId}",
                result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(
                new ProblemDetails
                {
                    Title = "Registration failed.",
                    Detail = ex.Message,
                    Status = StatusCodes.Status409Conflict
                });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult =
            await _loginValidator.ValidateAsync(
                command,
                cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group
                        .Select(x => x.ErrorMessage)
                        .ToArray());

            return ValidationProblem(
                new ValidationProblemDetails(errors)
                {
                    Title = "Login validation failed.",
                    Status = StatusCodes.Status400BadRequest
                });
        }

        try
        {
            var result = await _loginHandler.HandleAsync(
                command,
                cancellationToken);

            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(
                new ProblemDetails
                {
                    Title = "Login failed.",
                    Detail = "Invalid email or password.",
                    Status = StatusCodes.Status401Unauthorized
                });
        }
    }
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshCommand command,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.RefreshToken))
        {
            return ValidationProblem(
                new ValidationProblemDetails(
                    new Dictionary<string, string[]>
                    {
                        ["RefreshToken"] =
                        [
                            "Refresh token is required."
                        ]
                    })
                {
                    Title = "Refresh token validation failed.",
                    Status = StatusCodes.Status400BadRequest
                });
        }

        try
        {
            var result =
                await _refreshHandler.HandleAsync(
                    command,
                    cancellationToken);

            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(
                new ProblemDetails
                {
                    Title = "Token refresh failed.",
                    Detail = "Invalid or expired refresh token.",
                    Status = StatusCodes.Status401Unauthorized
                });
        }
    }
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(
        [FromBody] LogoutCommand command,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.RefreshToken))
        {
            return ValidationProblem(
                new ValidationProblemDetails(
                    new Dictionary<string, string[]>
                    {
                        ["RefreshToken"] =
                        [
                            "Refresh token is required."
                        ]
                    })
                {
                    Title = "Logout validation failed.",
                    Status = StatusCodes.Status400BadRequest
                });
        }

        await _logoutHandler.HandleAsync(
            command,
            cancellationToken);

        return NoContent();
    }
}