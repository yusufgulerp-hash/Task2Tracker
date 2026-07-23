using MediatR;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Task2Tracker.Application.Features.Auth.Commands.Login;
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
        var command = new LoginCommand(
            request.Email,
            request.Password);

        var result = await _mediator.Send(command);

        return Ok(result);
    }
}