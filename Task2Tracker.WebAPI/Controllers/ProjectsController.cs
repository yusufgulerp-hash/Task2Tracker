using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Task2Tracker.Application.Features.Projects.Commands.AddProjectMember;
using Task2Tracker.Application.Features.Projects.Commands.CreateProject;
using Task2Tracker.Application.Features.Projects.Commands.DeleteProject;
using Task2Tracker.Application.Features.Projects.Commands.RemoveProjectMember;
using Task2Tracker.Application.Features.Projects.Commands.UpdateProject;
using Task2Tracker.Application.Features.Projects.DTOs;
using Task2Tracker.Application.Features.Projects.Queries.GetAllProjects;
using Task2Tracker.Application.Features.Projects.Queries.GetProjectById;
using Task2Tracker.Application.Features.Projects.Queries.GetProjectDashboard;
using Task2Tracker.Application.Features.Projects.Queries.GetProjectMembers;
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

    // GET: api/projects/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProjectDetailDto>> GetById(Guid id)
    {
        var project = await _mediator.Send(new GetProjectByIdQuery(id));

        return Ok(project);
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

    // GET: api/projects/{id}/dashboard
    // Proje -> Üyeler -> Task'ları ağaç görünümü (+ sahipsiz task'lar)
    [HttpGet("{id:guid}/dashboard")]
    public async Task<ActionResult<ProjectDashboardDto>> GetDashboard(Guid id)
    {
        var dashboard = await _mediator.Send(new GetProjectDashboardQuery(id));

        return Ok(dashboard);
    }

    // GET: api/projects/{id}/members
    [HttpGet("{id:guid}/members")]
    public async Task<ActionResult<List<ProjectMemberDto>>> GetMembers(Guid id)
    {
        var members = await _mediator.Send(new GetProjectMembersQuery(id));

        return Ok(members);
    }

    // POST: api/projects/{id}/members
    [HttpPost("{id:guid}/members")]
    public async Task<IActionResult> AddMember(
        Guid id,
        [FromBody] AddProjectMemberRequest request)
    {
        await _mediator.Send(new AddProjectMemberCommand(id, request.UserId));

        return NoContent();
    }

    // DELETE: api/projects/{id}/members/{userId}
    [HttpDelete("{id:guid}/members/{userId:guid}")]
    public async Task<IActionResult> RemoveMember(Guid id, Guid userId)
    {
        await _mediator.Send(new RemoveProjectMemberCommand(id, userId));

        return NoContent();
    }
}