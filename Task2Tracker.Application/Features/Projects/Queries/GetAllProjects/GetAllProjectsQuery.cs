using MediatR;
using Task2Tracker.Application.Features.Projects.DTOs;

namespace Task2Tracker.Application.Features.Projects.Queries.GetAllProjects;

public sealed record GetAllProjectsQuery
    : IRequest<List<ProjectListItemDto>>;