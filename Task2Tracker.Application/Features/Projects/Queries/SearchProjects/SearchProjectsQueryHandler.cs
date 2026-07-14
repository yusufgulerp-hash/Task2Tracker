using MediatR;
using Task2Tracker.Application.Features.Projects.DTOs;
using Task2Tracker.Application.Interfaces.Repositories;

namespace Task2Tracker.Application.Features.Projects.Queries.SearchProjects;

public sealed class SearchProjectsQueryHandler
    : IRequestHandler<SearchProjectsQuery, List<ProjectListItemDto>>
{
    private readonly IProjectRepository _projectRepository;

    public SearchProjectsQueryHandler(
        IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<List<ProjectListItemDto>> Handle(
        SearchProjectsQuery request,
        CancellationToken cancellationToken)
    {
        var projects = await _projectRepository.SearchAsync(
            request.Text,
            cancellationToken);

        return projects
            .Select(x => new ProjectListItemDto(
                x.Id,
                x.Name))
            .ToList();
    }
}