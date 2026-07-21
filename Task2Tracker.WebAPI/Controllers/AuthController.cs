using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using Task2Tracker.Application.Features.Auth.Commands.Login;
using Task2Tracker.Application.Features.Auth.Commands.Logout;
using Task2Tracker.Application.Features.Auth.Commands.Refresh;
using Task2Tracker.Application.Features.Auth.Commands.Register;
using Task2Tracker.Application.Features.Auth.DTOs;
using LoginRequest = Task2Tracker.WebAPI.Contracts.Auth.LoginRequest;
using RegisterRequest = Task2Tracker.WebAPI.Contracts.Auth.RegisterRequest;

namespace Task2Tracker.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly ISender _mediator;
    private const string RefreshTokenCookieName = "refreshToken";

    public AuthController(ISender mediator)
    {
        _mediator = mediator;
    }

    // POST: api/auth/register
    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponseDto>> Register(
        [FromBody] RegisterRequest request)
    {
        var command = new RegisterCommand(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Password);

        var result = await _mediator.Send(command);

        return StatusCode(
            StatusCodes.Status201Created,
            result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(
        [FromBody] LoginRequest request)
    {
        var command = new LoginCommand(request.Email, request.Password);

        var result = await _mediator.Send(command);

        SetRefreshTokenCookie(result.RefreshToken);

        return Ok(result with { RefreshToken = string.Empty });
    }

    // POST: api/auth/refresh
    // Client bu endpoint'i access token expire olunca çağırır

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponseDto>> Refresh()
    {
        // Refresh token'ı cookie'den okuyoruz — body'den değil
        var refreshToken = Request.Cookies[RefreshTokenCookieName];

        if (string.IsNullOrEmpty(refreshToken))
            return Unauthorized("Refresh token bulunamadı.");

        var result = await _mediator.Send(new RefreshCommand(refreshToken));

        SetRefreshTokenCookie(result.RefreshToken);

        return Ok(result with { RefreshToken = string.Empty });
    }

    // POST: api/auth/logout
  
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        // JWT'den userId'yi okuyoruz
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
            ?? User.FindFirst("sub");

        if (userIdClaim is null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized();

        await _mediator.Send(new LogoutCommand(userId));

        // Cookie'yi temizle
        Response.Cookies.Delete(RefreshTokenCookieName);

        return NoContent();
    }

    private void SetRefreshTokenCookie(string refreshToken)
    {
        // HttpOnly: JS erişemez (XSS koruması)
        // Secure: sadece HTTPS üzerinden gönderilir
        // SameSite.Strict: başka sitelerden gelen isteklerde cookie gönderilmez (CSRF koruması)
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        };

        Response.Cookies.Append(
            RefreshTokenCookieName,
            refreshToken,
            cookieOptions);
    }
}