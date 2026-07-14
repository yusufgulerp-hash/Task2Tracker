using MediatR;
using Task2Tracker.Application.Features.Projects.DTOs;
using Task2Tracker.Application.Interfaces.Repositories;

namespace Task2Tracker.Application.Features.Projects.Queries.GetAllProjects;

public sealed class GetAllProjectsQueryHandler
    : IRequestHandler<GetAllProjectsQuery, List<ProjectListItemDto>>
{
    private readonly IProjectRepository _projectRepository;

    public GetAllProjectsQueryHandler(
        IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<List<ProjectListItemDto>> Handle(
        GetAllProjectsQuery request,
        CancellationToken cancellationToken)
    {
        var projects = await _projectRepository.GetAllAsync(cancellationToken);

        return projects
            .Select(project => new ProjectListItemDto(
                project.Id,
                project.Name))
            .ToList();
    }
}