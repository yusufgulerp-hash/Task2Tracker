using MediatR;

namespace Task2Tracker.Application.Features.Projects.Commands.UpdateProject;

public sealed record UpdateProjectCommand(
    Guid Id,
    string Name)
    : IRequest;