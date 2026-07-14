using MediatR;
using Microsoft.AspNetCore.Mvc;
using Task2Tracker.Application.Features.Users.Commands.CreateUser;
using Task2Tracker.Application.Features.Users.Commands.DeleteUser;
using Task2Tracker.Application.Features.Users.Commands.UpdateUser;
using Task2Tracker.Application.Features.Users.DTOs;
using Task2Tracker.Application.Features.Users.Queries.GetAllUsers;
using Task2Tracker.Application.Features.Users.Queries.SearchUsers;
using Task2Tracker.WebAPI.Contracts.Users;

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

    // POST: api/users
    [HttpPost]
    public async Task<ActionResult<Guid>> Create(
        [FromBody] CreateUserRequest request)
    {
        var command = new CreateUserCommand(
            request.FirstName,
            request.LastName,
            request.Email);

        var userId = await _mediator.Send(command);

        return Ok(userId);
    }

    // GET: api/users
    [HttpGet]
    public async Task<ActionResult<List<UserListItemDto>>> GetAll()
    {
        var users = await _mediator.Send(new GetAllUsersQuery());

        return Ok(users);
    }
    [HttpGet("search")]
    public async Task<ActionResult<List<UserListItemDto>>> Search(
    [FromQuery] string text)
    {
        var users = await _mediator.Send(new SearchUsersQuery(text));

        return Ok(users);
    }
    // PUT: api/users/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateUserRequest request)
    {
        var command = new UpdateUserCommand(
            id,
            request.FirstName,
            request.LastName,
            request.Email);

        await _mediator.Send(command);

        return NoContent();
    }
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteUserCommand(id));

        return NoContent();
    }

    // Geçici placeholder.
    [ApiExplorerSettings(IgnoreApi = true)]
    [NonAction]
    public IActionResult GetById(Guid id)
    {
        throw new NotImplementedException();
    }
}