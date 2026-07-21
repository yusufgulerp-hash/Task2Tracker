using MediatR;
using Microsoft.AspNetCore.Authorization;   
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Task2Tracker.Application.Features.Tasks.Commands.AssignTask;
using Task2Tracker.Application.Features.Tasks.Commands.CreateTask;
using Task2Tracker.Application.Features.Tasks.Commands.DeleteTask;
using Task2Tracker.Application.Features.Tasks.Commands.UnassignTask;
using Task2Tracker.Application.Features.Tasks.Commands.UpdateTask;
using Task2Tracker.Application.Features.Tasks.DTOs;
using Task2Tracker.Application.Features.Tasks.Queries.GetAllTasks;
using Task2Tracker.Application.Features.Tasks.Queries.GetTaskById;
using Task2Tracker.Application.Features.Tasks.Queries.SearchTasks;
using Task2Tracker.Domain.Enums;
using Task2Tracker.WebAPI.Contracts.Tasks;

namespace Task2Tracker.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class TasksController : ControllerBase
{
    private readonly ISender _mediator;

    public TasksController(ISender mediator)
    {
        _mediator = mediator;
    }

    // POST: api/tasks

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(
        [FromBody] CreateTaskRequest request)
    {
        var command = new CreateTaskCommand(
            request.Title,
            request.Description,
            request.Priority,
            request.ProjectId);

        var taskId = await _mediator.Send(command);

        return Ok(taskId);
    }

    // GET: api/tasks?projectId=...&userId=...&status=...&priority=...
    [HttpGet]
  
    public async Task<ActionResult<List<TaskListItemDto>>> GetAll(
        [FromQuery] Guid? projectId = null,
        [FromQuery] Guid? userId = null,
        [FromQuery] TaskProgressStatus? status = null,
        [FromQuery] TaskPriority? priority = null)
    {
        var tasks = await _mediator.Send(
            new GetAllTasksQuery(projectId, userId, status, priority));

        return Ok(tasks);
    }

    // GET: api/tasks/search?text=...

    [HttpGet("search")]
    public async Task<ActionResult<List<TaskListItemDto>>> Search(
        [FromQuery] string text)
    {
        var tasks = await _mediator.Send(new SearchTasksQuery(text));

        return Ok(tasks);
    }

    // GET: api/tasks/{id}

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TaskListItemDto>> GetById(Guid id)
    {
        var task = await _mediator.Send(new GetTaskByIdQuery(id));

        return Ok(task);
    }

    // PUT: api/tasks/{id}
 
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateTaskRequest request)
    {
        var command = new UpdateTaskCommand(
            id,
            request.Title,
            request.Description,
            request.Priority,
            request.Status,
            request.UserId);

        await _mediator.Send(command);

        return NoContent();
    }

    // PUT: api/tasks/{id}/assign
 
    [HttpPut("{id:guid}/assign")]
    public async Task<IActionResult> Assign(
        Guid id,
        [FromQuery] Guid userId)
    {
        await _mediator.Send(new AssignTaskCommand(id, userId));

        return NoContent();
    }

    // DELETE: api/tasks/{id}/assign
 
    [HttpDelete("{id:guid}/assign")]
    public async Task<IActionResult> Unassign(Guid id)
    {
        await _mediator.Send(new UnassignTaskCommand(id));

        return NoContent();
    }

    // DELETE: api/tasks/{id}
   
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteTaskCommand(id));

        return NoContent();
    }
}