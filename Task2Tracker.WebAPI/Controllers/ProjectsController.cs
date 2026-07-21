using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Task2Tracker.Application.Features.Projects.Commands.CreateProject;
using Task2Tracker.Application.Features.Projects.Commands.DeleteProject;
using Task2Tracker.Application.Features.Projects.Commands.UpdateProject;
using Task2Tracker.Application.Features.Projects.DTOs;
using Task2Tracker.Application.Features.Projects.Queries.GetAllProjects;
using Task2Tracker.Application.Features.Projects.Queries.SearchProjects;
using Task2Tracker.WebAPI.Contracts.Projects;

namespace Task2Tracker.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class ProjectsController : ControllerBase
{
    private readonly ISender _mediator;

    public ProjectsController(ISender mediator)
    {
        _mediator = mediator;
    }

    // POST: api/projects
 
    [HttpPost]
    public async Task<ActionResult<Guid>> Create(
        [FromBody] CreateProjectRequest request)
    {
        var command = new CreateProjectCommand(request.Name);

        var projectId = await _mediator.Send(command);

        return Ok(projectId);
    }

    // GET: api/projects
 
    [HttpGet]
    public async Task<ActionResult<List<ProjectListItemDto>>> GetAll()
    {
        var projects = await _mediator.Send(new GetAllProjectsQuery());

        return Ok(projects);
    }

    // GET: api/projects/search?text=test
    [HttpGet("search")]
  
    public async Task<ActionResult<List<ProjectListItemDto>>> Search(
        [FromQuery] string text)
    {
        var projects = await _mediator.Send(
            new SearchProjectsQuery(text));

        return Ok(projects);
    }

    // PUT: api/projects/{id}
  
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateProjectRequest request)
    {
        var command = new UpdateProjectCommand(
            id,
            request.Name);

        await _mediator.Send(command);

        return NoContent();
    }

    // DELETE: api/projects/{id}

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteProjectCommand(id));

        return NoContent();
    }
}