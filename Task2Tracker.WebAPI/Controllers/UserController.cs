using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Task2Tracker.Application.Features.Users.Commands.ApproveUser;
using Task2Tracker.Application.Features.Users.Commands.DeleteUser;
using Task2Tracker.Application.Features.Users.Commands.RejectUser;
using Task2Tracker.Application.Features.Users.Commands.UpdateUser;
using Task2Tracker.Application.Features.Users.DTOs;
using Task2Tracker.Application.Features.Users.Queries.GetAllUsers;
using Task2Tracker.Application.Features.Users.Queries.GetPendingUsers;
using Task2Tracker.Application.Features.Users.Queries.GetUserById;
using Task2Tracker.Application.Features.Users.Queries.SearchUsers;
using Task2Tracker.Domain.Enums;
using Task2Tracker.WebAPI.Contracts.Users;

namespace Task2Tracker.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ISender _mediator;

    public UsersController(ISender mediator)
    {
        _mediator = mediator;
    }
    // GET: api/users/pending

    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpGet("pending")]
    public async Task<ActionResult<List<PendingUserDto>>> GetPendingUsers()
    {
        var users = await _mediator.Send(
            new GetPendingUsersQuery());

        return Ok(users);
    }

    // GET: api/users

    [HttpGet]
    public async Task<ActionResult<List<UserListItemDto>>> GetAll()
    {
        var users = await _mediator.Send(new GetAllUsersQuery());

        return Ok(users);
    }
    // GET: api/users/search?text=...
  
    [HttpGet("search")]
    public async Task<ActionResult<List<UserListItemDto>>> Search(
    [FromQuery] string text)
    {
        var users = await _mediator.Send(new SearchUsersQuery(text));

        return Ok(users);
    }
    // GET: api/users/{id}

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDetailDto>> GetById(Guid id)
    {
        var user = await _mediator.Send(new GetUserByIdQuery(id));

        return Ok(user);
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
    // DELETE: api/users/{id}
 
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteUserCommand(id));

        return NoContent();
    }
    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> ApproveUser(
    Guid id)
    {
        await _mediator.Send(
            new ApproveUserCommand(id));

        return NoContent();
    }
    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpPost("{id:guid}/reject")]
    public async Task<IActionResult> RejectUser(
    Guid id)
    {
        await _mediator.Send(
            new RejectUserCommand(id));

        return NoContent();
    }

}