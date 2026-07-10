using MediatR;
using Microsoft.AspNetCore.Mvc;
using Task2Tracker.Application.Features.Users.Commands.CreateUser;
using Task2Tracker.Application.Features.Users.Queries.GetAllUsers;
using Task2Tracker.Domain.Entities;

namespace Task2Tracker.WebAPI.Controllers;


[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ISender _mediator;

    public UsersController(ISender mediator)
    {
        _mediator = mediator;
    }

    // Kullanıcı oluşturma (Zaten vardı)
    [HttpPost("details/age")]
    public async Task<ActionResult<Guid>> Create(CreateUserCommand command)
    {
        var userId = await _mediator.Send(command);
        return Ok(userId);
    }

    // Kullanıcı oluşturma (Zaten vardı)
    [HttpPost("asd")]
    public async Task<ActionResult<Guid>> Create2(CreateUserCommand command)
    {
        var userId = await _mediator.Send(command);
        return Ok(userId);
    }

    // Kullanıcıları listeleme (Yeni eklediğimiz)
    [HttpGet]
    public async Task<ActionResult<List<User>>> GetAll()
    {
        var users = await _mediator.Send(new GetAllUsersQuery());
        return Ok(users);
    }
}