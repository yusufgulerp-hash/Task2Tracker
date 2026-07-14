using MediatR;

namespace Task2Tracker.Application.Features.Projects.Commands.CreateProject;

public sealed record CreateProjectCommand(
    string Name)
    : IRequest<Guid>;