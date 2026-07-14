using MediatR;
using Task2Tracker.Application.Features.Projects.DTOs;

namespace Task2Tracker.Application.Features.Projects.Queries.SearchProjects;

public sealed record SearchProjectsQuery(string Text)
    : IRequest<List<ProjectListItemDto>>;